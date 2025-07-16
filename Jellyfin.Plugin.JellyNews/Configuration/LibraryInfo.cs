namespace Jellyfin.Plugin.JellyNews.Configuration
{
    /// <summary>
    /// A class to hold information about a library.
    /// </summary>
    public class LibraryInfo
    {
        /// <summary>
        /// Gets or sets the name of the library.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the library.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the content type of the library.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the library is selected.
        /// </summary>
        public bool Selected { get; set; }
    }
}
