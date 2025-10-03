namespace PEAKUnlimited.Model.GameInfo;

/// <summary>
/// Interface representing the general functionality of a util class that can produce information about a specified
/// instance. Used to make logging of information about instances cleaner.
/// </summary>
/// <typeparam name="T">The Type Parameter of the instances to provide information for.</typeparam>
public interface IGameInfo<T>
{
    /// <summary>
    /// Gets a message that provides information about the specified instance.
    /// </summary>
    /// <param name="gameInstance">The instance to get information about</param>
    /// <returns>Information message regarding the instance</returns>
    string GetInfoMessage(T gameInstance);
}
