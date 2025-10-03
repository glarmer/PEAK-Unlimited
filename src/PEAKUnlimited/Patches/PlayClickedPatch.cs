using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util.Debugging;
using Photon.Pun;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class PlayClickedPatch
{
    [HarmonyPatch(typeof(MainMenuMainPage), "PlayClicked")]
    [HarmonyPostfix]
    static void Postfix(MainMenuMainPage __instance)
    {
        Plugin.CampfireList.Clear();
        //This is a gross way of testing if a user created a lobby, since PhotonNetwork.IsMasterClient doesn't seem to work in PlayerConnectionLog
        UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.NetworkingLogic,"Play clicked, this player is the lobby host!");
        PlayerConnectionLogAwakePatch.isHost = true;
    }
}