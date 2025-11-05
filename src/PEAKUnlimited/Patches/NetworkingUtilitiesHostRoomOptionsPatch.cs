using HarmonyLib;
using Peak.Network;
using Photon.Realtime;

namespace PEAKUnlimited.Patches;

public class NetworkingUtilitiesHostRoomOptionsPatch
{
    [HarmonyPatch(typeof(NetworkingUtilities), "HostRoomOptions")]
    [HarmonyPrefix]
    static bool Prefix(ref RoomOptions __result)
    {
        __result = new RoomOptions
        {
            IsVisible = false,
            MaxPlayers = ConfigurationHandler.ConfigMaxPlayers.Value,
            PublishUserId = true
        };
        return false;
    }
}