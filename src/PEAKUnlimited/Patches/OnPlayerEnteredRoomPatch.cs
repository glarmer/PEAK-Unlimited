using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace PEAKUnlimited;

public class OnPlayerEnteredRoomPatch
{
    [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerEnteredRoom")]
    [HarmonyPostfix]
    static void Postfix(PlayerConnectionLog __instance)
    {
        Plugin._numberOfPlayers++;
        Plugin.Logger.LogInfo("Someone has joined the room! Number: " + Plugin._numberOfPlayers + "/" + NetworkConnector.MAX_PLAYERS);
        //Add a marshmallow at each campfire for the new player
        if (!Plugin.config.IsLateMarshmallowsEnabled)
            return;
        if (Plugin.isAfterAwake && PhotonNetwork.IsMasterClient && Plugin._numberOfPlayers > Plugin.VANILLA_MAX_PLAYERS && Plugin.config.CheatMarshmallows == 0)
        {
            foreach (Campfire campfire in Plugin.campfireList)
            {
                Vector3 position = campfire.gameObject.transform.position;
                Plugin.Logger.LogInfo("Spawning a marshmallow!");
                Plugin.marshmallows[campfire].Add(Utility.SpawnMarshmallows(1, position, campfire.advanceToSegment)[0]);
            }
        }
    }
}