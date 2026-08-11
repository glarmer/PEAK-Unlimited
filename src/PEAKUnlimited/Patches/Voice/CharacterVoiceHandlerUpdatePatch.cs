using HarmonyLib;
using UnityEngine;

namespace PEAKUnlimited.Patches.Voice;

public class CharacterVoiceHandlerUpdatePatch
{
    [HarmonyPatch(typeof(CharacterVoiceHandler), "Update")]
    [HarmonyPostfix]
    static void Postfix(CharacterVoiceHandler __instance)
    {
        Character _char = (Character)AccessTools.Field(typeof(CharacterVoiceHandler), "m_character").GetValue(__instance);
        
        if (_char == null)
            return;

        string userId = _char.photonView.Owner.UserId;
        float level = AudioLevels.GetPlayerLevel(userId);
        ((AudioSource)AccessTools.Field(typeof(CharacterVoiceHandler), "m_source").GetValue(__instance)).volume = level;
    }
}