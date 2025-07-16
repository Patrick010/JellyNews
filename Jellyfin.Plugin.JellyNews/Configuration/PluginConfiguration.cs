using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            SelectedLibraryIds = new Collection<string>();
            AvailableLibraries = new Collection<LibraryInfo>();
            LoggingLevel = LogLevel.Debug;
        }

        /// <summary>
        /// Gets the selected library ids.
        /// </summary>
        public Collection<string> SelectedLibraryIds { get; }

        /// <summary>
        /// Gets the available libraries.
        /// </summary>
        public Collection<LibraryInfo> AvailableLibraries { get; }

        /// <summary>
        /// Gets or sets the logging level.
        /// </summary>
        public LogLevel LoggingLevel { get; set; }
    }
}
