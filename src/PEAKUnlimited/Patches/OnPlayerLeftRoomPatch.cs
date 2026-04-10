using System;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class OnPlayerLeftRoomPatch
{
    [HarmonyPatch(typeof(PlayerConnectionLog), "OnPlayerLeftRoom")]
    [HarmonyPostfix]
    static void Postfix()
    {
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,"Someone has left the room! Number: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + ConfigurationHandler.ConfigMaxPlayers.Value);
        PlayerCountChangeUtilities.RespawnMarshmallows();
    }
}