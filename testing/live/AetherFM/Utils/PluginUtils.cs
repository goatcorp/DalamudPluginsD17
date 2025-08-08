using Dalamud.Plugin.Services;
using System;

namespace AetherFM.Utils;

public static class PluginUtils
{
    /// <summary>
    /// Formats a timestamp in a readable format
    /// </summary>
    /// <param name="timestamp">Timestamp to format</param>
    /// <returns>Formatted string</returns>
    public static string FormatTimestamp(DateTime timestamp)
    {
        return timestamp.ToString("HH:mm:ss");
    }

    /// <summary>
    /// Calculates the distance between two positions
    /// </summary>
    /// <param name="pos1">First position</param>
    /// <param name="pos2">Second position</param>
    /// <returns>Distance in game units</returns>
    public static float CalculateDistance(System.Numerics.Vector3 pos1, System.Numerics.Vector3 pos2)
    {
        return System.Numerics.Vector3.Distance(pos1, pos2);
    }

    /// <summary>
    /// Logs a message with timestamp
    /// </summary>
    /// <param name="log">Log service</param>
    /// <param name="message">Message to log</param>
    public static void LogWithTimestamp(IPluginLog log, string message)
    {
        var timestamp = FormatTimestamp(DateTime.Now);
        var formattedMessage = $"[{timestamp}] {message}";
        log.Information(formattedMessage);
    }

    /// <summary>
    /// Logs a debug message with timestamp
    /// </summary>
    /// <param name="log">Log service</param>
    /// <param name="message">Message to log</param>
    public static void LogDebugWithTimestamp(IPluginLog log, string message)
    {
        var timestamp = FormatTimestamp(DateTime.Now);
        var formattedMessage = $"[{timestamp}] {message}";
        log.Debug(formattedMessage);
    }

    /// <summary>
    /// Logs an error message with timestamp
    /// </summary>
    /// <param name="log">Log service</param>
    /// <param name="message">Message to log</param>
    public static void LogErrorWithTimestamp(IPluginLog log, string message)
    {
        var timestamp = FormatTimestamp(DateTime.Now);
        var formattedMessage = $"[{timestamp}] {message}";
        log.Error(formattedMessage);
    }
} 