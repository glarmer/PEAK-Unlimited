using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;

namespace PEAKUnlimited.Patches.Voice;

public class CharacterVoiceHandlerStartPatch
{
    [HarmonyPatch(typeof(CharacterVoiceHandler), "Start")]
    [HarmonyPostfix]
    static void Postfix(CharacterVoiceHandler __instance)
    {
        Character _char = (Character)AccessTools.Field(typeof(CharacterVoiceHandler), "m_character").GetValue(__instance);
        
        if (_char.IsLocal)
            return;

        AudioSource source = (AudioSource)AccessTools.Field(typeof(CharacterVoiceHandler), "m_source").GetValue(__instance);
        byte group = PlayerHandler.AssignMixerGroup(_char);

        var GetMixerGroup = AccessTools.Method(typeof(CharacterVoiceHandler), "GetMixerGroup");
        var GetMixerGroupParameter = AccessTools.Method(typeof(CharacterVoiceHandler), "GetMixerGroupParameter");
        
        try
        {
            source.outputAudioMixerGroup = (AudioMixerGroup)GetMixerGroup.Invoke(__instance, new object[] {group});
        }
        catch
        {
            source.outputAudioMixerGroup = null;
        }
        AccessTools.Field(typeof(CharacterVoiceHandler), "m_parameter").SetValue(__instance, GetMixerGroupParameter.Invoke(__instance, new object[]{(byte)(group % 4)}));
    }
}