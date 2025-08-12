using HarmonyLib;
using Photon.Pun;

namespace PEAKUnlimited.Patches;

public class LoadIslandMasterPatch
{
    [HarmonyPatch(typeof(AirportCheckInKiosk), "LoadIslandMaster")]
    [HarmonyPrefix]
    static bool Prefix()
    {
        Plugin.Logger.LogInfo("Load Island Master Patch running");
        if (!PhotonNetwork.IsMasterClient || !Plugin.hasHostStarted) return false;
        Plugin.Logger.LogInfo("Load Island Master Patch running 2");
        Plugin.hasHostStarted = false;
        return true;
    }
}