using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace PEAKUnlimited.Patches;

public class StartGamePatch
{
    [HarmonyPatch(typeof(AirportCheckInKiosk), nameof(AirportCheckInKiosk.StartGame))]
    [HarmonyPrefix]
    static void Prefix()
    {
        if (PhotonNetwork.IsMasterClient)
            Plugin.HasHostStarted = true;
    }
}