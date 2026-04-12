using System;
using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class OnPlayerEnteredRoomPatch
{
    [HarmonyPatch(typeof(PlayerConnectionLog), nameof(PlayerConnectionLog.OnPlayerEnteredRoom))]
    [HarmonyPostfix]
    static void Postfix()
    {
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,"Someone has joined the room! Number: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + ConfigurationHandler.ConfigMaxPlayers.Value);
        PlayerCountChangeUtilities.RespawnMarshmallows();
    }
}