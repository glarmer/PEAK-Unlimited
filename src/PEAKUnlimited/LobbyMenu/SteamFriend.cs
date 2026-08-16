using Steamworks;

namespace PEAKUnlimited.LobbyMenu;

public class SteamFriend
{
    public CSteamID PlayerID { get; private set; }
    public string UserName { get; private set; }
    public string DisplayName { get; private set; }
    private bool _isInGame;
    private FriendGameInfo_t _gameInfo;
    public string GameStatus { get; private set; } = "";

    public SteamFriend(CSteamID playerID)
    {
        PlayerID = playerID;
        UpdateFriendDetails();
    }

    public void UpdateFriendDetails()
    {
        GetFriendDetails();
        GetFriendGameDetails();
    }

    private void GetFriendDetails()
    {
        UserName = SteamFriends.GetFriendPersonaName(PlayerID);
        DisplayName = SteamFriends.GetPlayerNickname(PlayerID) ?? UserName;
    }

    private void GetFriendGameDetails()
    {
        _isInGame = SteamFriends.GetFriendGamePlayed(PlayerID, out FriendGameInfo_t gameInfo);
        _gameInfo = gameInfo;
        if (_isInGame && gameInfo.m_gameID.IsValid())
        {
            Plugin.Logger.LogInfo($"{DisplayName} is playing a game");
            if (gameInfo.m_gameID.AppID() == SteamUtils.GetAppID())
            {
                Plugin.Logger.LogInfo($"{DisplayName} is playing PEAK");
                if (gameInfo.m_steamIDLobby.IsValid())
                {
                    Plugin.Logger.LogInfo($"{DisplayName} is playing PEAK and in a lobby");
                    GameStatus = $"{DisplayName} is in a lobby with x/y players!";
                }
                else
                {
                    Plugin.Logger.LogInfo($"{DisplayName} is playing PEAK but not in a lobby");
                    GameStatus = $"{DisplayName} is in the Main Menu!";
                }
            }
            else
            {
                Plugin.Logger.LogInfo($"{DisplayName} is not playing PEAK");
            }
        }
        else
        {
            Plugin.Logger.LogInfo($"{DisplayName} is not playing anything");
        }
    }
}