using HarmonyLib;
using UnityEngine;

namespace PEAKUnlimited.Patches;

public class EndSequenceRoutinePatch : MonoBehaviour
{
    [HarmonyPatch(typeof(EndScreen), "EndSequenceRoutine")]
    [HarmonyPostfix]
    static void Postfix(EndScreen __instance)
    {
        for (int i = 4; i < Character.AllCharacters.Count; i++)
        {
            Plugin.Logger.LogInfo("Deactivating an end screen");
            //Don't display the extra names since it blocks the chart
            Destroy(__instance.scoutWindows[i].gameObject);
            Plugin.Logger.LogInfo("Deleted an end screen");
        }
    }
}