using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Configuration;
using PEAKUnlimited.Patches;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PEAKUnlimited;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    public static ConfigurationHandler ConfigurationHandler;
    private readonly Harmony _harmony = new(Id);
    public static List<Campfire> CampfireList = new();
    public static bool IsAfterAwake = false;
    public const int VanillaMaxPlayers = 4;
    public static Dictionary<Campfire, List<GameObject>> Marshmallows = new();
    public static bool HasHostStarted = false;

    private ModConfigurationUI _ui;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {Id} is loaded!");
        ConfigurationHandler = new ConfigurationHandler(Config);
        NetworkConnector.MAX_PLAYERS = ConfigurationHandler.MaxPlayers;
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,$"Plugin {Id} set the Max Players to " + NetworkConnector.MAX_PLAYERS + "!");

        //Extra marshmallow and backpack patches
        _harmony.PatchAll(typeof(CampfireAwakePatch));
        _harmony.PatchAll(typeof(OnPlayerLeftRoomPatch));
        _harmony.PatchAll(typeof(OnPlayerEnteredRoomPatch));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"Marshmallow patches successful!");

        //Lock Airport Kiosk Patches
        _harmony.PatchAll(typeof(StartGamePatch));
        _harmony.PatchAll(typeof(LoadIslandMasterPatch));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"Kiosk patches successful!");

        //End screen patches
        _harmony.PatchAll(typeof(EndSequenceRoutinePatch));
        _harmony.PatchAll(typeof(WaitingForPlayersUIPatch));
        _harmony.PatchAll(typeof(EndScreenStartPatch));
        _harmony.PatchAll(typeof(EndScreenNextPatch));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"End screen patches successful!");

        //In-game message patches
        _harmony.PatchAll(typeof(PlayerConnectionLogAwakePatch));
        _harmony.PatchAll(typeof(PlayClickedPatch));
        _harmony.PatchAll(typeof(LeaveLobbyPatch));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"Player connection log patches successful!");
        
        //Possibly help with broken audio bug?
        _harmony.PatchAll(typeof(AssignMixerGroupPatch));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"Audio patches successful!");
        
        //Disable vanilla marshmallow patch
        _harmony.PatchAll(typeof(SingleItemSpawnerTrySpawnItemsPatch));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"Item Spawner patches successful!");

        //Mod Configuration Menu
        var go = new GameObject("PEAKUnlimitedUI");
        DontDestroyOnLoad(go);
        _ui = go.AddComponent<ModConfigurationUI>();
        _ui.Init(new List<Option>
        {
            Option.Int("Max Players", ConfigurationHandler.ConfigMaxPlayers, 1, 30, isDisabled: () => PhotonNetwork.InRoom),
            Option.Bool("Extra Backpacks", ConfigurationHandler.ConfigExtraBackpacks, isDisabled: () => PhotonNetwork.InRoom && GameHandler.GetService<RichPresenceService>().m_currentState != RichPresenceState.Status_Airport),
            Option.Bool("Extra Marshmallows", ConfigurationHandler.ConfigExtraMarshmallows, isDisabled: () => PhotonNetwork.InRoom && GameHandler.GetService<RichPresenceService>().m_currentState != RichPresenceState.Status_Airport),
            Option.Bool("Late Join Marshmallows", ConfigurationHandler.ConfigLateMarshmallows, isDisabled: () => PhotonNetwork.InRoom && GameHandler.GetService<RichPresenceService>().m_currentState != RichPresenceState.Status_Airport),
            Option.Bool("Host Locked Kiosk", ConfigurationHandler.ConfigLockKiosk, isDisabled: () => PhotonNetwork.InRoom && GameHandler.GetService<RichPresenceService>().m_currentState != RichPresenceState.Status_Airport),
            Option.Bool("Lobby Details", ConfigurationHandler.ConfigLobbyDetails, isDisabled: () => PhotonNetwork.InRoom && GameHandler.GetService<RichPresenceService>().m_currentState != RichPresenceState.Status_Airport),
            Option.Int("Cheat Marshmallows", ConfigurationHandler.ConfigCheatExtraMarshmallows, 0, 30, isDisabled: () => PhotonNetwork.InRoom && GameHandler.GetService<RichPresenceService>().m_currentState != RichPresenceState.Status_Airport),
            Option.Int("Cheat Backpacks", ConfigurationHandler.ConfigCheatExtraBackpacks, 0, 10, isDisabled: () => PhotonNetwork.InRoom && GameHandler.GetService<RichPresenceService>().m_currentState != RichPresenceState.Status_Airport),
            Option.InputAction("Menu Key", ConfigurationHandler.ConfigMenuKey)
        });
    }
}