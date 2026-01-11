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
}
