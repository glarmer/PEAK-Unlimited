namespace PEAKUnlimited.Util.Debugging;
/// <summary>
/// Enum that defines the types of log messages that can be displayed by the Mod. 
/// </summary>
public enum DebugLogType
{
    /// <summary>
    /// Defines logic that relates to the placement / alteration of marshmallows to be placed around
    /// <see cref="Campfire"/> instances.
    /// </summary>
    MarshmallowLogic,
    /// <summary>
    /// Defines logic that relates to alteration to the <see cref="Campfire"/> instances.
    /// </summary>
    CampfireLogic,
    
    /// <summary>
    /// Defines logic that relates to placement / alteration to the <see cref="Backpack"/> instances placed around a
    /// <see cref="Campfire"/>.
    /// </summary>
    BackpackLogic, 
    
    /// <summary>
    /// Defines logic that relates to the networking of the game, such as handling <see cref="Player"/> login / logout
    /// events.
    /// </summary>
    NetworkingLogic, 
    /// <summary>
    /// Defines logic that relates to the handling of level <see cref="Segment"/> instances.
    /// </summary>
    SegmentLogic, 
    /// <summary>
    /// Defines logic that relates to the status of game methods the mod is attempting to patch.
    /// </summary>
    PatchingLogic
}
