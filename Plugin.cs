﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using Photon.Pun;
using Steamworks;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace PeakUnlimited;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static int _newMaxPlayers;
    private static int _cheatExtraMarshmallows;
    private static bool _extraMarshmallows;
    private static ConfigEntry<int> _configMaxPlayers;
    private static ConfigEntry<int> _configCheatExtraMarshmallows;
    private static ConfigEntry<bool> _configExtraMarshmallows;
    private static int _numberOfPlayers = 1;
    private readonly Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        _configMaxPlayers = Config.Bind
        (
            "General",
            "MaxPlayers",
            20,
            "The maximum number of players you want to be able to join your lobby (Including yourself). Warning: untested, higher numbers may be unstable! Range: 1-20"
        );
        _newMaxPlayers = _configMaxPlayers.Value;
        
        _configExtraMarshmallows = Config.Bind
        (
            "General",
            "ExtraMarshmallows",
            true,
            "Controls whether additional marshmallows are spawned for the extra players (Experimental)"
        );
        _extraMarshmallows = _configExtraMarshmallows.Value;
        

        _configCheatExtraMarshmallows = Config.Bind
        (
            "General",
            "Cheat Marshmallows",
            0,
            "(Cheat, disabled by default) This will set the desired amount of marshmallows to the campfires as a cheat, requires ExtraMarshmallows to be enabled."
        );
        _cheatExtraMarshmallows = _configCheatExtraMarshmallows.Value;
        
        if (_newMaxPlayers == 0)
        {
            _newMaxPlayers = 1;
        }
        else if (_newMaxPlayers > 30)
        {
            _newMaxPlayers = 30;
        }

        NetworkConnector.MAX_PLAYERS = _newMaxPlayers;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} set the Max Players to " + NetworkConnector.MAX_PLAYERS + "!");
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is patching!");
        if (_extraMarshmallows) {
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} extra marshmallows are enabled!");
            _harmony.PatchAll(typeof(AwakePatch));
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} left patch enabled!");
            _harmony.PatchAll(typeof(OnPlayerLeftRoomPatch));
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} joined patch enabled!");
            _harmony.PatchAll(typeof(OnPlayerEnteredRoomPatch));
        }
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} has patched!");
    }

    private static List<Vector3> GetEvenlySpacedPointsAroundCampfire(int numPoints, float innerRadius, float outerRadius, Vector3 campfirePosition, Segment advanceToSegment)
    {
        List<Vector3> points = new List<Vector3>();
    
        for (int i = 0; i < numPoints; i++)
        {
            float radius = outerRadius;
            if (i % 2 == 0)
            {
                radius = innerRadius;
            }
            
            float angle = i * Mathf.PI * 2f / numPoints; // Even spacing: 2π / n
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            if (advanceToSegment == Segment.Caldera)
            {
                points.Add(new Vector3(x, -0.5f, z) + campfirePosition);
            } else if (advanceToSegment == Segment.TheKiln)
            {
                points.Add(new Vector3(x, -1f, z) + campfirePosition);
            }
            else
            {
                points.Add(SetToGround(new Vector3(x, 0f, z) + campfirePosition));
            }
        }
        
        return points;
    }
    
    private static void spawnMarshmallows(int number, Vector3 campfirePosition, Segment advanceToSegment)
    {
        Item obj = SingletonAsset<ItemDatabase>.Instance.itemLookup[(ushort) 46];
        Plugin.Logger.LogInfo((object) ("Plugin PeakUnlimited " + obj.GetName()));
        obj.GetName();
        foreach (Vector3 position in Plugin.GetEvenlySpacedPointsAroundCampfire(number, 2.5f, 3f, campfirePosition, advanceToSegment))
            Plugin.Add(obj, position);
        Plugin.Logger.LogInfo((object) ("Plugin PeakUnlimited added with position: " + obj.GetName()));
    }

    private static Vector3 SetToGround(Vector3 vector)
    {
        return HelperFunctions.GetGroundPos(vector, HelperFunctions.LayerType.Default);
    }
    
    private static void Add(Item item, Vector3 position)
    {
        if (!PhotonNetwork.IsConnected)
            return;
        Logger.LogInfo((object) string.Format("Spawn item: {0} at {1}", (object) item, (object) position));
        PhotonNetwork.Instantiate("0_Items/" + item.name, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)).GetComponent<Item>();
    }
    
    public class AwakePatch
    {
        [HarmonyPatch(typeof(Campfire), "Awake")]
        [HarmonyPostfix]
        static void Postfix(Campfire __instance)
        {
            if (__instance.gameObject.transform.parent.gameObject.name.ToLower().Contains("wings"))
            {
                Logger.LogInfo("Skipping plane campfire...");
                return;
            }
            Logger.LogInfo("Marshmellowifying campfire...!");
            int vanillaMaxPlayers = 4;
            int currentPlayers = Plugin._numberOfPlayers;
            int amountOfMarshmallowsToSpawn = currentPlayers - vanillaMaxPlayers;
            if (_cheatExtraMarshmallows != 0)
            {
                Logger.LogInfo("Adding cheatmellows!");
                amountOfMarshmallowsToSpawn = _cheatExtraMarshmallows - vanillaMaxPlayers;
                if (currentPlayers < vanillaMaxPlayers)
                {
                    amountOfMarshmallowsToSpawn = _cheatExtraMarshmallows - currentPlayers;
                }
            }
            
            Plugin.Logger.LogInfo("Start of campfire patch!");
            if (PhotonNetwork.IsMasterClient && (currentPlayers > vanillaMaxPlayers || _cheatExtraMarshmallows != 0))
            {
                Logger.LogInfo("More than 4 players, preparing to marshmallowify! Number: " + currentPlayers);
                Vector3 position = __instance.gameObject.transform.position;
                Logger.LogInfo("Spawning " + amountOfMarshmallowsToSpawn + " marshmallows!");
                Plugin.spawnMarshmallows(amountOfMarshmallowsToSpawn, position, __instance.advanceToSegment);
                Logger.LogInfo("End of campfire patch!");
            }
            else
            {
                Logger.LogInfo("Not enough players for campfire patch or not host!");
            }
        }
    }
    
    public class OnPlayerEnteredRoomPatch
    {
        [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerEnteredRoom")]
        [HarmonyPostfix]
        static void Postfix(PlayerConnectionLog __instance)
        {
            _numberOfPlayers++;
            Logger.LogInfo("Someone has joined the room! Number: " + _numberOfPlayers + "/" + NetworkConnector.MAX_PLAYERS);
        }
    }
    
    public class OnPlayerLeftRoomPatch
    {
        [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerLeftRoom")]
        [HarmonyPostfix]
        static void Postfix(PlayerConnectionLog __instance)
        {
            _numberOfPlayers--;
            if (_numberOfPlayers < 0)
            {
                _numberOfPlayers = 0;
            }
            Logger.LogInfo("Someone has left the room! Number: " + _numberOfPlayers + "/" + NetworkConnector.MAX_PLAYERS);
        }
    }
}
