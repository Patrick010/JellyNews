#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JellyNews.Emails.EMAIL;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.JellyNews.ScheduledTasks
{
    /// <summary>
    /// Class RefreshMediaLibraryTask.
    /// </summary>
    public class EmailNewsletterTask : IScheduledTask
    {
        /// <inheritdoc />
        public string Name => "Email Newsletter";

        /// <inheritdoc />
        public string Description => "Email JellyNews";

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
                IntervalTicks = TimeSpan.FromHours(168).Ticks
            };
        }

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress.Report(0);

            var config = Plugin.Instance?.Configuration;
            if (config == null || string.IsNullOrEmpty(config.ToAddr))
            {
                // Optionally log that the email is not sent because the address is not configured
                return Task.CompletedTask;
            }

            Smtp mySmtp = new Smtp();
            mySmtp.SendEmail();
            progress.Report(100);
            return Task.CompletedTask;
        }
    }
}