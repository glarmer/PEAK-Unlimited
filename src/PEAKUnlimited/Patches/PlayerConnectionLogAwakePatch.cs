using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PEAKUnlimited.Patches;

public class PlayerConnectionLogAwakePatch
{
    public static bool isHost = false;
    [HarmonyPatch(typeof(PlayerConnectionLog), "Awake")]
    [HarmonyPostfix]
    static void Postfix(PlayerConnectionLog __instance)
    {

        if (!isHost) return;
        if (GameObject.Find("AirportGateKiosk") == null) return;
        
        __instance.AddMessage($"{__instance.GetColorTag(__instance.joinedColor)} Lobby started with: </color>{__instance.GetColorTag(__instance.userColor)} PEAK Unlimited </color>");

        if (Plugin.config.isLobbyDetailsEnabled)
        {
            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Max players: </color>{__instance.GetColorTag(__instance.userColor)} {NetworkConnector.MAX_PLAYERS} </color>");

            string isEnabled = "Enabled";
            if (!Plugin.config.IsExtraMarshmallowsEnabled)
            {
                isEnabled = "Disabled";
            }

            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Extra marshmallows: </color>{__instance.GetColorTag(__instance.userColor)} {isEnabled} </color>");

            isEnabled = "Enabled";
            if (!Plugin.config.IsExtraBackpacksEnabled)
            {
                isEnabled = "Disabled";
            }

            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Extra backpacks: </color>{__instance.GetColorTag(__instance.userColor)} {isEnabled} </color>");

            isEnabled = "Enabled";
            if (!Plugin.config.LockKiosk)
            {
                isEnabled = "Disabled";
            }

            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Host only kiosk: </color>{__instance.GetColorTag(__instance.userColor)} {isEnabled} </color>");
        }
    }
}