using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Peak.Network;
using PEAKUnlimited.Configuration;
using PEAKUnlimited.Patches;
using PEAKUnlimited.Patches.Voice;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PEAKUnlimited;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    public static ConfigurationHandler ConfigurationHandler;
    private readonly Harmony _harmony = new(Id);
    public static readonly HashSet<Campfire> CampfireList = new();
    public const int VanillaMaxPlayers = 4;
    public static readonly Dictionary<Campfire, HashSet<GameObject>> Marshmallows = new();
    public static bool HasHostStarted = false;

    private ModConfigurationUI _modConfigurationUIComponent;
    private GameObject _modUIGameObject;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {Id} is loaded!");
        ConfigurationHandler = new ConfigurationHandler(Config);
        
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,$"Pre-max-players change: {NetworkingUtilities.MAX_PLAYERS}! Applying patch...");
        _harmony.PatchAll(typeof(NetworkingUtilitiesGetMaxPlayersPatch));
        var value = typeof(NetworkingUtilities).GetProperty("MAX_PLAYERS").GetValue(null);
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,$"Post-max-players patch: {value}");
        
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.PatchingLogic,$"Plugin {Id} set the Max Players to " + ConfigurationHandler.ConfigMaxPlayers.Value + "!");

        _harmony.PatchAll(typeof(NetworkingUtilitiesHostRoomOptionsPatch));
        
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
        if (ConfigurationHandler.ConfigVoiceFix.Value)
        {
            _harmony.PatchAll(typeof(CharacterVoiceHandlerStartPatch));
            _harmony.PatchAll(typeof(CharacterVoiceHandlerUpdatePatch));
            _harmony.PatchAll(typeof(PlayerHandlerAssignMixerGroupPatch));
        
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.PatchingLogic, "Audio patches enabled!");
        }
        else
        {
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.PatchingLogic, "Audio patches disabled.");
        }
        
        //Disable vanilla marshmallow patch
        _harmony.PatchAll(typeof(SingleItemSpawnerTrySpawnItemsPatch));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"Item Spawner patches successful!");

        //Extra audio sliders patch
        _harmony.PatchAll(typeof(AudioLevelsInitNavigationPatch));


        _harmony.PatchAll(typeof(UIPlayerNamesPatches));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"Nameplate patches successful!");
        
        _harmony.PatchAll(typeof(PeakHandlerPatches));
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.PatchingLogic,"Helicopter patches successful!");
        
        //Mod Configuration Menu
        _modUIGameObject = new GameObject("PEAKUnlimitedUI");
        DontDestroyOnLoad(_modUIGameObject);
        _modConfigurationUIComponent = _modUIGameObject.AddComponent<ModConfigurationUI>();
        _modConfigurationUIComponent.Init(new List<Option>
        {
            Option.Int("Max Players", ConfigurationHandler.ConfigMaxPlayers, 1, 30, isDisabled: () => PhotonNetwork.InRoom),
            Option.Bool("Extra Backpacks", ConfigurationHandler.ConfigExtraBackpacks, isDisabled: () => PhotonNetwork.InRoom && GameHandler.IsOnIsland),
            Option.Bool("Extra Campfire Food", ConfigurationHandler.ConfigExtraMarshmallows, isDisabled: () => PhotonNetwork.InRoom && GameHandler.IsOnIsland),
            Option.Float("Hot Dog Chance", ConfigurationHandler.ConfigHotDogChance, 0f, 1f, 0.005f, isDisabled: () => PhotonNetwork.InRoom && GameHandler.IsOnIsland),
            Option.Bool("Late Join Campfire Food", ConfigurationHandler.ConfigLateMarshmallows, isDisabled: () => PhotonNetwork.InRoom && GameHandler.IsOnIsland),
            Option.Bool("Host Locked Kiosk", ConfigurationHandler.ConfigLockKiosk, isDisabled: () => PhotonNetwork.InRoom && GameHandler.IsOnIsland),
            Option.Bool("Lobby Details", ConfigurationHandler.ConfigLobbyDetails, isDisabled: () => PhotonNetwork.InRoom && GameHandler.IsOnIsland),
            Option.Int("Cheat Campfire Food", ConfigurationHandler.ConfigCheatExtraMarshmallows, 0, 30, isDisabled: () => PhotonNetwork.InRoom && GameHandler.IsOnIsland),
            Option.Int("Cheat Backpacks", ConfigurationHandler.ConfigCheatExtraBackpacks, 0, 10, isDisabled: () => PhotonNetwork.InRoom && GameHandler.IsOnIsland),
            Option.Bool("Experimental: Fix Voice Chat", ConfigurationHandler.ConfigVoiceFix, isDisabled: () => PhotonNetwork.InRoom),
            Option.Bool("Experimental: All Scouts in Helicopter", ConfigurationHandler.ConfigAllScoutsHelicopter, isDisabled: () => PhotonNetwork.InRoom),
            Option.InputAction("Menu Key", ConfigurationHandler.ConfigMenuKey)
        });
        
        #if DEBUG
        Logger.LogInfo("Debug patches!");
        _harmony.PatchAll(typeof(SteamLobbyAPIPlayerIsInLobbyPatch));
        #endif
    }
    
    void OnDestroy()
    {
        _harmony.UnpatchSelf();
        Destroy(_modUIGameObject);
        Logger.LogInfo($"Plugin {Name} unloaded!");
    }
}