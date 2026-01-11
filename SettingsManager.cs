using System;
using System.IO;
using System.Text.Json;

namespace ClippedImgToWSLPath
{
    /// <summary>
    /// Manages application settings persistence using JSON file storage.
    /// </summary>
    public class SettingsManager
    {
        private readonly string _baseDirectory;
        private readonly string _settingsFilePath;

        /// <summary>
        /// Gets or sets the directory path where clipboard images are saved.
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// Gets or sets whether debug logging is enabled.
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets whether Project Mode is enabled.
        /// When enabled, images are saved to project directory and relative paths are copied to clipboard.
        /// </summary>
        public bool ProjectModeEnabled { get; set; }

        /// <summary>
        /// Gets or sets the project root directory path.
        /// Used when Project Mode is enabled.
        /// </summary>
        public string ProjectRootPath { get; set; }

        /// <summary>
        /// Gets or sets the subdirectory name within the project root where screenshots are saved.
        /// Default value is "screenshots".
        /// </summary>
        public string ProjectScreenshotsDir { get; set; }

        /// <summary>
        /// Initializes a new instance of the SettingsManager class.
        /// </summary>
        /// <param name="baseDirectory">The base directory for settings file and default save path.</param>
        public SettingsManager(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
            _settingsFilePath = Path.Combine(_baseDirectory, "settings.json");

            // Set default values
            SavePath = Path.Combine(_baseDirectory, "ClipboardImages");
            EnableLogging = false;
            ProjectModeEnabled = false;
            ProjectRootPath = string.Empty;
            ProjectScreenshotsDir = "screenshots";
        }

        /// <summary>
        /// Loads settings from the settings file. If the file does not exist or is invalid,
        /// default values are used.
        /// </summary>
        public void Load()
        {
            if (!File.Exists(_settingsFilePath))
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<SettingsData>(json);

                if (settings != null)
                {
                    if (!string.IsNullOrEmpty(settings.SavePath))
                    {
                        SavePath = settings.SavePath;
                    }
                    EnableLogging = settings.EnableLogging;
                    ProjectModeEnabled = settings.ProjectModeEnabled;
                    ProjectRootPath = settings.ProjectRootPath ?? string.Empty;
                    if (!string.IsNullOrEmpty(settings.ProjectScreenshotsDir))
                    {
                        ProjectScreenshotsDir = settings.ProjectScreenshotsDir;
                    }
                }
            }
            catch (JsonException)
            {
                // Invalid JSON, use default values
            }
            catch (Exception)
            {
                // Other errors, use default values
            }
        }

        /// <summary>
        /// Saves current settings to the settings file.
        /// </summary>
        public void Save()
        {
            try
            {
                var settings = new SettingsData
                {
                    SavePath = SavePath,
                    EnableLogging = EnableLogging,
                    ProjectModeEnabled = ProjectModeEnabled,
                    ProjectRootPath = ProjectRootPath,
                    ProjectScreenshotsDir = ProjectScreenshotsDir
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception)
            {
                // Failed to save settings, ignore
            }
        }

        /// <summary>
        /// Internal class for JSON serialization/deserialization.
        /// </summary>
        private class SettingsData
        {
            public string SavePath { get; set; } = string.Empty;
            public bool EnableLogging { get; set; }
            public bool ProjectModeEnabled { get; set; }
            public string ProjectRootPath { get; set; } = string.Empty;
            public string ProjectScreenshotsDir { get; set; } = "screenshots";
        }
    }
}
