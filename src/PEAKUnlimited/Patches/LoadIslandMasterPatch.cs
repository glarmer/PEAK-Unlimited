using HarmonyLib;
using Photon.Pun;

namespace PEAKUnlimited.Patches;

public class LoadIslandMasterPatch
{
    [HarmonyPatch(typeof(AirportCheckInKiosk), "LoadIslandMaster")]
    [HarmonyPrefix]
    static bool Prefix()
    {
        if (Plugin.config.LockKiosk)
        {
            Plugin.Logger.LogInfo("Load Island Master Patch running");
            if (!PhotonNetwork.IsMasterClient || !Plugin.hasHostStarted) return false;
        }
        Plugin.hasHostStarted = false;
        return true;
    }
}