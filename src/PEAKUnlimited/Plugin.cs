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

// Here are some basic resources on code style and naming conventions to help
// you in your first CSharp plugin!
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names
// https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces

// This BepInAutoPlugin attribute comes from the Hamunii.BepInEx.AutoPlugin
// NuGet package, and it will generate the BepInPlugin attribute for you!
// For more info, see https://github.com/Hamunii/BepInEx.AutoPlugin
[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static int _newMaxPlayers;
    private static int _cheatExtraMarshmallows;
    private static int _cheatExtraBackpacks;
    private static bool _extraMarshmallows;
    private static ConfigEntry<int> _configMaxPlayers;
    private static ConfigEntry<int> _configCheatExtraMarshmallows;
    private static ConfigEntry<int> _configCheatExtraBackpacks;
    private static ConfigEntry<bool> _configExtraBackpacks;
    private static ConfigEntry<bool> _configExtraMarshmallows;
    private static ConfigEntry<bool> _configLateMarshmallows;
    private static int _numberOfPlayers = 1;
    private readonly Harmony _harmony = new Harmony(Id);
    private static List<Campfire> campfireList = new List<Campfire>();
    private static bool isAfterAwake = false;
    private static int vanillaMaxPlayers = 4;
    private static Dictionary<Campfire, List<GameObject>> marshmallows = new Dictionary<Campfire, List<GameObject>>();

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {Id} is loaded!");

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
            "Controls whether additional marshmallows are spawned for the extra players"
        );
        _extraMarshmallows = _configExtraMarshmallows.Value;
        
        _configExtraBackpacks = Config.Bind
        (
            "General",
            "ExtraBackpacks",
            true,
            "Controls whether additional backpacks have a chance to be spawned for extra players"
        );
        
        _configLateMarshmallows = Config.Bind
        (
            "General",
            "LateJoinMarshmallows",
            false,
            "Controls whether additional marshmallows are spawned for players who join late (mid run), and removed for those who leave early (Experimental + Untested)"
        );

        _configCheatExtraMarshmallows = Config.Bind
        (
            "General",
            "Cheat Marshmallows",
            0,
            "(Cheat, disabled by default) This will set the desired amount of marshmallows to the campfires as a cheat, requires ExtraMarshmallows to be enabled. Capped at 30."
        );
        _cheatExtraMarshmallows = _configCheatExtraMarshmallows.Value;
        if (_cheatExtraMarshmallows > 30)
        {
            _cheatExtraMarshmallows = 30;
        }
        
        _configCheatExtraBackpacks = Config.Bind
        (
            "General",
            "Cheat Backpacks",
            0,
            "(Cheat, disabled by default) Sets how many backpacks will spawn as a cheat, requires ExtraBackpacks to also be enabled. Capped at 10."
        );
        _cheatExtraBackpacks = _configCheatExtraBackpacks.Value;

        if (_cheatExtraBackpacks > 10)
        {
            _cheatExtraBackpacks = 10;
        }
        
        if (_newMaxPlayers == 0)
        {
            _newMaxPlayers = 1;
        }
        else if (_newMaxPlayers > 30)
        {
            _newMaxPlayers = 30;
        }

        NetworkConnector.MAX_PLAYERS = _newMaxPlayers;
        Logger.LogInfo($"Plugin {Id} set the Max Players to " + NetworkConnector.MAX_PLAYERS + "!");
        Logger.LogInfo($"Plugin {Id} is patching!");
        if (_extraMarshmallows) {
            Logger.LogInfo($"Plugin {Id} extra marshmallows are enabled!");
            _harmony.PatchAll(typeof(AwakePatch));
            Logger.LogInfo($"Plugin {Id} late marshmallows are enabled!");
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
            
            points.Add(SetToGround(new Vector3(x, 0f, z) + campfirePosition));
        }
        
        return points;
    }
    
    private static List<GameObject> spawnMarshmallows(int number, Vector3 campfirePosition, Segment advanceToSegment)
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
    
    private static Item Add(Item item, Vector3 position)
    {
        if (!PhotonNetwork.IsConnected)
            return null;
        Logger.LogInfo((object) string.Format("Spawn item: {0} at {1}", (object) item, (object) position));
        return PhotonNetwork.Instantiate("0_Items/" + item.name, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)).GetComponent<Item>();
    }
    
    public class AwakePatch
    {
        [HarmonyPatch(typeof(Campfire), "Awake")]
        [HarmonyPostfix]
        static void Postfix(Campfire __instance)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            
            //Backpack addition
            if (_configExtraBackpacks.Value)
            {
                Logger.LogInfo("Backpackification enabled and starting!");
                Item obj = SingletonAsset<ItemDatabase>.Instance.itemLookup[6];
                int numberOfExtraPlayers = _numberOfPlayers - vanillaMaxPlayers;
                int number = 0;
                if (numberOfExtraPlayers > 0) {
                    double backpackNumber = numberOfExtraPlayers * 0.25;
                    
                    if (backpackNumber % 4 == 0)
                    {
                        number = (int)backpackNumber;
                    }
                    else
                    {
                        number = (int)backpackNumber;
                        if (Random.Range(0f, 1f) <= backpackNumber - number)
                        {
                            number++;
                        }
                    }
                }
                if (_cheatExtraBackpacks  > 0 && _cheatExtraBackpacks  <= 10)
                {
                    number = _cheatExtraBackpacks  - 1; //Minus one as there is already a backpack present
                }

                if (number > 0)
                {
                    foreach (Vector3 position in GetEvenlySpacedPointsAroundCampfire(number, 3.3f, 3.7f,
                                 __instance.gameObject.transform.position,
                                 __instance.advanceToSegment))
                    {
                        Vector3 finalPosition = position;
                        if (__instance.gameObject.transform.parent.gameObject.name.ToLower().Contains("wings"))
                        {
                            finalPosition =
                                position + new Vector3(0, 10f, 0f); // stops backpacks on the beach spawning underground...
                        }
                        Add(obj, finalPosition).transform.parent = __instance.gameObject.transform;
                    }
                }
                else
                {
                    Logger.LogInfo("Not enough players to add additional backpacks, use the Cheat Backpack configuration setting if you want to override this!");
                }
            }
            //End of backpack addition
            
            //Marshmallow addition
            if (__instance.gameObject.transform.parent.gameObject.name.ToLower().Contains("wings"))
            {
                return;
            }
            campfireList.Add(__instance);
            Logger.LogInfo("Marshmellowifying campfire...!");
            int amountOfMarshmallowsToSpawn = _numberOfPlayers - vanillaMaxPlayers;
            if (_cheatExtraMarshmallows != 0)
            {
                Logger.LogInfo("Adding cheatmellows!");
                amountOfMarshmallowsToSpawn = _cheatExtraMarshmallows - vanillaMaxPlayers;
                if (_numberOfPlayers < vanillaMaxPlayers)
                {
                    amountOfMarshmallowsToSpawn = _cheatExtraMarshmallows - _numberOfPlayers;
                }
            }
            
            Plugin.Logger.LogInfo("Start of campfire patch!");
            if (PhotonNetwork.IsMasterClient && (_numberOfPlayers > vanillaMaxPlayers || _cheatExtraMarshmallows != 0))
            {
                Logger.LogInfo("More than 4 players, preparing to marshmallowify! Number: " + _numberOfPlayers);
                Vector3 position = __instance.gameObject.transform.position;
                marshmallows.Add(__instance, spawnMarshmallows(amountOfMarshmallowsToSpawn, position, __instance.advanceToSegment));
            }
            else
            {
                Logger.LogInfo("Not enough players for extra marshmallows, use the extra marshmallows cheat configuration option to override this!");
            }
            isAfterAwake = true;
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
            //Add a marshmallow at each campfire for the new player
            if (!_configLateMarshmallows.Value)
                return;
            if (isAfterAwake && PhotonNetwork.IsMasterClient && _numberOfPlayers > vanillaMaxPlayers && _cheatExtraMarshmallows == 0)
            {
                foreach (Campfire campfire in campfireList)
                {
                    Vector3 position = campfire.gameObject.transform.position;
                    Logger.LogInfo("Spawning a marshmallow!");
                    marshmallows[campfire].Add(spawnMarshmallows(1, position, campfire.advanceToSegment)[0]);
                }
            }
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
            if (!_configLateMarshmallows.Value)
                return;
            if (isAfterAwake && PhotonNetwork.IsMasterClient && _numberOfPlayers >= vanillaMaxPlayers && _cheatExtraMarshmallows == 0)
            {
                Logger.LogInfo("Removing a marshmallow!");
                foreach (Campfire campfire in campfireList)
                {
                    Logger.LogInfo("Removing a marshmallow! " +  marshmallows[campfire].Count);
                    Logger.LogInfo("Removing a marshmallow! " +  marshmallows[campfire][0].gameObject.name);
                    Destroy(marshmallows[campfire][0]);
                    marshmallows[campfire].RemoveAt(0);
                    Logger.LogInfo("Removing a marshmallow! " +  marshmallows[campfire].Count);
                    Logger.LogInfo("Removing a marshmallow! " +  marshmallows[campfire][0].gameObject.name);
                }
            }
        }
    }

    public class WaitingForPlayersUIPatch
    {
        [HarmonyPatch(typeof(WaitingForPlayersUI), "Update")]
        [HarmonyPrefix]
        static void Prefix(WaitingForPlayersUI __instance)
        {
            if (__instance.scoutImages.Length >= Character.AllCharacters.Count)
                return;
            var newScoutImages = new Image[Character.AllCharacters.Count];
            Image original = __instance.scoutImages[0];
            for (int i = 0; i < Character.AllCharacters.Count; i++)
            {
                if (i < vanillaMaxPlayers)
                {
                    newScoutImages[i] = __instance.scoutImages[i];
                }
                else
                {
                    newScoutImages[i] = Instantiate(original, original.transform.parent);
                }
            }
            __instance.scoutImages = newScoutImages;
        }
    }
    
    public class EndScreenNextPatch
    {
        [HarmonyPatch(typeof (EndScreen), "Next")]
        [HarmonyPostfix]
        private static void PostFix()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Singleton<PeakHandler>.Instance.EndScreenComplete();
            }
        }
    }

    public class EndScreenStartPatch
    {
        [HarmonyPatch(typeof(EndScreen), "Start")]
        [HarmonyPrefix]
        static void Prefix(EndScreen __instance)
        {
            if (Character.AllCharacters.Count <= vanillaMaxPlayers)
                return;

            var newScoutWindows = new EndScreenScoutWindow[Character.AllCharacters.Count];
            var newScouts = new Image[Character.AllCharacters.Count];
            var newScoutsAtPeak = new Image[Character.AllCharacters.Count];
            var newOldPip = new Image[Character.AllCharacters.Count];
            var newScoutLines = new Transform[Character.AllCharacters.Count];

            for (int i = 0; i < Character.AllCharacters.Count; i++)
            {
                //Don't do anything to the original ones
                bool withinExisting = i < __instance.scouts.Length;
                if (!withinExisting)
                {
                    if ((UnityEngine.Object)__instance.scoutWindows[0] == null)
                    {
                        newScoutWindows[i] = null;
                    }
                    else
                    {
                        newScoutWindows[i] = Instantiate(
                            __instance.scoutWindows[0],
                            __instance.scoutWindows[0].transform.parent
                        );
                    }
                    
                    if ((UnityEngine.Object)__instance.scouts[0] == null)
                    {
                        newScouts[i] = null;
                    }
                    else
                    {
                        newScouts[i] = Instantiate(
                            __instance.scouts[0],
                            __instance.scouts[0].transform.parent
                        );
                    }
                    
                    if ((UnityEngine.Object)__instance.scoutsAtPeak[0] == null)
                    {
                        newScoutsAtPeak[i] = null;
                    }
                    else
                    {
                        newScoutsAtPeak[i] = Instantiate(
                            __instance.scoutsAtPeak[0],
                            __instance.scoutsAtPeak[0].transform.parent
                        );
                    }
                    
                    if ((UnityEngine.Object)__instance.oldPip[0] == null)
                    {
                        newOldPip[i] = null;
                    }
                    else
                    {
                        newOldPip[i] = Instantiate(
                            __instance.oldPip[0],
                            __instance.oldPip[0].transform.parent
                        );
                    }
                    
                    if ((UnityEngine.Object)__instance.scoutLines[0] == null)
                    {
                        newScoutLines[i] = null;
                    }
                    else
                    {
                        newScoutLines[i] = Instantiate(
                            __instance.scoutLines[0],
                            __instance.scoutLines[0].transform.parent
                        );
                    }}
                else
                {
                    newScoutWindows[i] = __instance.scoutWindows[i];
                    newScouts[i] = __instance.scouts[i];
                    newScoutsAtPeak[i] = __instance.scoutsAtPeak[i];
                    newOldPip[i] = __instance.oldPip[i];
                    newScoutLines[i] = __instance.scoutLines[i];
                }
            }

            //Reassign arrays with new ones
            __instance.scoutWindows = newScoutWindows;
            __instance.scouts = newScouts;
            __instance.scoutsAtPeak = newScoutsAtPeak;
            __instance.oldPip = newOldPip;
            __instance.scoutLines = newScoutLines;
        }
    }

    public class EndSequenceRoutinePatch
    {
        [HarmonyPatch(typeof(EndScreen), "EndSequenceRoutine")]
        [HarmonyPostfix]
        static void Postfix(EndScreen __instance)
        {
            for (int i = 4; i < Character.AllCharacters.Count; i++)
            {
                Logger.LogInfo("Deactivating an end screen");
                //Don't display the extra names since it blocks the chart
                Destroy(__instance.scoutWindows[i].gameObject);
                Logger.LogInfo("Deleted an end screen");
            }
        }
    }
}
