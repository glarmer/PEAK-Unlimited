using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class OnPlayerLeftRoomPatch : MonoBehaviour
{
    [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerLeftRoom")]
    [HarmonyPostfix]
    static void Postfix()
    {
        Plugin.Logger.LogInfo("Someone has left the room! Number: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + NetworkConnector.MAX_PLAYERS);
        if (!Plugin.ConfigurationHandler.IsLateMarshmallowsEnabled)
            return;
        if (Plugin.CampfireList == null || Plugin.CampfireList.Count == 0) return;
        Segment segment = Singleton<MapHandler>.Instance.GetCurrentSegment();
        if (Plugin.IsAfterAwake && PhotonNetwork.IsMasterClient && Plugin.ConfigurationHandler.CheatMarshmallows == 0)
        {
            //Delete existing marshmallows
            foreach (var campfireMarshmallows in Plugin.Marshmallows)
            {
                if (campfireMarshmallows.Key.advanceToSegment > segment)
                {
                    foreach (var marshmallow in campfireMarshmallows.Value)
                    {
                        PhotonNetwork.Destroy(marshmallow);
                    }
                }
            }
            Plugin.Marshmallows.Clear();
            foreach (Campfire campfire in Plugin.CampfireList)
            {
                if (campfire.advanceToSegment > segment)
                {
                    //respawn this campfires marshmallows
                    Plugin.Marshmallows.Add(campfire, Utility.SpawnMarshmallows(PhotonNetwork.CurrentRoom.PlayerCount, campfire.transform.position, campfire.gameObject.transform.eulerAngles, campfire.advanceToSegment));
                }
            }
        }
    }
}