using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Jellyfin.Plugin.JellyNews.Configuration;
using Jellyfin.Plugin.JellyNews.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Tasks;
using Serilog;
using Serilog.Events;

namespace Jellyfin.Plugin.JellyNews
{
    /// <summary>
    /// Root plugin class with its own Serilog instance that writes to jellynews.log.
    /// </summary>
    public sealed class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        /// <inheritdoc />
        public override string Name => "JellyNews";

        private readonly ILibraryManager? _libraryManager;
        private readonly ScanLibraryTask? _scanLibraryTask;
        private static readonly Serilog.Core.LoggingLevelSwitch _levelSwitch =
            new(LogEventLevel.Information);

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("a7757465-2526-45fb-99c1-6464949dc189");

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <param name="xmlSerializer">The xml serializer.</param>
        /// <param name="libraryManager">The library manager.</param>
        public Plugin(IApplicationPaths paths, IXmlSerializer xmlSerializer, ILibraryManager libraryManager) : base(paths, xmlSerializer)
        {
            try
            {
                Instance = this;
                _libraryManager = libraryManager;
                _scanLibraryTask = new ScanLibraryTask(_libraryManager);
                var logFile = Path.Combine(paths.LogDirectoryPath, "jellynews.log");

                Log = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(_levelSwitch)
                    .WriteTo.File(
                        logFile,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,
                        buffered: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(5),
                        formatProvider: CultureInfo.InvariantCulture)
                    .CreateLogger();

                Log.Information("JellyNews logger initialised → {LogFile}", logFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing JellyNews plugin: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the global logger for the entire plugin.
        /// </summary>
        internal static ILogger? Log { get; private set; }

        /// <summary>
        /// Gets the current plugin instance.
        /// </summary>
        public static Plugin? Instance { get; private set; }

        /// <summary>
        /// Allow admin UI or env‑var to flip verbosity: Debug, Information, Warning.
        /// </summary>
        /// <param name="level">The new log level.</param>
        public static void SetLogLevel(LogEventLevel level)
        {
            _levelSwitch.MinimumLevel = level;
            Log?.Information("Log level switched to {Level}", level);
        }

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = Name,
                    EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
                }
            };
        }

        /// <summary>
        /// Gets the scheduled tasks.
        /// </summary>
        /// <returns>The scheduled tasks.</returns>
        public IEnumerable<IScheduledTask> GetScheduledTasks()
        {
            return _scanLibraryTask is not null ? new[] { _scanLibraryTask } : Enumerable.Empty<IScheduledTask>();
        }
    }
}
