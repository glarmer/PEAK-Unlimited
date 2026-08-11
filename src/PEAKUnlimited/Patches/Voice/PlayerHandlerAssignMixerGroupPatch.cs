using System.Collections.Generic;
using HarmonyLib;

namespace PEAKUnlimited.Patches.Voice;

public class PlayerHandlerAssignMixerGroupPatch
{
    [HarmonyPatch(typeof(PlayerHandler), nameof(PlayerHandler.AssignMixerGroup))]
    [HarmonyPrefix]
    static bool Prefix(ref byte __result, Character character)
    {
        int actor = character.photonView.Owner.ActorNumber;
        __result = (byte)(actor % 4);
        
        var assignedVoiceGroups = (Dictionary<byte, Character>)AccessTools.Field(typeof(PlayerHandler), "m_assignedVoiceGroups").GetValue(GameHandler.GetService<PlayerHandler>());
        assignedVoiceGroups[__result] = character;
        
        return false;
    }
}