using System.Drawing;
using System.Runtime.InteropServices;

namespace ClippedImgToWSLPath.Tests;

/// <summary>
/// Tests for ImageHashCalculator.
/// Note: These tests only run on Windows because System.Drawing.Common is Windows-only.
/// </summary>
public class ImageHashCalculatorTests
{
    private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    [SkippableFact]
    public void ComputeHash_WithSameImage_ReturnsSameHash()
    {
        Skip.IfNot(IsWindows, "System.Drawing.Common is only supported on Windows");

        // Arrange
        var calculator = new ImageHashCalculator();
        using var image1 = CreateTestImage(100, 100, Color.Red);
        using var image2 = CreateTestImage(100, 100, Color.Red);

        // Act
        var hash1 = calculator.ComputeHash(image1);
        var hash2 = calculator.ComputeHash(image2);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [SkippableFact]
    public void ComputeHash_WithDifferentImages_ReturnsDifferentHashes()
    {
        Skip.IfNot(IsWindows, "System.Drawing.Common is only supported on Windows");

        // Arrange
        var calculator = new ImageHashCalculator();
        using var image1 = CreateTestImage(100, 100, Color.Red);
        using var image2 = CreateTestImage(100, 100, Color.Blue);

        // Act
        var hash1 = calculator.ComputeHash(image1);
        var hash2 = calculator.ComputeHash(image2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [SkippableFact]
    public void ComputeHash_WithDifferentSizes_ReturnsDifferentHashes()
    {
        Skip.IfNot(IsWindows, "System.Drawing.Common is only supported on Windows");

        // Arrange
        var calculator = new ImageHashCalculator();
        using var image1 = CreateTestImage(100, 100, Color.Red);
        using var image2 = CreateTestImage(200, 200, Color.Red);

        // Act
        var hash1 = calculator.ComputeHash(image1);
        var hash2 = calculator.ComputeHash(image2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [SkippableFact]
    public void ComputeHash_ReturnsValidSha256Format()
    {
        Skip.IfNot(IsWindows, "System.Drawing.Common is only supported on Windows");

        // Arrange
        var calculator = new ImageHashCalculator();
        using var image = CreateTestImage(50, 50, Color.Green);

        // Act
        var hash = calculator.ComputeHash(image);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length); // SHA256 produces 64 hex characters
        Assert.Matches("^[a-f0-9]+$", hash); // Only lowercase hex characters
    }

    private static Bitmap CreateTestImage(int width, int height, Color color)
    {
        var bitmap = new Bitmap(width, height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(color);
        }
        return bitmap;
    }
}
