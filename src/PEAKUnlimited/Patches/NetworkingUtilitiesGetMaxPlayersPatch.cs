using HarmonyLib;
using Peak.Network;

namespace PEAKUnlimited.Patches;

public class NetworkingUtilitiesGetMaxPlayersPatch
{
    /* This patch should be all that's needed to change the max player count.
     
     In the past max players was a variable that could be changed, then for a period it was a constant.
     In case this getter is not used in all situations or changes in the future, I have opted to keep the patches
     from when it was a constant as well as adding this. In the future, I will do testing and remove redundant patches.
     */
    [HarmonyPatch(typeof(NetworkingUtilities), nameof(NetworkingUtilities.MAX_PLAYERS), MethodType.Getter)]
    [HarmonyPrefix]
    static bool Prefix(ref int __result)
    {
        __result = ConfigurationHandler.ConfigMaxPlayers.Value;
        return false;
    }
}