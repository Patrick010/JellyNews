using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JellyNews.Configuration;
using Jellyfin.Plugin.JellyNews.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.JellyNews.Tasks
{
    /// <summary>
    /// A scheduled task to scan the libraries.
    /// </summary>
    public class ScanLibraryTask : IScheduledTask
    {
        private readonly ILibraryManager _libraryManager;
        private readonly Logger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanLibraryTask"/> class.
        /// </summary>
        /// <param name="libraryManager">The library manager.</param>
        public ScanLibraryTask(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
            _logger = new Logger();
        }

        /// <inheritdoc />
        public string Name => "Scan Libraries";

        /// <inheritdoc />
        public string Key => "ScanLibraries";

        /// <inheritdoc />
        public string Description => "Scans the libraries to get all viewable libraries for the user to select from.";

        /// <inheritdoc />
        public string Category => "JellyNews";

        /// <inheritdoc />
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo
                {
                    Type = TaskTriggerInfo.TriggerStartup
                }
            };
        }

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, "ScanLibraryTask Started");
            var config = Plugin.Instance?.Configuration;
            if (config == null)
            {
                _logger.Log(LogLevel.Warning, "Plugin configuration is null. Skipping library scan.");
                return Task.CompletedTask;
            }

            var existingLibraries = config.AvailableLibraries.Where(l => l.Id != null).ToDictionary(l => l.Id!);
            config.AvailableLibraries.Clear();

            var libraries = _libraryManager.GetUserRootFolder().Children.ToArray();
            foreach (var library in libraries)
            {
                if (library is Folder folder)
                {
                    if (existingLibraries.TryGetValue(folder.Id.ToString(), out var existingLibrary))
                    {
                        config.AvailableLibraries.Add(existingLibrary);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, $"Found new library: {folder.Name} with Id: {folder.Id}");
                        config.AvailableLibraries.Add(new LibraryInfo
                        {
                            Name = folder.Name,
                            Id = folder.Id.ToString(),
                            ContentType = folder.GetClientTypeName(),
                            Selected = false
                        });
                        _logger.Log(LogLevel.Information, $"Added library {folder.Name} to available libraries");
                    }
                }
            }

            Plugin.Instance?.UpdateConfiguration(config);
            _logger.Log(LogLevel.Information, "ScanLibraryTask Finished");
            progress.Report(100);
            return Task.CompletedTask;
        }
    }
}
