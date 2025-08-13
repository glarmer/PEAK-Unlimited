using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace PEAKUnlimited.Patches;

public class StartGamePatch
{
    [HarmonyPatch(typeof(AirportCheckInKiosk), "StartGame")]
    [HarmonyPrefix]
    static void Prefix()
    {
        if (PhotonNetwork.IsMasterClient)
            Plugin.HasHostStarted = true;
    }
}