using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace PEAKUnlimited.Patches;

public class OnPlayerLeftRoomPatch : MonoBehaviour
{
    [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerLeftRoom")]
    [HarmonyPostfix]
    static void Postfix(PlayerConnectionLog __instance)
    {
        Plugin.Logger.LogInfo("Someone has left the room! Number: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + NetworkConnector.MAX_PLAYERS);
        if (!Plugin.ConfigurationHandler.IsLateMarshmallowsEnabled)
            return;
        if (Plugin.IsAfterAwake && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= Plugin.VanillaMaxPlayers && Plugin.ConfigurationHandler.CheatMarshmallows == 0)
        {
            Plugin.Logger.LogInfo("Removing a marshmallow!");
            foreach (Campfire campfire in Plugin.CampfireList)
            {
                Plugin.Logger.LogInfo("Removing a marshmallow! " +  Plugin.Marshmallows[campfire].Count);
                Plugin.Logger.LogInfo("Removing a marshmallow! " +  Plugin.Marshmallows[campfire][0].gameObject.name);
                Destroy(Plugin.Marshmallows[campfire][0]);
                Plugin.Marshmallows[campfire].RemoveAt(0);
                Plugin.Logger.LogInfo("Removing a marshmallow! " +  Plugin.Marshmallows[campfire].Count);
                Plugin.Logger.LogInfo("Removing a marshmallow! " +  Plugin.Marshmallows[campfire][0].gameObject.name);
            }
        }
    }
}