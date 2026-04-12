using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util;
using PEAKUnlimited.Util.Debugging;

namespace PEAKUnlimited.Patches;

public class LeaveLobbyPatch
{
    [HarmonyPatch(typeof(SteamLobbyHandler), nameof(SteamLobbyHandler.LeaveLobby))]
    [HarmonyPostfix]
    static void Postfix(SteamLobbyHandler __instance)
    {
        PlayerCountChangeUtilities.ForgetCampfiresAndMarshmallows();
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,"Left Lobby");
        PlayerConnectionLogAwakePatch.isHost = false;
    }
}