using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace PEAKUnlimited.Patches;

public class WaitingForPlayersUIPatch : MonoBehaviour
{
    [HarmonyPatch(typeof(WaitingForPlayersUI), "Update")]
    [HarmonyPrefix]
    static void Prefix(WaitingForPlayersUI __instance)
    {
        if (__instance.scoutImages.Length >= Character.AllCharacters.Count)
            return;
        var newScoutImages = new Image[Character.AllCharacters.Count];
        Image original = __instance.scoutImages[0];
        for (int i = 0; i < Character.AllCharacters.Count; i++)
        {
            if (i < Plugin.VANILLA_MAX_PLAYERS)
            {
                newScoutImages[i] = __instance.scoutImages[i];
            }
            else
            {
                newScoutImages[i] = Instantiate(original, original.transform.parent);
            }
        }
        __instance.scoutImages = newScoutImages;
    }
}