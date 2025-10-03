using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util.Debugging;
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
            bool shouldCancelIslandLoad = !PhotonNetwork.IsMasterClient || !Plugin.HasHostStarted;
            UltimateLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,$"Host Only Kiosk Logic being tested: Should Cancel Island Load = {shouldCancelIslandLoad}");
            
            if (!PhotonNetwork.IsMasterClient || !Plugin.HasHostStarted) return false;
        }
        Plugin.HasHostStarted = false;
        return true;
    }
}