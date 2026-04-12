using System;
using BepInEx.Logging;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;

namespace PEAKUnlimited.Util;

public static class PlayerCountChangeUtilities
{
    public static void ForgetCampfiresAndMarshmallows()
    {
        if (Plugin.Marshmallows != null)
        {
            Plugin.Marshmallows.Clear();
        }
        if (Plugin.CampfireList != null)
        {
            Plugin.CampfireList.Clear();
        }
    }
    
    public static void RespawnMarshmallows()
    {
        if (!GameHandler.IsOnIslandAndInitialized) 
            return;
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Error, DebugLogType.MarshmallowLogic,"We are on the island.");
        if (!Plugin.ConfigurationHandler.IsLateMarshmallowsEnabled)
            return;
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Error, DebugLogType.MarshmallowLogic,"Late marshmallows are enabled.");
        if (Plugin.CampfireList == null || Plugin.CampfireList.Count == 0) 
            return;
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Error, DebugLogType.MarshmallowLogic,"The campfire list exists and is not empty.");
        Segment segment;
        try
        {
            segment = MapHandler.Instance.GetCurrentSegment();
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Error, DebugLogType.MarshmallowLogic,"Segment found: " + segment);
        }
        catch (Exception e)
        {
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Error, DebugLogType.MarshmallowLogic,"Error getting current segment, will refresh all campfires: " + e);
            segment = Segment.Beach;
        }
        if (PhotonNetwork.IsMasterClient && Plugin.ConfigurationHandler.CheatMarshmallows == 0)
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
                    //respawn these campfires marshmallows
                    Utility.SpawnMarshmallows(PhotonNetwork.CurrentRoom.PlayerCount, campfire);
                }
            }
        }
    }
}