using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Jellyfin.Plugin.JellyNews.Configuration;
using Jellyfin.Plugin.JellyNews.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Jellyfin.Plugin.JellyNews
{
    /// <summary>
    /// The main plugin.
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        private readonly ILogger<Plugin> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILibraryManager _libraryManager;
        private readonly ScanLibraryTask _scanLibraryTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="libraryManager">The library manager.</param>
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILoggerFactory loggerFactory, ILibraryManager libraryManager)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            _loggerFactory = loggerFactory;
            _libraryManager = libraryManager;

            var logFilePath = Path.Combine(applicationPaths.LogDirectoryPath, "JellyNews.log");
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, formatProvider: CultureInfo.InvariantCulture)
                .CreateLogger();
            _loggerFactory.AddSerilog(logger);
            _logger = _loggerFactory.CreateLogger<Plugin>();

            _scanLibraryTask = new ScanLibraryTask(_loggerFactory.CreateLogger<ScanLibraryTask>(), _libraryManager);
        }

        /// <inheritdoc />
        public override string Name => "JellyNews";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("a7757465-2526-45fb-99c1-6464949dc189");

        /// <summary>
        /// Gets the current plugin instance.
        /// </summary>
        public static Plugin? Instance { get; private set; }

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
            return new[] { _scanLibraryTask };
        }
    }
}
