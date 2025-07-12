using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Jellyfin.Plugin.JellyNews.Configuration;
using Jellyfin.Plugin.JellyNews.LOGGER;
using Jellyfin.Plugin.JellyNews.ScheduledTasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Library;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.JellyNews;

/// <summary>
/// The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly ILibraryManager _libraryManager;
    private readonly ITaskManager _taskManager;
    private Logger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
    /// <param name="taskManager">Instance of the <see cref="ITaskManager"/> interface.</param>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILibraryManager libraryManager, ITaskManager taskManager)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _libraryManager = libraryManager;
        _taskManager = taskManager;
        logger = new Logger();
        logger.Info("JellyNews Plugin constructor called.");

        void SetConfigPaths(IApplicationPaths dataPaths)
        {
            // custom code
            // IApplication Paths
            PluginConfiguration config = Plugin.Instance!.Configuration;
            config.DataPath = dataPaths.DataPath;
            logger.Info($"DataPath: {config.DataPath}");
            config.TempDirectory = dataPaths.TempDirectory;
            logger.Info($"TempDirectory: {config.TempDirectory}");
            config.PluginsPath = dataPaths.PluginsPath;
            logger.Info($"PluginsPath: {config.PluginsPath}");
            config.ProgramDataPath = dataPaths.ProgramDataPath;
            logger.Info($"ProgramDataPath: {config.ProgramDataPath}");
            config.ProgramSystemPath = dataPaths.ProgramSystemPath;
            logger.Info($"ProgramSystemPath: {config.ProgramSystemPath}");
            config.SystemConfigurationFilePath = dataPaths.SystemConfigurationFilePath;
            logger.Info($"SystemConfigurationFilePath: {config.SystemConfigurationFilePath}");
            config.LogDirectoryPath = dataPaths.LogDirectoryPath;
            logger.Info($"LogDirectoryPath: {config.LogDirectoryPath}");

            // Custom Paths
            config.NewsletterDir = $"{config.TempDirectory}/JellyNews/";
            logger.Info($"NewsletterDir: {config.NewsletterDir}");
        }

        SetConfigPaths(applicationPaths);

        _taskManager.CreateTask<ScanLibraryTask>();
        _taskManager.CreateTask<EmailNewsletterTask>();

        logger.Info("JellyNews Plugin constructor finished.");
    }

    /// <inheritdoc />
    public override string Name => "JellyNews";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("f7c2c9df-9c38-4421-9d8e-1f6ae91df2ad");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        logger.Info("GetPages method called.");
        return new[]
        {
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html",
                EnableInMainMenu = false
            }
        };
    }
}
