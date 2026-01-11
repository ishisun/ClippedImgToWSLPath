using System.IO;

namespace ClippedImgToWSLPath
{
    /// <summary>
    /// Resolves file paths based on the current mode (normal or project mode).
    /// In normal mode, returns WSL paths.
    /// In project mode, returns relative paths from the project root.
    /// </summary>
    public class ProjectPathResolver
    {
        private readonly SettingsManager _settings;
        private readonly PathConverter _pathConverter;

        /// <summary>
        /// Initializes a new instance of the ProjectPathResolver class.
        /// </summary>
        /// <param name="settings">The settings manager containing path configuration.</param>
        public ProjectPathResolver(SettingsManager settings)
        {
            _settings = settings;
            _pathConverter = new PathConverter();
        }

        /// <summary>
        /// Determines if project mode is currently active.
        /// Project mode is active when it is enabled AND a valid project root path is set.
        /// </summary>
        /// <returns>True if project mode is active, false otherwise.</returns>
        public bool IsProjectModeActive()
        {
            return _settings.ProjectModeEnabled && !string.IsNullOrEmpty(_settings.ProjectRootPath);
        }

        /// <summary>
        /// Gets the directory path where images should be saved.
        /// </summary>
        /// <returns>
        /// In project mode: {ProjectRootPath}/{ProjectScreenshotsDir}
        /// In normal mode: SavePath
        /// </returns>
        public string GetSavePath()
        {
            if (IsProjectModeActive())
            {
                return Path.Combine(_settings.ProjectRootPath, _settings.ProjectScreenshotsDir);
            }

            return _settings.SavePath;
        }

        /// <summary>
        /// Gets the path to copy to the clipboard for the given file.
        /// </summary>
        /// <param name="filePath">The full path to the saved image file.</param>
        /// <returns>
        /// In project mode: Relative path from project root (e.g., "screenshots/image.png")
        /// In normal mode: WSL-formatted path (e.g., "/mnt/c/path/image.png")
        /// </returns>
        public string GetClipboardPath(string filePath)
        {
            if (IsProjectModeActive())
            {
                // Extract filename from the full path (handle both \ and / separators)
                var normalizedPath = filePath.Replace('\\', '/');
                var lastSlashIndex = normalizedPath.LastIndexOf('/');
                var fileName = lastSlashIndex >= 0 ? normalizedPath.Substring(lastSlashIndex + 1) : normalizedPath;

                // Return relative path using forward slashes
                var relativePath = _settings.ProjectScreenshotsDir.Replace('\\', '/');
                return $"{relativePath}/{fileName}";
            }

            return _pathConverter.ConvertToWSLPath(filePath);
        }
    }
}
