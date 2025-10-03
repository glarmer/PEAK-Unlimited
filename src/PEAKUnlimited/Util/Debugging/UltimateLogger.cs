using System;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;

namespace PEAKUnlimited.Util.Debugging;

/// <summary>
/// Utility that allows for dynamic logging messages within the mod. The logger will determine if a message should be
/// logged with the console based on the messages <see cref="DebugLogType"/>. 
/// </summary>
public class UltimateLogger
{
    static UltimateLogger _instance;
    HashSet<DebugLogType> logTypeMap = new();

    public static UltimateLogger GetInstance()
    {
        if (_instance == null)
        {
            _instance = new UltimateLogger();
        }
        return _instance;
    }

    /// <summary>
    /// Constructor that determines which <see cref="DebugLogType"/> messages should be logged, based on config values.
    /// 
    /// Developer note: Is private to ensure singleton pattern is respected. 
    /// </summary>
    UltimateLogger()
    {
        string logTypesString = Plugin.ConfigurationHandler.VisibleLogTypes;
        string[] logTypesNames = logTypesString.Split(new[] { ", " }, StringSplitOptions.None);
        
        foreach (var logTypeName in logTypesNames)
        {
            if (Enum.TryParse<DebugLogType>(logTypeName, ignoreCase: true, out var logType))
            {
                logTypeMap.Add(logType);
            }
            else
            {
                Console.WriteLine($"Invalid enum name: {logTypeName}");
            }
        }
        
    }

    /// <summary>
    /// Logs a message with the Ultimate Logger with the specified Log Level and Log Type. Will only log messages to the
    /// console if the log type is set in the mod's config file.
    /// </summary>
    /// <param name="logLevel">The log level of the message</param>
    /// <param name="logType">The type of log message</param>
    /// <param name="message">The contents of the message</param>
    public void DebugMessage(LogLevel logLevel ,DebugLogType logType ,string message)
    {
        if (!logTypeMap.Contains(logType)) return;
        Plugin.Logger.Log(logLevel, $"[{logType.ToString()}]: {message}");
    }
}
