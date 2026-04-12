using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace PEAKUnlimited.Patches;

[HarmonyPatch(typeof(PeakHandler))]
public class PeakHandlerPatches
{
    [HarmonyPatch(nameof(PeakHandler.EndCutscene))]
    [HarmonyPrefix]
    static void EndCutscenePrefix(PeakHandler __instance)
    {
        if (!Plugin.ConfigurationHandler.AllScoutsHelicopter) return;
        
        Plugin.Logger.LogInfo("[PeakHandler] EndCutscene Prefix called");

        int maxPlayers = Plugin.ConfigurationHandler.MaxPlayers;

        if (__instance.cutsceneScoutRefs == null || __instance.cutsceneScoutRefs.Length == 0)
        {
            Plugin.Logger.LogInfo("[PeakHandler] cutsceneScoutRefs is null or empty");
            return;
        }

        if (__instance.cutsceneScoutRefs.Length >= maxPlayers)
        {
            Plugin.Logger.LogInfo("[PeakHandler] Arrays are already large enough");
            return;
        }

        var oldRefs = __instance.cutsceneScoutRefs;
        var oldAnims = __instance.cutsceneScoutAnims;

        var templateRoot = oldRefs[0].transform.parent.gameObject;
        var parent = templateRoot.transform.parent;

        var newRefs = new CustomizationRefs[maxPlayers];
        var newAnims = new EndCutsceneScoutHelper[maxPlayers];
        
        for (int i = 0; i < oldRefs.Length; i++)
        {
            newRefs[i] = oldRefs[i];
            newAnims[i] = oldAnims[i];
        }
        
        for (int i = oldRefs.Length; i < maxPlayers; i++)
        {
            Plugin.Logger.LogInfo($"[PeakHandler] Creating a clone: P ({i + 1})");

            var cloneRoot = Object.Instantiate(templateRoot, parent);
            cloneRoot.name = $"P ({i + 1})";

            var cloneRef = cloneRoot.GetComponentInChildren<CustomizationRefs>(true);
            var cloneAnim = cloneRoot.GetComponentInChildren<EndCutsceneScoutHelper>(true);
            
            cloneRef.transform.name = $"Scout P{i + 1} Sit";

            cloneRoot.SetActive(false);

            newRefs[i] = cloneRef;
            newAnims[i] = cloneAnim;
        }

        __instance.cutsceneScoutRefs = newRefs;
        __instance.cutsceneScoutAnims = newAnims;

        Plugin.Logger.LogInfo("[PeakHandler] Finished expanding arrays");
    }
    
    [HarmonyPatch(nameof(PeakHandler.SetCosmetics))]
    [HarmonyPostfix]
    static void SetCosmeticsPostfix(PeakHandler __instance, List<Character> characters)
    {
        if (!Plugin.ConfigurationHandler.AllScoutsHelicopter) return;
        
        Plugin.Logger.LogInfo("[PeakHandler] SetCosmetics Postfix called");

        if (characters == null)
        {
            Plugin.Logger.LogInfo("[PeakHandler] characters list is null");
            return;
        }

        Plugin.Logger.LogInfo($"[PeakHandler] Total characters passed in: {characters.Count}");

        var valid = characters
            .Where(c => c.refs.stats.won)
            .OrderBy(c => c.photonView.ViewID)
            .ToList();

        Plugin.Logger.LogInfo($"[PeakHandler] Valid (won) characters: {valid.Count}");

        if (valid.Count <= 4)
        {
            Plugin.Logger.LogInfo("[PeakHandler] <= 4 players, nothing to extend");
            return;
        }

        var localMouths = (List<AnimatedMouth>)AccessTools
            .Field(typeof(PeakHandler), "localMouths")
            .GetValue(__instance);
        
        for (int i = 4; i < valid.Count && i < __instance.cutsceneScoutRefs.Length; i++)
        {
            Plugin.Logger.LogInfo($"[PeakHandler] Processing extra player index {i}");

            var character = valid[i];
            var scout = __instance.cutsceneScoutRefs[i];

            if (scout == null)
            {
                Plugin.Logger.LogInfo($"[PeakHandler] scout at index {i} is null");
                continue;
            }
            
            var root = scout.transform.parent.gameObject;
            root.SetActive(true);

            character.refs.customization.SetCustomizationForRef(scout);

            if (character.data.isSkeleton)
                scout.SetSkeleton(true, false);

            BadgeUnlocker.SetBadges(character, scout.sashRenderer);

            var mouth = scout.GetComponent<AnimatedMouth>();
            mouth.audioSource = character.GetComponent<AnimatedMouth>().audioSource;

            if (character.IsLocal)
            {
                Plugin.Logger.LogInfo($"[PeakHandler] Adding local mouth for player {i}");
                localMouths.Add(mouth);
            }
        }
        
        int total = valid.Count;
        if (total > 1)
        {
            var refs = __instance.cutsceneScoutRefs;

            var leftScout = refs[0].transform;
            var rightScout = refs[3].transform;

            float leftX = leftScout.localPosition.x;
            float rightX = rightScout.localPosition.x;

            Plugin.Logger.LogInfo($"[PeakHandler] Repositioning & scaling {total} players");

            Vector3 originalScale = refs[0].transform.localScale;
            Vector3 originalScale2 = refs[0].transform.parent.localScale;
            for (int i = 0; i < total && i < refs.Length; i++)
            {
                var scout = refs[i];
                if (scout == null)
                    continue;

                var root = scout.transform.parent;
                root.gameObject.SetActive(true);

                float t = (float) i / (total - 1);
                float newX = Mathf.Lerp(leftX, rightX, t);

                var position = scout.transform.localPosition;
                position.x = newX;
                scout.transform.localPosition = position;
                
                float scaleFactor = Mathf.Clamp(4f / total, 0.01f, 1f);
                scout.transform.localScale = originalScale * scaleFactor;
                //root.transform.localScale = originalScale2 * scaleFactor;
                root.localRotation = refs[0].transform.parent.localRotation;
            }
        }
    }
}