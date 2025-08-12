using HarmonyLib;
using Photon.Pun;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class PlayClickedPatch
{
    [HarmonyPatch(typeof(MainMenuMainPage), "PlayClicked")]
    [HarmonyPostfix]
    static void Postfix(MainMenuMainPage __instance)
    {
        //This is a gross way of testing if a user created a lobby, since PhotonNetwork.IsMasterClient doesn't seem to work in PlayerConnectionLog
        Plugin.Logger.LogInfo("Play clicked");
        PlayerConnectionLogAwakePatch.isHost = true;
    }
}