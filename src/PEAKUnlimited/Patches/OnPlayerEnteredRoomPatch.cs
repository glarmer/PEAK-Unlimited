using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace PEAKUnlimited.Patches;

public class OnPlayerEnteredRoomPatch
{
    [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerEnteredRoom")]
    [HarmonyPostfix]
    static void Postfix(PlayerConnectionLog __instance)
    {
        Plugin.NumberOfPlayers++;
        Plugin.Logger.LogInfo("Someone has joined the room! Number: " + Plugin.NumberOfPlayers + "/" + NetworkConnector.MAX_PLAYERS);
        //Add a marshmallow at each campfire for the new player
        if (!Plugin.ConfigurationHandler.IsLateMarshmallowsEnabled)
            return;
        if (Plugin.IsAfterAwake && PhotonNetwork.IsMasterClient && Plugin.NumberOfPlayers > Plugin.VanillaMaxPlayers && Plugin.ConfigurationHandler.CheatMarshmallows == 0)
        {
            foreach (Campfire campfire in Plugin.CampfireList)
            {
                Vector3 position = campfire.gameObject.transform.position;
                Plugin.Logger.LogInfo("Spawning a marshmallow!");
                Plugin.Marshmallows[campfire].Add(Utility.SpawnMarshmallows(1, position, campfire.gameObject.transform.eulerAngles, campfire.advanceToSegment)[0]);
            }
        }
    }
}