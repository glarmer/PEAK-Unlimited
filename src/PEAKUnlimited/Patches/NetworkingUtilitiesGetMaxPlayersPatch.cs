using BepInEx.Logging;
using HarmonyLib;
using Peak.Network;
using PEAKUnlimited.Util.Debugging;
using Photon.Realtime;

namespace PEAKUnlimited.Patches;

public class NetworkingUtilitiesGetMaxPlayersPatch
{
    [HarmonyPatch(typeof(NetworkingUtilities), nameof(NetworkingUtilities.MAX_PLAYERS), MethodType.Getter)]
    [HarmonyPrefix]
    static bool Prefix(ref int __result)
    {
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,$"Get max players patch!");
        __result = ConfigurationHandler.ConfigMaxPlayers.Value;
        return false;
    }
}