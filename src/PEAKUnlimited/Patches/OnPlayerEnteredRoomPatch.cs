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
        Plugin.Logger.LogInfo("Someone has joined the room! Number: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + NetworkConnector.MAX_PLAYERS);
        //Add a marshmallow at each campfire for the new player
        if (!Plugin.ConfigurationHandler.IsLateMarshmallowsEnabled)
            return;
        if (Plugin.IsAfterAwake && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > Plugin.VanillaMaxPlayers && Plugin.ConfigurationHandler.CheatMarshmallows == 0)
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