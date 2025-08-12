using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace PEAKUnlimited.Patches;

public class EndScreenStartPatch : MonoBehaviour
    {
        [HarmonyPatch(typeof(EndScreen), "Start")]
        [HarmonyPrefix]
        static void Prefix(EndScreen __instance)
        {
            if (Character.AllCharacters.Count <= Plugin.VANILLA_MAX_PLAYERS)
                return;

            var newScoutWindows = new EndScreenScoutWindow[Character.AllCharacters.Count];
            var newScouts = new Image[Character.AllCharacters.Count];
            var newScoutsAtPeak = new Image[Character.AllCharacters.Count];
            var newOldPip = new Image[Character.AllCharacters.Count];
            var newScoutLines = new Transform[Character.AllCharacters.Count];

            for (int i = 0; i < Character.AllCharacters.Count; i++)
            {
                //Don't do anything to the original ones
                bool withinExisting = i < __instance.scouts.Length;
                if (!withinExisting)
                {
                    if ((UnityEngine.Object)__instance.scoutWindows[0] == null)
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
                    
                    if ((UnityEngine.Object)__instance.scouts[0] == null)
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
                    
                    if ((UnityEngine.Object)__instance.scoutsAtPeak[0] == null)
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
                    
                    if ((UnityEngine.Object)__instance.oldPip[0] == null)
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
                    
                    if ((UnityEngine.Object)__instance.scoutLines[0] == null)
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

            //Reassign arrays with new ones
            __instance.scoutWindows = newScoutWindows;
            __instance.scouts = newScouts;
            __instance.scoutsAtPeak = newScoutsAtPeak;
            __instance.oldPip = newOldPip;
            __instance.scoutLines = newScoutLines;
        }
    }