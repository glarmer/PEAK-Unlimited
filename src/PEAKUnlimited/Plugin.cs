using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Patches;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zorro.Core;

namespace PEAKUnlimited;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    public static int _numberOfPlayers = 1;
    public static ConfigurationHandler config;
    private readonly Harmony _harmony = new Harmony(Id);
    public static List<Campfire> campfireList = new List<Campfire>();
    public static bool isAfterAwake = false;
    public static int VANILLA_MAX_PLAYERS = 4;
    public static Dictionary<Campfire, List<GameObject>> marshmallows = new Dictionary<Campfire, List<GameObject>>();
    public static bool hasHostStarted = false;

    private ModConfigurationUI _ui;
    
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {Id} is loaded!");
        config = new ConfigurationHandler();
        NetworkConnector.MAX_PLAYERS = config.MaxPlayers;
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
            Option.Int("Max Players (Requires Restart)",   config._configMaxPlayers, 1, 30, 1),
            Option.Bool("Extra Backpacks",   config._configExtraBackpacks),
            Option.Bool("Extra Marshmallows",    config._configExtraMarshmallows),
            Option.Bool("Host Locked Kiosk",    config._lockKiosk),
            Option.Bool("Lobby Details",    config._configLobbyDetails),
            Option.Int("Cheat Marshmallows",   config._configCheatExtraMarshmallows, 0, 30, 1),
            Option.Int("Cheat Backpacks",   config._configCheatExtraBackpacks, 0, 10, 1),
        });
    }
}
