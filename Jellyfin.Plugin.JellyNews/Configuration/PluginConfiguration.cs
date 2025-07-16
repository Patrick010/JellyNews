using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using Jellyfin.Plugin.JellyNews.Logging;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.JellyNews.Configuration
{
    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        private Collection<LibraryInfo> _availableLibraries;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            SelectedLibraryIds = new Collection<string>();
            _availableLibraries = new Collection<LibraryInfo>();
            LogLevel = LogLevel.Debug;
        }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets the selected library ids.
        /// </summary>
        public Collection<string> SelectedLibraryIds { get; }

        /// <summary>
        /// Gets or sets the available libraries as a JSON string.
        /// </summary>
        public string? AvailableLibrariesJson { get; set; }

        /// <summary>
        /// Gets the available libraries.
        /// </summary>
        public Collection<LibraryInfo> AvailableLibraries
        {
            get
            {
                if (!string.IsNullOrEmpty(AvailableLibrariesJson))
                {
                    _availableLibraries = JsonSerializer.Deserialize<Collection<LibraryInfo>>(AvailableLibrariesJson) ?? new Collection<LibraryInfo>();
                }

                return _availableLibraries;
            }
        }
    }
}
