using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace PEAKUnlimited;

public class OnPlayerLeftRoomPatch : MonoBehaviour
{
    [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerLeftRoom")]
    [HarmonyPostfix]
    static void Postfix(PlayerConnectionLog __instance)
    {
        Plugin._numberOfPlayers--;
        if (Plugin._numberOfPlayers < 0)
        {
            Plugin._numberOfPlayers = 0;
        }
        Plugin.Logger.LogInfo("Someone has left the room! Number: " + Plugin._numberOfPlayers + "/" + NetworkConnector.MAX_PLAYERS);
        if (!Plugin.config.IsLateMarshmallowsEnabled)
            return;
        if (Plugin.isAfterAwake && PhotonNetwork.IsMasterClient && Plugin._numberOfPlayers >= Plugin.VANILLA_MAX_PLAYERS && Plugin.config.CheatMarshmallows == 0)
        {
            Plugin.Logger.LogInfo("Removing a marshmallow!");
            foreach (Campfire campfire in Plugin.campfireList)
            {
                Plugin.Logger.LogInfo("Removing a marshmallow! " +  Plugin.marshmallows[campfire].Count);
                Plugin.Logger.LogInfo("Removing a marshmallow! " +  Plugin.marshmallows[campfire][0].gameObject.name);
                Destroy(Plugin.marshmallows[campfire][0]);
                Plugin.marshmallows[campfire].RemoveAt(0);
                Plugin.Logger.LogInfo("Removing a marshmallow! " +  Plugin.marshmallows[campfire].Count);
                Plugin.Logger.LogInfo("Removing a marshmallow! " +  Plugin.marshmallows[campfire][0].gameObject.name);
            }
        }
    }
}