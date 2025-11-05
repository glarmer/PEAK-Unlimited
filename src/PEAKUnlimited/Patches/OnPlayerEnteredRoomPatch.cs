using System;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class OnPlayerEnteredRoomPatch
{
    [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerEnteredRoom")]
    [HarmonyPostfix]
    static void Postfix()
    {
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,"Someone has joined the room! Number: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + ConfigurationHandler.ConfigMaxPlayers.Value);
        //Add a marshmallow at each campfire for the new player
        if (!Plugin.ConfigurationHandler.IsLateMarshmallowsEnabled)
            return;
        if (Plugin.CampfireList == null || Plugin.CampfireList.Count == 0) return;
        Segment segment;
        try
        {
            segment = MapHandler.Instance.GetCurrentSegment();
        }
        catch (Exception e)
        {
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Error, DebugLogType.NetworkingLogic,"Error getting current segment: " + e);
            segment = Segment.Beach;
        }
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