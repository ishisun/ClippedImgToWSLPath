using System.Text.Json;

namespace ClippedImgToWSLPath.Tests;

public class SettingsManagerTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly string _settingsFilePath;

    public SettingsManagerTests()
    {
        // Create a unique test directory for each test run
        _testDirectory = Path.Combine(Path.GetTempPath(), $"SettingsManagerTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
        _settingsFilePath = Path.Combine(_testDirectory, "settings.json");
    }

    public void Dispose()
    {
        // Cleanup test directory after each test
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ReturnsDefaultValues()
    {
        // Arrange
        var manager = new SettingsManager(_testDirectory);

        // Act
        manager.Load();

        // Assert
        Assert.Equal(Path.Combine(_testDirectory, "ClipboardImages"), manager.SavePath);
        Assert.False(manager.EnableLogging);
    }

    [Fact]
    public void Load_WhenFileExists_ReturnsStoredValues()
    {
        // Arrange
        var expectedSavePath = @"C:\Custom\Path";
        var expectedLogging = true;
        var settingsJson = JsonSerializer.Serialize(new
        {
            SavePath = expectedSavePath,
            EnableLogging = expectedLogging
        });
        File.WriteAllText(_settingsFilePath, settingsJson);

        var manager = new SettingsManager(_testDirectory);

        // Act
        manager.Load();

        // Assert
        Assert.Equal(expectedSavePath, manager.SavePath);
        Assert.True(manager.EnableLogging);
    }

    [Fact]
    public void Load_WhenFileIsInvalidJson_ReturnsDefaultValues()
    {
        // Arrange
        File.WriteAllText(_settingsFilePath, "{ invalid json }");
        var manager = new SettingsManager(_testDirectory);

        // Act
        manager.Load();

        // Assert
        Assert.Equal(Path.Combine(_testDirectory, "ClipboardImages"), manager.SavePath);
        Assert.False(manager.EnableLogging);
    }

    [Fact]
    public void Save_CreatesSettingsFile()
    {
        // Arrange
        var manager = new SettingsManager(_testDirectory);
        manager.SavePath = @"D:\TestPath";
        manager.EnableLogging = true;

        // Act
        manager.Save();

        // Assert
        Assert.True(File.Exists(_settingsFilePath));
    }

    [Fact]
    public void Save_AndLoad_PreservesValues()
    {
        // Arrange
        var expectedSavePath = @"E:\MyImages";
        var expectedLogging = true;

        var manager1 = new SettingsManager(_testDirectory);
        manager1.SavePath = expectedSavePath;
        manager1.EnableLogging = expectedLogging;

        // Act
        manager1.Save();

        var manager2 = new SettingsManager(_testDirectory);
        manager2.Load();

        // Assert
        Assert.Equal(expectedSavePath, manager2.SavePath);
        Assert.Equal(expectedLogging, manager2.EnableLogging);
    }

    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var manager = new SettingsManager(_testDirectory);

        // Assert
        Assert.Equal(Path.Combine(_testDirectory, "ClipboardImages"), manager.SavePath);
        Assert.False(manager.EnableLogging);
    }

    [Fact]
    public void Load_WhenFileHasPartialData_UsesDefaultsForMissingFields()
    {
        // Arrange
        var settingsJson = JsonSerializer.Serialize(new { SavePath = @"C:\Partial" });
        File.WriteAllText(_settingsFilePath, settingsJson);

        var manager = new SettingsManager(_testDirectory);

        // Act
        manager.Load();

        // Assert
        Assert.Equal(@"C:\Partial", manager.SavePath);
        Assert.False(manager.EnableLogging); // Default value for missing field
    }

    // ========== Project Mode Tests ==========

    [Fact]
    public void Constructor_SetsDefaultValues_ForProjectModeProperties()
    {
        // Arrange & Act
        var manager = new SettingsManager(_testDirectory);

        // Assert
        Assert.False(manager.ProjectModeEnabled);
        Assert.Equal(string.Empty, manager.ProjectRootPath);
        Assert.Equal("screenshots", manager.ProjectScreenshotsDir);
    }

    [Fact]
    public void Load_LegacySettings_UsesDefaultsForNewProperties()
    {
        // Arrange - Legacy settings file without Project Mode properties
        var settingsJson = JsonSerializer.Serialize(new
        {
            SavePath = @"C:\LegacyPath",
            EnableLogging = true
        });
        File.WriteAllText(_settingsFilePath, settingsJson);

        var manager = new SettingsManager(_testDirectory);

        // Act
        manager.Load();

        // Assert - Old properties loaded correctly
        Assert.Equal(@"C:\LegacyPath", manager.SavePath);
        Assert.True(manager.EnableLogging);
        // New properties use defaults
        Assert.False(manager.ProjectModeEnabled);
        Assert.Equal(string.Empty, manager.ProjectRootPath);
        Assert.Equal("screenshots", manager.ProjectScreenshotsDir);
    }

    [Fact]
    public void Load_WithNewSettings_ReturnsStoredValues()
    {
        // Arrange
        var settingsJson = JsonSerializer.Serialize(new
        {
            SavePath = @"C:\Custom\Path",
            EnableLogging = true,
            ProjectModeEnabled = true,
            ProjectRootPath = @"C:\Projects\MyProject",
            ProjectScreenshotsDir = "images"
        });
        File.WriteAllText(_settingsFilePath, settingsJson);

        var manager = new SettingsManager(_testDirectory);

        // Act
        manager.Load();

        // Assert
        Assert.Equal(@"C:\Custom\Path", manager.SavePath);
        Assert.True(manager.EnableLogging);
        Assert.True(manager.ProjectModeEnabled);
        Assert.Equal(@"C:\Projects\MyProject", manager.ProjectRootPath);
        Assert.Equal("images", manager.ProjectScreenshotsDir);
    }

    [Fact]
    public void Save_IncludesProjectModeProperties()
    {
        // Arrange
        var manager = new SettingsManager(_testDirectory);
        manager.SavePath = @"D:\TestPath";
        manager.EnableLogging = true;
        manager.ProjectModeEnabled = true;
        manager.ProjectRootPath = @"C:\Projects\Test";
        manager.ProjectScreenshotsDir = "screens";

        // Act
        manager.Save();

        // Assert
        var json = File.ReadAllText(_settingsFilePath);
        var savedSettings = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.Equal(@"D:\TestPath", savedSettings.GetProperty("SavePath").GetString());
        Assert.True(savedSettings.GetProperty("EnableLogging").GetBoolean());
        Assert.True(savedSettings.GetProperty("ProjectModeEnabled").GetBoolean());
        Assert.Equal(@"C:\Projects\Test", savedSettings.GetProperty("ProjectRootPath").GetString());
        Assert.Equal("screens", savedSettings.GetProperty("ProjectScreenshotsDir").GetString());
    }

    [Fact]
    public void Save_AndLoad_PreservesProjectModeValues()
    {
        // Arrange
        var manager1 = new SettingsManager(_testDirectory);
        manager1.ProjectModeEnabled = true;
        manager1.ProjectRootPath = @"E:\MyProject";
        manager1.ProjectScreenshotsDir = "assets/screenshots";

        // Act
        manager1.Save();

        var manager2 = new SettingsManager(_testDirectory);
        manager2.Load();

        // Assert
        Assert.True(manager2.ProjectModeEnabled);
        Assert.Equal(@"E:\MyProject", manager2.ProjectRootPath);
        Assert.Equal("assets/screenshots", manager2.ProjectScreenshotsDir);
    }
}
