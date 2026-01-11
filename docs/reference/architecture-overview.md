# Architecture Overview

## Overview

ClippedImgToWSLPath is a Windows Forms system tray application that monitors the clipboard for images, saves them to disk, and converts the file path to WSL format.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    Windows System                            │
│  ┌─────────────┐     ┌──────────────────────────────────┐   │
│  │  Clipboard  │────▶│       ClippedImgToWSLPath        │   │
│  └─────────────┘     │                                  │   │
│                      │  ┌────────────────────────────┐  │   │
│                      │  │        MainForm            │  │   │
│                      │  │  - Clipboard monitoring    │  │   │
│                      │  │  - Image detection         │  │   │
│                      │  │  - Image saving            │  │   │
│                      │  └────────────────────────────┘  │   │
│                      │               │                  │   │
│                      │      ┌────────┴────────┐         │   │
│                      │      ▼                 ▼         │   │
│                      │  ┌─────────────┐ ┌─────────────┐ │   │
│                      │  │PathConverter│ │ImageHash   │ │   │
│                      │  │             │ │Calculator  │ │   │
│                      │  └─────────────┘ └─────────────┘ │   │
│                      │          │                       │   │
│                      │          ▼                       │   │
│                      │  ┌─────────────────────────────┐ │   │
│                      │  │   ProjectPathResolver       │ │   │
│                      │  │  - Path mode resolution     │ │   │
│                      │  └─────────────────────────────┘ │   │
│                      │                                  │   │
│                      │  ┌────────────────────────────┐  │   │
│                      │  │     SettingsDialog         │  │   │
│                      │  │  - Save path configuration │  │   │
│                      │  │  - Project Mode settings   │  │   │
│                      │  └────────────────────────────┘  │   │
│                      │                                  │   │
│                      │  ┌────────────────────────────┐  │   │
│                      │  │      System Tray           │  │   │
│                      │  │  - NotifyIcon              │  │   │
│                      │  │  - Context menu            │  │   │
│                      │  └────────────────────────────┘  │   │
│                      └──────────────────────────────────┘   │
│                                    │                         │
│                                    ▼                         │
│                      ┌──────────────────────────────────┐   │
│                      │     ClipboardImages/             │   │
│                      │     (Saved PNG files)            │   │
│                      └──────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. MainForm (MainForm.cs)

The main application form that runs hidden in the background.

**Responsibilities:**
- Clipboard monitoring via AddClipboardFormatListener API (event-driven)
- Image detection and deduplication coordination
- Image saving to PNG format
- System tray icon management
- Balloon notifications

**Key Methods:**
| Method | Description |
|--------|-------------|
| `ProcessClipboardChange()` | Handles WM_CLIPBOARDUPDATE message for new images |
| `GetImageHash()` | Delegates to ImageHashCalculator |
| `SaveImageAndConvertPath()` | Saves image and converts path to WSL format |
| `ConvertToWSLPath()` | Delegates to PathConverter |

### 2. PathConverter (PathConverter.cs)

Utility class for converting Windows paths to WSL paths.

**Responsibilities:**
- Convert Windows drive paths (e.g., `C:\...`) to WSL mount paths (e.g., `/mnt/c/...`)
- Replace backslashes with forward slashes

**Key Methods:**
| Method | Description |
|--------|-------------|
| `ConvertToWSLPath(string)` | Converts a Windows path to WSL format |

### 3. ImageHashCalculator (ImageHashCalculator.cs)

Utility class for calculating image hashes.

**Responsibilities:**
- Generate SHA256 hash of images for deduplication

**Key Methods:**
| Method | Description |
|--------|-------------|
| `ComputeHash(Image)` | Returns SHA256 hash as lowercase hex string |

### 4. ProjectPathResolver (ProjectPathResolver.cs)

Utility class for resolving save and clipboard paths based on the current mode.

**Responsibilities:**
- Determine if Project Mode is active
- Resolve save path (normal save path or project screenshots directory)
- Resolve clipboard path (WSL path or relative path)

**Key Methods:**
| Method | Description |
|--------|-------------|
| `IsProjectModeActive()` | Returns true if Project Mode is enabled and configured |
| `GetSavePath()` | Returns the directory path for saving images |
| `GetClipboardPath(string)` | Returns the path to copy to clipboard |

### 5. SettingsDialog (SettingsDialog.cs)

Modal dialog for configuring application settings.

**Responsibilities:**
- Save path configuration
- Project Mode configuration
- Path validation

### 6. IconGenerator (IconGenerator.cs)

Utility for icon generation (if needed).

## Data Flow

```
1. User copies image to clipboard
         │
         ▼
2. WM_CLIPBOARDUPDATE message received
         │
         ▼
3. Generate hash of image (via ImageHashCalculator)
         │
         ▼
4. Compare with last known hash
         │
         ├── Same hash → Skip (already processed)
         │
         └── New hash → Continue
                  │
                  ▼
5. Save image as PNG to ClipboardImages/
         │
         ▼
6. Convert Windows path to WSL path (via PathConverter)
         │
         ▼
7. Copy WSL path to clipboard
         │
         ▼
8. Show balloon notification
```

## File Structure

```
ClippedImgToWSLPath/
├── ClippedImgToWSLPath.sln       # Solution file
├── ClippedImgToWSLPath.csproj    # Main project
├── MainForm.cs                   # Main application logic
├── PathConverter.cs              # Path conversion utility
├── ImageHashCalculator.cs        # Image hash utility
├── ProjectPathResolver.cs        # Path mode resolution
├── SettingsManager.cs            # Settings persistence
├── SettingsDialog.cs             # Settings dialog
├── Program.cs                    # Application entry point
├── IconGenerator.cs              # Icon utilities
├── icon.ico                      # Application icon
├── ClippedImgToWSLPath.Tests/    # Test project
│   ├── PathConverterTests.cs     # Path converter tests
│   ├── ImageHashCalculatorTests.cs # Image hash tests
│   ├── SettingsManagerTests.cs   # Settings manager tests
│   └── ProjectPathResolverTests.cs # Project path resolver tests
├── docs/                         # Documentation
│   ├── reference/                # Current specifications
│   ├── guides/                   # How-to guides
│   └── status/                   # Status information
└── ClipboardImages/              # Default image save location (runtime)
```

## Dependencies

- .NET 8.0 (Windows)
- Windows Forms
- System.Drawing.Common (NuGet package)

### Test Dependencies

- xUnit
- Xunit.SkippableFact (for platform-conditional tests)

## Platform Requirements

- Windows 10/11
- WSL/WSL2 (for using converted paths)

## Testing

The project includes unit tests for core utility classes:

- **PathConverterTests**: Tests for Windows to WSL path conversion
- **ImageHashCalculatorTests**: Tests for image hashing (Windows-only, skipped on other platforms)

Run tests with:
```bash
dotnet test
```
