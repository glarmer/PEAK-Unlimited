using BepInEx.Logging;
using HarmonyLib;
using PEAKUnlimited.Util.Debugging;
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
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.EndScreenLogic,"Deactivating an end screen");
            //Don't display the extra names since it blocks the chart
            Destroy(__instance.scoutWindows[i].gameObject);
            UnlimitedLogger.GetInstance().DebugMessage(LogLevel.Info, DebugLogType.EndScreenLogic,"Deleted an end screen");
        }
    }
}