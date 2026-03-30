using HarmonyLib;

namespace PEAKUnlimited.Patches.Voice;

public class CharacterVoiceHandlerUpdatePatch
{
    [HarmonyPatch(typeof(CharacterVoiceHandler), "Update")]
    [HarmonyPostfix]
    static void Postfix(CharacterVoiceHandler __instance)
    {
        if (__instance.m_character == null)
            return;

        string userId = __instance.m_character.photonView.Owner.UserId;
        float level = AudioLevels.GetPlayerLevel(userId);
        __instance.m_source.volume = level;
    }
}