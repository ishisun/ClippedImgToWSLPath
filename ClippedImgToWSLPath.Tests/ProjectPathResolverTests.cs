namespace ClippedImgToWSLPath.Tests;

public class ProjectPathResolverTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly SettingsManager _settingsManager;

    public ProjectPathResolverTests()
    {
        // Create a unique test directory for each test run
        _testDirectory = Path.Combine(Path.GetTempPath(), $"ProjectPathResolverTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
        _settingsManager = new SettingsManager(_testDirectory);
    }

    public void Dispose()
    {
        // Cleanup test directory after each test
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    // ========== GetSavePath Tests ==========

    [Fact]
    public void GetSavePath_NormalMode_ReturnsSavePath()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = false;
        _settingsManager.SavePath = @"C:\ClipboardImages";
        var resolver = new ProjectPathResolver(_settingsManager);

        // Act
        var result = resolver.GetSavePath();

        // Assert
        Assert.Equal(@"C:\ClipboardImages", result);
    }

    [Fact]
    public void GetSavePath_ProjectMode_ReturnsProjectScreenshotsPath()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = true;
        _settingsManager.ProjectRootPath = @"C:\Projects\MyProject";
        _settingsManager.ProjectScreenshotsDir = "screenshots";
        var resolver = new ProjectPathResolver(_settingsManager);

        // Act
        var result = resolver.GetSavePath();

        // Assert - Path.Combine uses platform-specific separator
        var expected = Path.Combine(@"C:\Projects\MyProject", "screenshots");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSavePath_ProjectMode_WithCustomScreenshotsDir()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = true;
        _settingsManager.ProjectRootPath = @"C:\Projects\MyProject";
        _settingsManager.ProjectScreenshotsDir = "assets/images";
        var resolver = new ProjectPathResolver(_settingsManager);

        // Act
        var result = resolver.GetSavePath();

        // Assert - Path.Combine uses platform-specific separator
        var expected = Path.Combine(@"C:\Projects\MyProject", "assets/images");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSavePath_ProjectModeEnabled_ButNoProjectRootPath_FallsBackToSavePath()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = true;
        _settingsManager.ProjectRootPath = string.Empty;
        _settingsManager.SavePath = @"C:\Fallback";
        var resolver = new ProjectPathResolver(_settingsManager);

        // Act
        var result = resolver.GetSavePath();

        // Assert
        Assert.Equal(@"C:\Fallback", result);
    }

    // ========== GetClipboardPath Tests ==========

    [Fact]
    public void GetClipboardPath_NormalMode_ReturnsWSLPath()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = false;
        var resolver = new ProjectPathResolver(_settingsManager);
        var filePath = @"C:\ClipboardImages\clipboard_20250111_123456.png";

        // Act
        var result = resolver.GetClipboardPath(filePath);

        // Assert
        Assert.Equal("/mnt/c/ClipboardImages/clipboard_20250111_123456.png", result);
    }

    [Fact]
    public void GetClipboardPath_ProjectMode_ReturnsRelativePath()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = true;
        _settingsManager.ProjectRootPath = @"C:\Projects\MyProject";
        _settingsManager.ProjectScreenshotsDir = "screenshots";
        var resolver = new ProjectPathResolver(_settingsManager);
        var filePath = @"C:\Projects\MyProject\screenshots\clipboard_20250111_123456.png";

        // Act
        var result = resolver.GetClipboardPath(filePath);

        // Assert
        Assert.Equal("screenshots/clipboard_20250111_123456.png", result);
    }

    [Fact]
    public void GetClipboardPath_ProjectMode_WithNestedScreenshotsDir()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = true;
        _settingsManager.ProjectRootPath = @"C:\Projects\MyProject";
        _settingsManager.ProjectScreenshotsDir = "docs/images";
        var resolver = new ProjectPathResolver(_settingsManager);
        var filePath = @"C:\Projects\MyProject\docs\images\clipboard_20250111_123456.png";

        // Act
        var result = resolver.GetClipboardPath(filePath);

        // Assert
        Assert.Equal("docs/images/clipboard_20250111_123456.png", result);
    }

    [Fact]
    public void GetClipboardPath_ProjectModeEnabled_ButNoProjectRootPath_FallsBackToWSLPath()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = true;
        _settingsManager.ProjectRootPath = string.Empty;
        var resolver = new ProjectPathResolver(_settingsManager);
        var filePath = @"D:\Images\clipboard_20250111_123456.png";

        // Act
        var result = resolver.GetClipboardPath(filePath);

        // Assert
        Assert.Equal("/mnt/d/Images/clipboard_20250111_123456.png", result);
    }

    // ========== IsProjectModeActive Tests ==========

    [Fact]
    public void IsProjectModeActive_WhenEnabledAndValidPath_ReturnsTrue()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = true;
        _settingsManager.ProjectRootPath = @"C:\Projects\MyProject";
        var resolver = new ProjectPathResolver(_settingsManager);

        // Act & Assert
        Assert.True(resolver.IsProjectModeActive());
    }

    [Fact]
    public void IsProjectModeActive_WhenDisabled_ReturnsFalse()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = false;
        _settingsManager.ProjectRootPath = @"C:\Projects\MyProject";
        var resolver = new ProjectPathResolver(_settingsManager);

        // Act & Assert
        Assert.False(resolver.IsProjectModeActive());
    }

    [Fact]
    public void IsProjectModeActive_WhenEnabledButEmptyPath_ReturnsFalse()
    {
        // Arrange
        _settingsManager.ProjectModeEnabled = true;
        _settingsManager.ProjectRootPath = string.Empty;
        var resolver = new ProjectPathResolver(_settingsManager);

        // Act & Assert
        Assert.False(resolver.IsProjectModeActive());
    }
}
