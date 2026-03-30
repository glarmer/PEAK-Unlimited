using HarmonyLib;

namespace PEAKUnlimited.Patches.Voice;

public class PlayerHandlerAssignMixerGroupPatch
{
    [HarmonyPatch(typeof(PlayerHandler), "AssignMixerGroup")]
    [HarmonyPrefix]
    static bool Prefix(ref byte __result, Character character)
    {
        int actor = character.photonView.Owner.ActorNumber;
        __result = (byte)(actor % 4);
        PlayerHandler.Instance.m_assignedVoiceGroups[__result] = character;
        return false;
    }
}