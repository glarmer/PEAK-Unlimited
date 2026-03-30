using HarmonyLib;

namespace PEAKUnlimited.Patches.Voice;

public class CharacterVoiceHandlerStartPatch
{
    [HarmonyPatch(typeof(CharacterVoiceHandler), "Start")]
    [HarmonyPostfix]
    static void Postfix(CharacterVoiceHandler __instance)
    {
        if (__instance.m_character.IsLocal)
            return;

        byte group = PlayerHandler.AssignMixerGroup(__instance.m_character);
        try
        {
            __instance.m_source.outputAudioMixerGroup = __instance.GetMixerGroup(group);
        }
        catch
        {
            __instance.m_source.outputAudioMixerGroup = null;
        }
        __instance.m_parameter = __instance.GetMixerGroupParameter((byte)(group % 4));
    }
}