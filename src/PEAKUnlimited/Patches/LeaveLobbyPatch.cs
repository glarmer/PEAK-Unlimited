using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util.Debugging;

namespace PEAKUnlimited.Patches;

public class LeaveLobbyPatch
{
    [HarmonyPatch(typeof(SteamLobbyHandler), "LeaveLobby")]
    [HarmonyPostfix]
    static void Postfix(SteamLobbyHandler __instance)
    {
        //This is part of a gross way of testing if a user created a lobby, since PhotonNetwork.IsMasterClient doesn't seem to work in PlayerConnectionLog
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,"Left Lobby");
        PlayerConnectionLogAwakePatch.isHost = false;
    }
}