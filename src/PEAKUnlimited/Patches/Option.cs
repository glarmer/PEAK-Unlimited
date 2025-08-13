using BepInEx.Configuration;

namespace PEAKUnlimited.Patches;

public class Option
{
    public enum OptionType { Bool, Int }
    
    public string Label;
    public OptionType Type;
    public ConfigEntry<bool> BoolEntry;
    public ConfigEntry<int> IntEntry;
    public int MinInt, MaxInt, Step;

    private Option(string label, OptionType type)
    {
        Label = label;
        Type = type;
    }

    public static Option Bool(string label, ConfigEntry<bool> entry)
    {
        return new Option(label, OptionType.Bool) { BoolEntry = entry };
    }

    public static Option Int(string label, ConfigEntry<int> entry, int min, int max, int step = 1)
    {
        return new Option(label, OptionType.Int)
        {
            IntEntry = entry,
            MinInt = min,
            MaxInt = max,
            Step = step
        };
    }
}