using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace ClippedImgToWSLPath;

/// <summary>
/// Calculates SHA256 hash of images for deduplication purposes.
/// </summary>
public class ImageHashCalculator
{
    /// <summary>
    /// Computes a SHA256 hash of the given image.
    /// </summary>
    /// <param name="image">The image to hash</param>
    /// <returns>A lowercase hexadecimal string representing the SHA256 hash</returns>
    public string ComputeHash(Image image)
    {
        using var memoryStream = new MemoryStream();
        image.Save(memoryStream, ImageFormat.Png);
        var bytes = memoryStream.ToArray();

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(bytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
