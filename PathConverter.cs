namespace ClippedImgToWSLPath;

/// <summary>
/// Converts Windows file paths to WSL-compatible paths.
/// </summary>
public class PathConverter
{
    /// <summary>
    /// Converts a Windows path to a WSL path.
    /// </summary>
    /// <param name="windowsPath">The Windows path to convert (e.g., "C:\Users\test\file.png")</param>
    /// <returns>The WSL-compatible path (e.g., "/mnt/c/Users/test/file.png")</returns>
    public string ConvertToWSLPath(string windowsPath)
    {
        // Replace backslashes with forward slashes
        string path = windowsPath.Replace('\\', '/');

        // Check if path starts with a drive letter (e.g., "C:" or "c:")
        if (path.Length >= 2 && path[1] == ':')
        {
            char driveLetter = char.ToLower(path[0]);
            path = "/mnt/" + driveLetter + path.Substring(2);
        }

        return path;
    }
}
