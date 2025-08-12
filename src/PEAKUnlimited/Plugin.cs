using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
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

    public static List<Vector3> GetEvenlySpacedPointsAroundCampfire(int numPoints, float innerRadius, float outerRadius, Vector3 campfirePosition, Segment advanceToSegment)
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
            
            points.Add(SetToGround(new Vector3(x, 0f, z) + campfirePosition));
        }
        
        return points;
    }

    public static List<GameObject> spawnMarshmallows(int number, Vector3 campfirePosition, Segment advanceToSegment)
    {
        List<GameObject> marshmallows = new List<GameObject>();
        Item obj = SingletonAsset<ItemDatabase>.Instance.itemLookup[46];
        Logger.LogInfo((object) ("Plugin PeakUnlimited " + obj.GetName()));
        obj.GetName();
        foreach (Vector3 position in GetEvenlySpacedPointsAroundCampfire(number, 2.5f, 3f, campfirePosition,
                     advanceToSegment))
        {
            marshmallows.Add(Add(obj, position).gameObject);
        }
        Logger.LogInfo((object) ("Plugin PeakUnlimited added with position: " + obj.GetName()));
        return marshmallows;
    }

    private static Vector3 SetToGround(Vector3 vector)
    {
        return HelperFunctions.GetGroundPos(vector, HelperFunctions.LayerType.TerrainMap);
    }

    public static Item Add(Item item, Vector3 position)
    {
        if (!PhotonNetwork.IsConnected)
            return null;
        Logger.LogInfo((object) string.Format("Spawn item: {0} at {1}", (object) item, (object) position));
        return PhotonNetwork.Instantiate("0_Items/" + item.name, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)).GetComponent<Item>();
    }
    
    

    
}
