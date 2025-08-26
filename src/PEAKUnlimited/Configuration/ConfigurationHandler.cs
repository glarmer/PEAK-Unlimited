using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PEAKUnlimited;

public class ConfigurationHandler
{
    private ConfigFile _config = new ConfigFile(Path.Combine(Paths.ConfigPath, "PEAKUnlimited.cfg"), true);
    public InputAction MenuAction { get; set; }

    public ConfigEntry<int> ConfigMaxPlayers;
    public ConfigEntry<bool> ConfigLockKiosk;
    public ConfigEntry<bool> ConfigLobbyDetails;
    public ConfigEntry<bool> ConfigExtraMarshmallows;
    public ConfigEntry<bool> ConfigLateMarshmallows;
    public ConfigEntry<int> ConfigCheatExtraMarshmallows;
    public ConfigEntry<bool> ConfigExtraBackpacks;
    public ConfigEntry<int> ConfigCheatExtraBackpacks;
    public ConfigEntry<string> ConfigMenuKey;
    
    public int MaxPlayers => ConfigMaxPlayers.Value;
    public bool LockKiosk => ConfigLockKiosk.Value;
    public bool IsLobbyDetailsEnabled => ConfigLobbyDetails.Value;
    public bool IsExtraMarshmallowsEnabled => ConfigExtraMarshmallows.Value;
    public bool IsLateMarshmallowsEnabled => ConfigLateMarshmallows.Value;
    public int CheatMarshmallows => ConfigCheatExtraMarshmallows.Value;
    public bool IsExtraBackpacksEnabled => ConfigExtraBackpacks.Value;
    public int CheatBackpacks => ConfigCheatExtraBackpacks.Value;
    
    
    public ConfigurationHandler()
    {
        Plugin.Logger.LogInfo("ConfigurationHandler initialising");
        ConfigMaxPlayers = _config.Bind
        (
            "General",
            "MaxPlayers",
            20,
            "The maximum number of players you want to be able to join your lobby (Including yourself). Warning: untested, higher numbers may be unstable! Range: 1-20"
        );
        if (ConfigMaxPlayers.Value == 0)
        {
            ConfigMaxPlayers.Value = 1;
        }
        else if (ConfigMaxPlayers.Value > 30)
        {
            ConfigMaxPlayers.Value = 30;
        }
        Plugin.Logger.LogInfo("ConfigurationHandler: Max Players Loaded: " + ConfigMaxPlayers.Value);
        
        ConfigMaxPlayers.SettingChanged += OnMaxPlayersChanged;
        
        ConfigLockKiosk = _config.Bind
        (
            "General",
            "LockKiosk",
            false,
            "Allows you to stop other players starting the game from the Airport Kiosk"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Lock Kiosk enabled: " + ConfigLockKiosk.Value);
        
        ConfigLobbyDetails = _config.Bind
        (
            "General",
            "LobbyDetails",
            true,
            "Prints the lobby details in the join log when a game is started"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Lobby details enabled: " + ConfigLobbyDetails.Value);
        
        ConfigExtraMarshmallows = _config.Bind
        (
            "General",
            "ExtraMarshmallows",
            true,
            "Controls whether additional marshmallows are spawned for the extra players"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Extra marshmallows enabled: " + ConfigExtraMarshmallows.Value);
        
        ConfigExtraBackpacks = _config.Bind
        (
            "General",
            "ExtraBackpacks",
            true,
            "Controls whether additional backpacks have a chance to be spawned for extra players"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Extra backpacks enabled: " + ConfigExtraBackpacks.Value);
        
        ConfigLateMarshmallows = _config.Bind
        (
            "General",
            "LateJoinMarshmallows",
            false,
            "Controls whether additional marshmallows are spawned for players who join late (mid run), and removed for those who leave early (Experimental + Untested)"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Late Marshmallows enabled: " + ConfigLateMarshmallows.Value);

        ConfigCheatExtraMarshmallows = _config.Bind
        (
            "General",
            "Cheat Marshmallows",
            0,
            "(Cheat, disabled by default) This will set the desired amount of marshmallows to the campfires as a cheat, requires ExtraMarshmallows to be enabled. Capped at 30."
        );
        if (ConfigCheatExtraMarshmallows.Value > 30)
        {
            ConfigCheatExtraMarshmallows.Value = 30;
        }
        else if (ConfigCheatExtraMarshmallows.Value < 0)
        {
            ConfigCheatExtraMarshmallows.Value = 0;
        }
        Plugin.Logger.LogInfo("ConfigurationHandler: Cheat Marshmallows set to: " + ConfigCheatExtraMarshmallows.Value);
        
        ConfigMenuKey = _config.Bind
        (
            "General",
            "Config Menu Key",
            "<Keyboard>/f2",
            "Control path for opening the mod configuration menu (e.g. <Keyboard>/f2, <Keyboard>/space, <Keyboard>/escape)"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Config Menu Key: " + ConfigMenuKey.Value);
        SetupInputAction();
        ConfigMenuKey.SettingChanged += OnMenuKeyChanged;
        
        ConfigCheatExtraBackpacks = _config.Bind
        (
            "General",
            "Cheat Backpacks",
            0,
            "(Cheat, disabled by default) Sets how many backpacks will spawn as a cheat, requires ExtraBackpacks to also be enabled. Capped at 10."
        );
        if (ConfigCheatExtraBackpacks.Value > 10)
        {
            ConfigCheatExtraBackpacks.Value = 10;
        }
        else if (ConfigCheatExtraBackpacks.Value < 0)
        {
            ConfigCheatExtraBackpacks.Value = 0;
        }
        Plugin.Logger.LogInfo("ConfigurationHandler: Cheat Backpacks set to: " + ConfigCheatExtraBackpacks.Value);
        Plugin.Logger.LogInfo("ConfigurationHandler initialised");
    }
    
    private void OnMaxPlayersChanged(object sender, System.EventArgs e)
    {
        NetworkConnector.MAX_PLAYERS = MaxPlayers;
        Plugin.Logger.LogInfo($"Set the Max Players to " + NetworkConnector.MAX_PLAYERS + "!");
    }

    private void OnMenuKeyChanged(object sender, System.EventArgs e)
    {
        SetupInputAction();
    }
    
    private void SetupInputAction()
    {
        MenuAction?.Dispose();

        MenuAction = new InputAction(type: InputActionType.Button);
        MenuAction.AddBinding(ConfigMenuKey.Value);
        MenuAction.Enable();
    }
}