using HarmonyLib;
using Steamworks;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class SteamLobbyHandlerSetLobbyDataPatch
{
    [HarmonyPatch(typeof(SteamLobbyHandler), "SetLobbyData")]
    [HarmonyPostfix]
    static void Postfix(SteamLobbyHandler __instance)
    {
        Plugin.Logger.LogInfo("Setting lobby member limit to " + ConfigurationHandler.ConfigMaxPlayers.Value);
        SteamMatchmaking.SetLobbyMemberLimit(__instance.m_currentLobby, ConfigurationHandler.ConfigMaxPlayers.Value);
    }
}