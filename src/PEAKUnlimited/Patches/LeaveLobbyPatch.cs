using HarmonyLib;

namespace PEAKUnlimited.Patches;

public class LeaveLobbyPatch
{
    [HarmonyPatch(typeof(SteamLobbyHandler), "LeaveLobby")]
    [HarmonyPostfix]
    static void Postfix(SteamLobbyHandler __instance)
    {
        //This is part of a gross way of testing if a user created a lobby, since PhotonNetwork.IsMasterClient doesn't seem to work in PlayerConnectionLog
        Plugin.Logger.LogInfo("Left Lobby");
        PlayerConnectionLogAwakePatch.isHost = false;
    }
}