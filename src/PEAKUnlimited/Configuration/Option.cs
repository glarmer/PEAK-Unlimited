using System;
using BepInEx.Configuration;

namespace PEAKUnlimited.Patches;

public class Option
{
    public enum OptionType { Bool, Int }
    
    public string Label {get; set;}
    public OptionType Type {get; set;}
    public ConfigEntry<bool> BoolEntry {get; set;}
    public ConfigEntry<int> IntEntry {get; set;}
    public int MinInt  {get; set;}
    public int MaxInt  {get; set;}
    public int Step {get; set;}
    public Func<bool> IsDisabled { get; set; } = () => false;

    private Option(string label, OptionType type)
    {
        Label = label;
        Type = type;
    }

    public static Option Bool(string label, ConfigEntry<bool> entry, Func<bool>? isDisabled = null)
    {
        return new Option(label, OptionType.Bool)
        {
            BoolEntry = entry,
            IsDisabled = isDisabled ?? (() => false)
        };
    }

    public static Option Int(string label, ConfigEntry<int> entry, int min, int max, int step = 1, Func<bool>? isDisabled = null)
    {
        return new Option(label, OptionType.Int)
        {
            IntEntry = entry,
            MinInt = min,
            MaxInt = max,
            Step = step,
            IsDisabled = isDisabled ?? (() => false)
        };
    }
}