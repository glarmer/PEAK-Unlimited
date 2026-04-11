using HarmonyLib;
using UnityEngine;

namespace PEAKUnlimited.Patches;

[HarmonyPatch(typeof(UIPlayerNames))]
class UIPlayerNamesPatches
{

    private static void FixArrayLengths(UIPlayerNames instance)
    {
        if (!Plugin.ConfigurationHandler.NamePlateFix) return;
        
        int targetSize = Plugin.ConfigurationHandler.MaxPlayers;
        if (instance.playerNameText == null || instance.playerNameText.Length >= targetSize)
            return;

        var oldArray = instance.playerNameText;
        int oldLength = oldArray.Length;

        PlayerName template = oldArray[0];
        Transform parent = template.transform.parent;

        PlayerName[] newArray = new PlayerName[targetSize];
        
        for (int i = 0; i < oldLength; i++)
            newArray[i] = oldArray[i];
        
        for (int i = oldLength; i < targetSize; i++)
        {
            PlayerName clone = Object.Instantiate(template, parent);
            clone.gameObject.SetActive(false);
            
            clone.group.alpha = 0f;
            clone.audioImageTimeout = 0f;

            newArray[i] = clone;
        }

        instance.playerNameText = newArray;
    }
    
    [HarmonyPatch(nameof(UIPlayerNames.Init))]
    [HarmonyPrefix]
    static void InitPrefix(UIPlayerNames __instance)
    {
        if (!Plugin.ConfigurationHandler.NamePlateFix) return;
        
        FixArrayLengths(__instance);
    }
    
    [HarmonyPatch(nameof(UIPlayerNames.UpdateName))]
    [HarmonyPrefix]
    static bool UpdateNamePrefix(UIPlayerNames __instance, int index)
    {
        if (!Plugin.ConfigurationHandler.NamePlateFix) return true;
        
        FixArrayLengths(__instance);

        if (__instance.playerNameText == null)
            return false;

        if (index >= __instance.playerNameText.Length)
            return false;

        return true;
    }
    
    [HarmonyPatch(nameof(UIPlayerNames.DisableName))]
    [HarmonyPrefix]
    static bool DisableNamePrefix(UIPlayerNames __instance, int index)
    {
        if (!Plugin.ConfigurationHandler.NamePlateFix) return true;
        
        if (__instance.playerNameText == null)
            return false;

        if (index >= __instance.playerNameText.Length)
            return false;

        return true;
    }
}