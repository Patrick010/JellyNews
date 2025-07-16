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
        private readonly ILibraryManager _libraryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanLibraryTask"/> class.
        /// </summary>
        /// <param name="libraryManager">The library manager.</param>
        public ScanLibraryTask(ILibraryManager libraryManager)
        {
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
            Plugin.Log?.Information("ScanLibraryTask Started");
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
                    Plugin.Log?.Information("Found library: {Name} with Id: {Id}", folder.Name, folder.Id);

                    config.AvailableLibraries.Add(new LibraryInfo
                    {
                        Name = folder.Name,
                        Id = folder.Id.ToString(),
                        ContentType = folder.GetClientTypeName()
                    });

                    Plugin.Log?.Information("Added library {Name} to available libraries", folder.Name);
                }
            }

            Plugin.Instance?.UpdateConfiguration(config);
            Plugin.Log?.Information("ScanLibraryTask Finished");
            progress.Report(100);
            return Task.CompletedTask;
        }
    }
}
