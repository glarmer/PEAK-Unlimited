using HarmonyLib;
using Zorro.Core;

namespace PEAKUnlimited.Patches;

public class AssignMixerGroupPatch
{
    [HarmonyPatch(typeof(PlayerHandler), "AssignMixerGroup")]
    [HarmonyPrefix]
    static bool Prefix(ref byte __result, Character character)
    {
        for (byte key = 0; key < ConfigurationHandler.ConfigMaxPlayers.Value; ++key)
        {
            if (!PlayerHandler.Instance.m_assignedVoiceGroups.ContainsKey(key) || !PlayerHandler.Instance.m_assignedVoiceGroups[key].UnityObjectExists<Character>())
            {
                PlayerHandler.Instance.m_assignedVoiceGroups[key] = character;
                __result = key;
                return false;
            }
        }
        __result = byte.MaxValue;
        return false;
    }
}