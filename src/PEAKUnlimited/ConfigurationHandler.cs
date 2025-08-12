using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace PEAKUnlimited;

public class ConfigurationHandler
{
    private ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "PEAKUnlimited.cfg"), true);
    
    private ConfigEntry<int> _configMaxPlayers;
    private ConfigEntry<bool> _lockKiosk;
    private ConfigEntry<bool> _configExtraMarshmallows;
    private ConfigEntry<bool> _configLateMarshmallows;
    private ConfigEntry<int> _configCheatExtraMarshmallows;
    private ConfigEntry<bool> _configExtraBackpacks;
    private ConfigEntry<int> _configCheatExtraBackpacks;
    
    public int MaxPlayers => _configMaxPlayers.Value;
    public bool LockKiosk => _lockKiosk.Value;
    public bool IsExtraMarshmallowsEnabled => _configExtraMarshmallows.Value;
    public bool IsLateMarshmallowsEnabled => _configLateMarshmallows.Value;
    public int CheatMarshmallows => _configCheatExtraMarshmallows.Value;
    public bool IsExtraBackpacksEnabled => _configExtraBackpacks.Value;
    public int CheatBackpacks => _configCheatExtraBackpacks.Value;
    
    
    public ConfigurationHandler()
    {
        Plugin.Logger.LogInfo("ConfigurationHandler initialising");
        _configMaxPlayers = config.Bind
        (
            "General",
            "MaxPlayers",
            20,
            "The maximum number of players you want to be able to join your lobby (Including yourself). Warning: untested, higher numbers may be unstable! Range: 1-20"
        );
        if (_configMaxPlayers.Value == 0)
        {
            _configMaxPlayers.Value = 1;
        }
        else if (_configMaxPlayers.Value > 30)
        {
            _configMaxPlayers.Value = 30;
        }
        Plugin.Logger.LogInfo("ConfigurationHandler: Max Players Loaded: " + _configMaxPlayers.Value);
        
        _lockKiosk = config.Bind
        (
            "General",
            "LockKiosk",
            false,
            "Allows you to stop other players starting the game from the Airport Kiosk"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Lock Kiosk enabled: " + _lockKiosk.Value);
        
        _configExtraMarshmallows = config.Bind
        (
            "General",
            "ExtraMarshmallows",
            true,
            "Controls whether additional marshmallows are spawned for the extra players"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Extra marshmallows enabled: " + _configExtraMarshmallows.Value);
        
        _configExtraBackpacks = config.Bind
        (
            "General",
            "ExtraBackpacks",
            true,
            "Controls whether additional backpacks have a chance to be spawned for extra players"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Extra backpacks enabled: " + _configExtraBackpacks.Value);
        
        _configLateMarshmallows = config.Bind
        (
            "General",
            "LateJoinMarshmallows",
            false,
            "Controls whether additional marshmallows are spawned for players who join late (mid run), and removed for those who leave early (Experimental + Untested)"
        );
        Plugin.Logger.LogInfo("ConfigurationHandler: Late Marshmallows enabled: " + _configLateMarshmallows.Value);

        _configCheatExtraMarshmallows = config.Bind
        (
            "General",
            "Cheat Marshmallows",
            0,
            "(Cheat, disabled by default) This will set the desired amount of marshmallows to the campfires as a cheat, requires ExtraMarshmallows to be enabled. Capped at 30."
        );
        if (_configCheatExtraMarshmallows.Value > 30)
        {
            _configCheatExtraMarshmallows.Value = 30;
        }
        else if (_configCheatExtraMarshmallows.Value < 0)
        {
            _configCheatExtraMarshmallows.Value = 0;
        }
        Plugin.Logger.LogInfo("ConfigurationHandler: Cheat Marshmallows set to: " + _configCheatExtraMarshmallows.Value);
        
        _configCheatExtraBackpacks = config.Bind
        (
            "General",
            "Cheat Backpacks",
            0,
            "(Cheat, disabled by default) Sets how many backpacks will spawn as a cheat, requires ExtraBackpacks to also be enabled. Capped at 10."
        );
        if (_configCheatExtraBackpacks.Value > 10)
        {
            _configCheatExtraBackpacks.Value = 10;
        }
        else if (_configCheatExtraBackpacks.Value < 0)
        {
            _configCheatExtraBackpacks.Value = 0;
        }
        Plugin.Logger.LogInfo("ConfigurationHandler: Cheat Backpacks set to: " + _configCheatExtraBackpacks.Value);
        Plugin.Logger.LogInfo("ConfigurationHandler initialised");
    }
}