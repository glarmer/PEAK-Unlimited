using HarmonyLib;
using Peak.Network;

namespace PEAKUnlimited.Patches;

public class NetworkingUtilitiesGetMaxPlayersPatch
{
    
    [HarmonyPatch(typeof(NetworkingUtilities), nameof(NetworkingUtilities.MAX_PLAYERS), MethodType.Getter)]
    [HarmonyPrefix]
    static bool Prefix(ref int __result)
    {
        __result = ConfigurationHandler.ConfigMaxPlayers.Value;
        return false;
    }
}