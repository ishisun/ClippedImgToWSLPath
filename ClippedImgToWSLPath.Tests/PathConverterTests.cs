namespace ClippedImgToWSLPath.Tests;

public class PathConverterTests
{
    [Theory]
    [InlineData(@"C:\Users\test\file.png", "/mnt/c/Users/test/file.png")]
    [InlineData(@"D:\Projects\image.jpg", "/mnt/d/Projects/image.jpg")]
    [InlineData(@"c:\lowercase\path.txt", "/mnt/c/lowercase/path.txt")]
    [InlineData(@"E:\", "/mnt/e/")]
    public void ConvertToWSLPath_WithDriveLetter_ReturnsCorrectPath(string windowsPath, string expectedWslPath)
    {
        // Arrange
        var converter = new PathConverter();

        // Act
        var result = converter.ConvertToWSLPath(windowsPath);

        // Assert
        Assert.Equal(expectedWslPath, result);
    }

    [Theory]
    [InlineData(@"C:\Path With Spaces\file.png", "/mnt/c/Path With Spaces/file.png")]
    [InlineData(@"D:\日本語\ファイル.png", "/mnt/d/日本語/ファイル.png")]
    public void ConvertToWSLPath_WithSpecialCharacters_ReturnsCorrectPath(string windowsPath, string expectedWslPath)
    {
        // Arrange
        var converter = new PathConverter();

        // Act
        var result = converter.ConvertToWSLPath(windowsPath);

        // Assert
        Assert.Equal(expectedWslPath, result);
    }

    [Fact]
    public void ConvertToWSLPath_WithForwardSlashes_ReturnsCorrectPath()
    {
        // Arrange
        var converter = new PathConverter();
        var windowsPath = "C:/Users/test/file.png";

        // Act
        var result = converter.ConvertToWSLPath(windowsPath);

        // Assert
        Assert.Equal("/mnt/c/Users/test/file.png", result);
    }

    [Theory]
    [InlineData("relative/path/file.png", "relative/path/file.png")]
    [InlineData("file.png", "file.png")]
    public void ConvertToWSLPath_WithRelativePath_ReturnsPathWithForwardSlashes(string inputPath, string expectedPath)
    {
        // Arrange
        var converter = new PathConverter();

        // Act
        var result = converter.ConvertToWSLPath(inputPath);

        // Assert
        Assert.Equal(expectedPath, result);
    }
}
