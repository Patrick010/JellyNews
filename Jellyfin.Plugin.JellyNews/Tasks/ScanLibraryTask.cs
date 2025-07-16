using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JellyNews.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JellyNews.Tasks
{
    /// <summary>
    /// A scheduled task to scan the libraries.
    /// </summary>
    public class ScanLibraryTask : IScheduledTask
    {
        private readonly ILogger<ScanLibraryTask> _logger;
        private readonly ILibraryManager _libraryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanLibraryTask"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="libraryManager">The library manager.</param>
        public ScanLibraryTask(ILogger<ScanLibraryTask> logger, ILibraryManager libraryManager)
        {
            _logger = logger;
            _libraryManager = libraryManager;
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
            _logger.LogInformation("ScanLibraryTask Started");
            var libraries = _libraryManager.GetUserRootFolder().Children.ToArray();
            var config = Plugin.Instance?.Configuration;
            if (config == null)
            {
                return Task.CompletedTask;
            }

            config.AvailableLibraries.Clear();
            foreach (var library in libraries)
            {
                if (library is Folder folder)
                {
                    _logger.LogInformation("Found library: {Name} with Id: {Id}", folder.Name, folder.Id);

                    config.AvailableLibraries.Add(new LibraryInfo
                    {
                        Name = folder.Name,
                        Id = folder.Id.ToString(),
                        ContentType = folder.GetClientTypeName()
                    });

                    _logger.LogInformation("Added library {Name} to available libraries", folder.Name);
                }
            }

            Plugin.Instance?.UpdateConfiguration(config);
            _logger.LogInformation("ScanLibraryTask Finished");
            progress.Report(100);
            return Task.CompletedTask;
        }
    }
}
