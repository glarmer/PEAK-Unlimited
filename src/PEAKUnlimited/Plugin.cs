using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Patches;
using Photon.Pun;
using UnityEngine;
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

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {Id} is loaded!");
        config = new ConfigurationHandler();
        NetworkConnector.MAX_PLAYERS = config.MaxPlayers;
        Logger.LogInfo($"Plugin {Id} set the Max Players to " + NetworkConnector.MAX_PLAYERS + "!");
        Logger.LogInfo($"Plugin {Id} is patching!");
        if (config.IsExtraMarshmallowsEnabled) {
            Logger.LogInfo($"Plugin {Id} extra marshmallows are enabled!");
            _harmony.PatchAll(typeof(CampfireAwakePatch));
            Logger.LogInfo($"Plugin {Id} left patch enabled!");
            _harmony.PatchAll(typeof(OnPlayerLeftRoomPatch));
            Logger.LogInfo($"Plugin {Id} joined patch enabled!");
            _harmony.PatchAll(typeof(OnPlayerEnteredRoomPatch));
        }
        Logger.LogInfo($"Plugin {Id} patching end screen!");
        _harmony.PatchAll(typeof(EndSequenceRoutinePatch));
        _harmony.PatchAll(typeof(WaitingForPlayersUIPatch));
        _harmony.PatchAll(typeof(EndScreenStartPatch));
        _harmony.PatchAll(typeof(EndScreenNextPatch));
        Logger.LogInfo($"Plugin {Id} has patched!");
    }
}
