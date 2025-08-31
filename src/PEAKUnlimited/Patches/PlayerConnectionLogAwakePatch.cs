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

        if (Plugin.ConfigurationHandler.IsLobbyDetailsEnabled)
        {
            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Max players: </color>{__instance.GetColorTag(__instance.userColor)} {NetworkConnector.MAX_PLAYERS} </color>");

            string isEnabled = "Enabled";
            if (!Plugin.ConfigurationHandler.IsExtraMarshmallowsEnabled)
            {
                isEnabled = "Disabled";
            }

            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Extra marshmallows: </color>{__instance.GetColorTag(__instance.userColor)} {isEnabled} </color>");

            isEnabled = "Enabled";
            if (!Plugin.ConfigurationHandler.IsExtraBackpacksEnabled)
            {
                isEnabled = "Disabled";
            }

            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Extra backpacks: </color>{__instance.GetColorTag(__instance.userColor)} {isEnabled} </color>");

            isEnabled = "Enabled";
            if (!Plugin.ConfigurationHandler.IsLateMarshmallowsEnabled)
            {
                isEnabled = "Disabled";
            }

            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Late join marshmallows: </color>{__instance.GetColorTag(__instance.userColor)} {isEnabled} </color>");

            
            isEnabled = "Enabled";
            if (!Plugin.ConfigurationHandler.LockKiosk)
            {
                isEnabled = "Disabled";
            }

            __instance.AddMessage(
                $"{__instance.GetColorTag(__instance.joinedColor)} Host only kiosk: </color>{__instance.GetColorTag(__instance.userColor)} {isEnabled} </color>");
            if (Plugin.ConfigurationHandler.CheatMarshmallows > 0)
            {
                __instance.AddMessage(
                    $"{__instance.GetColorTag(__instance.joinedColor)} Cheat marshmallows: </color>{__instance.GetColorTag(__instance.userColor)} {Plugin.ConfigurationHandler.CheatMarshmallows} </color>");
            }
            if (Plugin.ConfigurationHandler.CheatBackpacks > 0)
            {
                __instance.AddMessage(
                    $"{__instance.GetColorTag(__instance.joinedColor)} Cheat backpacks: </color>{__instance.GetColorTag(__instance.userColor)} {Plugin.ConfigurationHandler.CheatBackpacks} </color>");
            }
        }
        
        __instance.AddMessage($"{__instance.GetColorTag(__instance.joinedColor)} Configure PEAK Unlimited with: </color>{__instance.GetColorTag(__instance.userColor)} F2 </color>");

    }
}