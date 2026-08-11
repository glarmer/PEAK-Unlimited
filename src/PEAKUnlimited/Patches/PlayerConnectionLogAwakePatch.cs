using System.Reflection;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PEAKUnlimited.Patches;

public class PlayerConnectionLogAwakePatch
{
    public static bool isHost = false;

    public static string GetColorTag(Color c)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(c)}>";
    }

    public static void AddMessage(PlayerConnectionLog instance, string msg)
    {
        MethodInfo AddMessage = AccessTools.Method(typeof(PlayerConnectionLog), "AddMessage");
        AddMessage.Invoke(instance, new object[] { msg });
    }
    
    [HarmonyPatch(typeof(PlayerConnectionLog), "Awake")]
    [HarmonyPostfix]
    static void Postfix(PlayerConnectionLog __instance)
    {
        if (!isHost) return;
        if (GameObject.Find("AirportGateKiosk") == null) return;
        
        AddMessage(__instance, $"{GetColorTag(__instance.joinedColor)} Lobby started with: </color>{(string)GetColorTag(__instance.userColor)} PEAK Unlimited </color>");

        if (Plugin.ConfigurationHandler.IsLobbyDetailsEnabled)
        {
            AddMessage(__instance,
                $"{GetColorTag(__instance.joinedColor)} Max players: </color>{GetColorTag(__instance.userColor)} {ConfigurationHandler.ConfigMaxPlayers.Value} </color>");

            string isEnabled = "Enabled";
            if (!Plugin.ConfigurationHandler.IsExtraMarshmallowsEnabled)
            {
                isEnabled = "Disabled";
            }

            AddMessage(__instance,
                $"{GetColorTag(__instance.joinedColor)} Extra marshmallows: </color>{GetColorTag(__instance.userColor)} {isEnabled} </color>");

            isEnabled = "Enabled";
            if (!Plugin.ConfigurationHandler.IsExtraBackpacksEnabled)
            {
                isEnabled = "Disabled";
            }

            AddMessage(__instance,
                $"{GetColorTag(__instance.joinedColor)} Extra backpacks: </color>{GetColorTag(__instance.userColor)} {isEnabled} </color>");

            isEnabled = "Enabled";
            if (!Plugin.ConfigurationHandler.IsLateMarshmallowsEnabled)
            {
                isEnabled = "Disabled";
            }

            AddMessage(__instance,
                $"{GetColorTag(__instance.joinedColor)} Late join marshmallows: </color>{GetColorTag(__instance.userColor)} {isEnabled} </color>");

            
            isEnabled = "Enabled";
            if (!Plugin.ConfigurationHandler.LockKiosk)
            {
                isEnabled = "Disabled";
            }

            AddMessage(__instance,
                $"{GetColorTag(__instance.joinedColor)} Host only kiosk: </color>{GetColorTag(__instance.userColor)} {isEnabled} </color>");
            if (Plugin.ConfigurationHandler.CheatMarshmallows > 0)
            {
                AddMessage(__instance,
                    $"{GetColorTag(__instance.joinedColor)} Cheat marshmallows: </color>{GetColorTag(__instance.userColor)} {Plugin.ConfigurationHandler.CheatMarshmallows} </color>");
            }
            if (Plugin.ConfigurationHandler.CheatBackpacks > 0)
            {
                AddMessage(__instance,
                    $"{GetColorTag(__instance.joinedColor)} Cheat backpacks: </color>{GetColorTag(__instance.userColor)} {Plugin.ConfigurationHandler.CheatBackpacks} </color>");
            }
        }
        
        AddMessage(__instance,
            $"{GetColorTag(__instance.joinedColor)} Configure PEAK Unlimited with: </color>{GetColorTag(__instance.userColor)} F2 </color>");
    }
}