using HarmonyLib;
using Photon.Pun;
using Zorro.Core;

namespace PEAKUnlimited;

public class EndScreenNextPatch
{
    [HarmonyPatch(typeof (EndScreen), "Next")]
    [HarmonyPostfix]
    private static void PostFix()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Singleton<PeakHandler>.Instance.EndScreenComplete();
        }
    }
}