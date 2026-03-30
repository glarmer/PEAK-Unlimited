using System;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Model.GameInfo;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;
using Random = UnityEngine.Random;

namespace PEAKUnlimited.Patches;

public class CampfireAwakePatch
{
    [HarmonyPatch(typeof(Campfire), "Awake")]
    [HarmonyPostfix]
    static void Postfix(Campfire __instance)
    {
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.CampfireLogic,$"Campfire Awake Patch! Number of known campfires: {Plugin.CampfireList.Count}");
        if (PhotonNetwork.CurrentRoom == null)
        {
            __instance.StartCoroutine(WaitForRoomAndRun(__instance));
            return;
        }
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.CampfireLogic, new CampfireInfo().GetInfoMessage(__instance));
        if (!PhotonNetwork.IsMasterClient)
        {
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.CampfireLogic,$"Campfire Awake Patch! Not the master client, returning!");
            return;
        }
        if (__instance.nameOverride == "NAME_PORTABLE STOVE")
            return;
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.CampfireLogic, "Testing 123");
        if (Plugin.ConfigurationHandler.IsExtraBackpacksEnabled)
        {
            AddBackpacks(__instance);
        }
        AddMarshmallows(__instance);

        Plugin.IsAfterAwake = true;
    }

    private static void AddMarshmallows(Campfire __instance)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (__instance.gameObject.transform.parent.gameObject.name.ToLower().Contains("wings"))
            {
                return;
            }
            Plugin.CampfireList.Add(__instance);
            
            int amountOfMarshmallowsToSpawn = Math.Min(4, PhotonNetwork.CurrentRoom.PlayerCount);
            if (Plugin.ConfigurationHandler.IsExtraMarshmallowsEnabled)
            {
                UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.MarshmallowLogic,"Marshmallowification enabled and starting!");
                amountOfMarshmallowsToSpawn = PhotonNetwork.CurrentRoom.PlayerCount;
            }
            if (Plugin.ConfigurationHandler.CheatMarshmallows != 0)
            {
                amountOfMarshmallowsToSpawn = Plugin.ConfigurationHandler.CheatMarshmallows;
                UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.MarshmallowLogic,"Cheatmallows enabled!");
            }
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.MarshmallowLogic,$"Will spawn {amountOfMarshmallowsToSpawn} marshmallows for {PhotonNetwork.CurrentRoom.PlayerCount} people!");
            Vector3 position = __instance.gameObject.transform.position;
            Vector3 eulerAngles = __instance.gameObject.transform.eulerAngles;
            Plugin.Marshmallows.Add(__instance, Utility.SpawnMarshmallows(amountOfMarshmallowsToSpawn, position, eulerAngles, __instance.advanceToSegment));
        }
    }

    private static void AddBackpacks(Campfire __instance)
    {
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.BackpackLogic,"Backpackification enabled and starting!");
        Item obj = SingletonAsset<ItemDatabase>.Instance.itemLookup[6];
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.BackpackLogic,"Backpackification enabled and starting! 1");
        int numberOfExtraPlayers;
        try
        {
            numberOfExtraPlayers = PhotonNetwork.CurrentRoom.PlayerCount - Plugin.VanillaMaxPlayers;
        }
        catch
        {
            numberOfExtraPlayers = 0;
        }
        
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.BackpackLogic,"Backpackification enabled and starting! 2");
        int number = 0;
        if (numberOfExtraPlayers > 0)
        {
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
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.BackpackLogic,"Backpackification enabled and starting! 3");
        if (Plugin.ConfigurationHandler.CheatBackpacks != 0)
        {
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.BackpackLogic,"Cheat Backpacks enabled = " + Plugin.ConfigurationHandler.CheatBackpacks);
            number = Plugin.ConfigurationHandler.CheatBackpacks - 1; //Minus one as there is already a backpack present
        }

        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.BackpackLogic,"Backpacks enabled = " + number);
        if (number > 0)
        {
            foreach (Vector3 position in Utility.GetEvenlySpacedPointsAroundCampfire(number, 3.3f, 3.7f,
                         __instance.gameObject.transform.position, __instance.gameObject.transform.eulerAngles,
                         __instance.advanceToSegment))
            {
                Vector3 finalPosition = position;
                if (__instance.gameObject.transform.parent.gameObject.name.ToLower().Contains("wings"))
                {
                    finalPosition =
                        position + new Vector3(0, 10f, 0f); // stops backpacks on the beach spawning underground...
                }

                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                Utility.Add(obj, finalPosition, rotation).transform.parent = __instance.gameObject.transform;
            }
        }
        else
        {
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info,DebugLogType.BackpackLogic,
                "Not enough players to add additional backpacks, use the Cheat Backpack configuration setting if you want to override this!");
        }
    }
    
    private static System.Collections.IEnumerator WaitForRoomAndRun(Campfire instance)
    {
        while (PhotonNetwork.CurrentRoom == null)
        {
            yield return null;
        }
        
        if (!PhotonNetwork.IsMasterClient)
            yield break;

        if (instance.nameOverride == "NAME_PORTABLE STOVE")
            yield break;

        if (Plugin.ConfigurationHandler.IsExtraBackpacksEnabled)
        {
            AddBackpacks(instance);
        }

        AddMarshmallows(instance);

        Plugin.IsAfterAwake = true;
    }
}