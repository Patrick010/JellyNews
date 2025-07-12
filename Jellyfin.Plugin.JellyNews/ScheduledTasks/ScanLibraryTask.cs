#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JellyNews.Scripts.SCRAPER;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.JellyNews.ScheduledTasks
{
    /// <summary>
    /// Class RefreshMediaLibraryTask.
    /// </summary>
    public class ScanLibraryTask : IScheduledTask
    {
        private readonly ILibraryManager _libraryManager;

        public ScanLibraryTask(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        /// <inheritdoc />
        public string Name => "Filesystem Scraper";

        /// <inheritdoc />
        public string Description => "Gather info on recently added media and store it for JellyNews";

        /// <inheritdoc />
        public string Category => "JellyNews";

        /// <inheritdoc />
        public string Key => "EmailJellyNews";

        /// <summary>
        /// Creates the triggers that define when the task will run.
        /// </summary>
        /// <returns>IEnumerable{BaseTaskTrigger}.</returns>
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            yield return new TaskTriggerInfo
            {
                Type = TaskTriggerInfo.TriggerInterval,
                IntervalTicks = TimeSpan.FromHours(1).Ticks
            };
        }

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress.Report(0);

            Scraper myScraper = new Scraper(_libraryManager, progress, cancellationToken);
            return myScraper.GetSeriesData();
        }
    }
}