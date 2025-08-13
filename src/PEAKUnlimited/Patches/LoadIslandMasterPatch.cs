using HarmonyLib;
using Photon.Pun;

namespace PEAKUnlimited.Patches;

public class LoadIslandMasterPatch
{
    [HarmonyPatch(typeof(AirportCheckInKiosk), "LoadIslandMaster")]
    [HarmonyPrefix]
    static bool Prefix()
    {
        if (Plugin.ConfigurationHandler.LockKiosk)
        {
            Plugin.Logger.LogInfo("Load Island Master Patch running");
            if (!PhotonNetwork.IsMasterClient || !Plugin.HasHostStarted) return false;
        }
        Plugin.HasHostStarted = false;
        return true;
    }
}