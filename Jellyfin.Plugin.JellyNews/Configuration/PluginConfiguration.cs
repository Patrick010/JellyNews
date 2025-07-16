using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jellyfin.Plugin.JellyNews.Logging;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.JellyNews.Configuration
{
    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            AvailableLibraries = new Collection<LibraryInfo>();
            LogLevel = LogLevel.Debug;
        }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets the available libraries.
        /// </summary>
        public Collection<LibraryInfo> AvailableLibraries { get; }
    }
}
