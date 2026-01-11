# ClippedImgToWSLPath

A Windows system tray application that monitors clipboard for images, saves them to disk, and converts the file path to WSL format.

## Quick Reference

### Build & Test Commands

```bash
# Build
dotnet build

# Run tests
dotnet test

# Build for release (Windows executable)
dotnet publish -r win-x64 -c Release
# Output: bin/Release/net8.0-windows/win-x64/publish/ClippedImgToWSLPath.exe
```

### Tech Stack

- .NET 8.0 (Windows)
- Windows Forms
- xUnit (testing)
- System.Drawing.Common

## Project Structure

```
ClippedImgToWSLPath/
├── MainForm.cs              # Main app logic, clipboard monitoring, system tray
├── PathConverter.cs         # Windows → WSL path conversion
├── ImageHashCalculator.cs   # SHA256 image hashing for deduplication
├── SettingsDialog.cs        # Settings dialog
├── Program.cs               # Entry point
├── ClippedImgToWSLPath.Tests/  # Unit tests
└── docs/                    # Documentation
```

### Key Classes

| Class | Purpose |
|-------|---------|
| `MainForm` | System tray app, clipboard polling (1s interval), image saving |
| `PathConverter` | Converts `C:\path` → `/mnt/c/path` |
| `ImageHashCalculator` | SHA256 hash for duplicate detection |

## Development Notes

### WSL Build Support

The project includes `EnableWindowsTargeting` in .csproj, allowing builds on WSL:
```xml
<EnableWindowsTargeting>true</EnableWindowsTargeting>
```

### Test Execution

- **PathConverter tests**: Run on all platforms
- **ImageHashCalculator tests**: Windows-only (skipped on WSL/Linux)

### Important Files

- `docs/reference/architecture-overview.md` - System architecture
- `docs/reference/feature-specification.md` - Feature details
- `docs/guides/development-setup.md` - Setup instructions

## Operating Principles

Follow the "AI Operating Principles" appended to each prompt via hooks.

## Language Requirements

- Write all code, comments, and documentation in **English**
- Respond to the user in **Japanese**

## Documentation

Follow the rules defined in `docs/_README_DOCS_RULES.md`:

- Check `docs/INDEX.md` before creating new documents
- Place documents in the correct directory based on lifecycle stage
- Update `docs/INDEX.md` when adding or moving documents
- No duplicate documents - consolidate when necessary
