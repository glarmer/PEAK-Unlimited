using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace PEAKUnlimited.LobbyMenu;

public class FriendLobbyMenu : MonoBehaviour
{
    private Dictionary<CSteamID, SteamFriend> _friends = new Dictionary<CSteamID, SteamFriend>();
    
    public void Awake()
    {
        GetSteamFriends();
    }

    public void GetSteamFriends()
    {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < friendCount; i++)
        {
            CSteamID steamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            _friends.Add(steamID, new SteamFriend(steamID));
        }
    }
}