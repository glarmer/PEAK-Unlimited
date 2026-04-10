using HarmonyLib;
using Peak.Network;

namespace PEAKUnlimited.Util.Debugging;

public class SteamLobbyAPIPlayerIsInLobbyPatch
{
    
    [HarmonyPatch(typeof(SteamLobbyAPI), nameof(SteamLobbyAPI.PlayerIsInLobby))]
    [HarmonyPrefix]
    static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}