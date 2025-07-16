#pragma warning disable SA1611, CS0162
using System;
using System.IO;
using Jellyfin.Plugin.JellyNews.Configuration;
using MediaBrowser.Common.Configuration;

namespace Jellyfin.Plugin.JellyNews.Logging
{
    /// <summary>
    /// Log level enum.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Trace log level.
        /// </summary>
        Trace,

        /// <summary>
        /// Debug log level.
        /// </summary>
        Debug,

        /// <summary>
        /// Information log level.
        /// </summary>
        Information,

        /// <summary>
        /// Warning log level.
        /// </summary>
        Warning,

        /// <summary>
        /// Error log level.
        /// </summary>
        Error,

        /// <summary>
        /// Critical log level.
        /// </summary>
        Critical
    }

    /// <summary>
    /// A logger class.
    /// </summary>
    public class Logger
    {
        private readonly PluginConfiguration _config;
        private readonly string _logFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger()
        {
            _config = Plugin.Instance!.Configuration;
            var logDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(logDir))
            {
                logDir = Path.Combine(Plugin.Instance.ApplicationPaths.DataPath, "plugins", "JellyNews");
            }

            _logFile = Path.Combine(logDir, "jellynews.log");
        }

        /// <summary>
        /// Log a message at the specified log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to log.</param>
        public void Log(LogLevel level, string message)
        {
            if (level < _config.LogLevel)
            {
                return;
            }

            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            Console.WriteLine(logMessage);
            File.AppendAllText(_logFile, logMessage + Environment.NewLine);
        }
    }
}
