using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;

namespace PEAKUnlimited.Patches;

public class SingleItemSpawnerTrySpawnItemsPatch
{
    [HarmonyPatch(typeof(Spawner), nameof(Spawner.TrySpawnItems))]
    [HarmonyPrefix]
    static bool Prefix(ref List<PhotonView> __result,Spawner __instance)
    {
        if (__instance.gameObject.name == "CampfireFoodSpawner")
        {
            __result = new List<PhotonView>();
            return false;
        }
        return true;
    }
}