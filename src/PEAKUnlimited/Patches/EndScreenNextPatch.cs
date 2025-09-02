using HarmonyLib;
using Photon.Pun;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class EndScreenNextPatch
{
    [HarmonyPatch(typeof (EndScreen), "Next")]
    [HarmonyPostfix]
    private static void PostFix()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Plugin.CampfireList.Clear();
            Singleton<PeakHandler>.Instance.EndScreenComplete();
        }
    }
}