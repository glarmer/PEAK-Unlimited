using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Patches;
using UnityEngine;

namespace PEAKUnlimited;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    public static int NumberOfPlayers = 1;
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
        ConfigurationHandler = new ConfigurationHandler();
        NetworkConnector.MAX_PLAYERS = ConfigurationHandler.MaxPlayers;
        Logger.LogInfo($"Plugin {Id} set the Max Players to " + NetworkConnector.MAX_PLAYERS + "!");

        //Extra marshmallow and backpack patches
        _harmony.PatchAll(typeof(CampfireAwakePatch));
        _harmony.PatchAll(typeof(OnPlayerLeftRoomPatch));
        _harmony.PatchAll(typeof(OnPlayerEnteredRoomPatch));
        Logger.LogInfo("Marshmallow patches successful!");

        //Lock Airport Kiosk Patches
        _harmony.PatchAll(typeof(StartGamePatch));
        _harmony.PatchAll(typeof(LoadIslandMasterPatch));
        Logger.LogInfo("Kiosk patches successful!");

        //End screen patches
        _harmony.PatchAll(typeof(EndSequenceRoutinePatch));
        _harmony.PatchAll(typeof(WaitingForPlayersUIPatch));
        _harmony.PatchAll(typeof(EndScreenStartPatch));
        _harmony.PatchAll(typeof(EndScreenNextPatch));
        Logger.LogInfo("End screen patches successful!");

        //In-game message patches
        _harmony.PatchAll(typeof(PlayerConnectionLogAwakePatch));
        _harmony.PatchAll(typeof(PlayClickedPatch));
        _harmony.PatchAll(typeof(LeaveLobbyPatch));


        //Mod Configuration Menu
        var go = new GameObject("PEAKUnlimitedUI");
        DontDestroyOnLoad(go);
        _ui = go.AddComponent<ModConfigurationUI>();
        _ui.Init(new List<Option>
        {
            Option.Int("Max Players", ConfigurationHandler.ConfigMaxPlayers, 1, 30),
            Option.Bool("Extra Backpacks", ConfigurationHandler.ConfigExtraBackpacks),
            Option.Bool("Extra Marshmallows", ConfigurationHandler.ConfigExtraMarshmallows),
            Option.Bool("Host Locked Kiosk", ConfigurationHandler.ConfigLockKiosk),
            Option.Bool("Lobby Details", ConfigurationHandler.ConfigLobbyDetails),
            Option.Int("Cheat Marshmallows", ConfigurationHandler.ConfigCheatExtraMarshmallows, 0, 30),
            Option.Int("Cheat Backpacks", ConfigurationHandler.ConfigCheatExtraBackpacks, 0, 10)
        });
    }
}