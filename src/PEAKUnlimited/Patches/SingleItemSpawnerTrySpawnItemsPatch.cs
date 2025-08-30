using HarmonyLib;
using Photon.Pun;

namespace PEAKUnlimited.Patches;

public class SingleItemSpawnerTrySpawnItemsPatch
{
    [HarmonyPatch(typeof(SingleItemSpawner), nameof(SingleItemSpawner.TrySpawnItems))]
    [HarmonyPrefix]
    static bool Prefix(SingleItemSpawner __instance)
    {
        if (__instance.prefab.name == "Marshmallow")
        {
            return false;
        }
        return true;
    }
}