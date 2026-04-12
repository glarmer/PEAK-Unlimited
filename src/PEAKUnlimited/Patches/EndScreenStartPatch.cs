using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace PEAKUnlimited.Patches;

public class EndScreenStartPatch : MonoBehaviour
    {
        [HarmonyPatch(typeof(EndScreen), nameof(EndScreen.Start))]
        [HarmonyPrefix]
        static void Prefix(EndScreen __instance)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            if (playerCount <= Plugin.VanillaMaxPlayers)
                return;

            var newScoutWindows = new EndScreenScoutWindow[playerCount];
            var newScouts = new Image[playerCount];
            var newScoutsAtPeak = new Image[playerCount];
            var newOldPip = new Image[playerCount];
            var newScoutLines = new Transform[playerCount];

            for (int i = 0; i < playerCount; i++)
            {
                bool withinExisting = i < __instance.scouts.Length;
                if (!withinExisting)
                {
                    if (__instance.scoutWindows[0] == null)
                    {
                        newScoutWindows[i] = null;
                    }
                    else
                    {
                        newScoutWindows[i] = Instantiate(
                            __instance.scoutWindows[0],
                            __instance.scoutWindows[0].transform.parent
                        );
                    }
                    
                    if (__instance.scouts[0] == null)
                    {
                        newScouts[i] = null;
                    }
                    else
                    {
                        newScouts[i] = Instantiate(
                            __instance.scouts[0],
                            __instance.scouts[0].transform.parent
                        );
                    }
                    
                    if (__instance.scoutsAtPeak[0] == null)
                    {
                        newScoutsAtPeak[i] = null;
                    }
                    else
                    {
                        newScoutsAtPeak[i] = Instantiate(
                            __instance.scoutsAtPeak[0],
                            __instance.scoutsAtPeak[0].transform.parent
                        );
                    }
                    
                    if (__instance.oldPip[0] == null)
                    {
                        newOldPip[i] = null;
                    }
                    else
                    {
                        newOldPip[i] = Instantiate(
                            __instance.oldPip[0],
                            __instance.oldPip[0].transform.parent
                        );
                    }
                    
                    if (__instance.scoutLines[0] == null)
                    {
                        newScoutLines[i] = null;
                    }
                    else
                    {
                        newScoutLines[i] = Instantiate(
                            __instance.scoutLines[0],
                            __instance.scoutLines[0].transform.parent
                        );
                    }}
                else
                {
                    newScoutWindows[i] = __instance.scoutWindows[i];
                    newScouts[i] = __instance.scouts[i];
                    newScoutsAtPeak[i] = __instance.scoutsAtPeak[i];
                    newOldPip[i] = __instance.oldPip[i];
                    newScoutLines[i] = __instance.scoutLines[i];
                }
            }
            
            __instance.scoutWindows = newScoutWindows;
            __instance.scouts = newScouts;
            __instance.scoutsAtPeak = newScoutsAtPeak;
            __instance.oldPip = newOldPip;
            __instance.scoutLines = newScoutLines;
        }
    }