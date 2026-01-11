# Feature Specification

## Overview

ClippedImgToWSLPath automatically saves clipboard images and provides WSL-compatible paths.

---

## Features

### 1. Clipboard Image Monitoring

**Description:** Continuously monitors the Windows clipboard for image content.

**Behavior:**
- Uses AddClipboardFormatListener API for event-driven clipboard monitoring
- Receives WM_CLIPBOARDUPDATE messages when clipboard content changes
- Detects images copied from any application
- Uses SHA256 hashing to detect duplicate images
- Skips processing if the same image is still in clipboard

### 2. Automatic Image Saving

**Description:** Saves detected images to a local directory.

**Behavior:**
- Saves images in PNG format
- Filename format: `clipboard_YYYYMMDD_HHmmss.png`
- Default save location: `<application_directory>/ClipboardImages/`
- Creates directory if it doesn't exist

**Configurable:** Save path can be changed via Settings dialog

### 3. WSL Path Conversion

**Description:** Converts Windows file paths to WSL-compatible paths.

**Conversion Rules:**
| Windows Path | WSL Path |
|--------------|----------|
| `C:\Users\...` | `/mnt/c/Users/...` |
| `D:\Projects\...` | `/mnt/d/Projects/...` |
| `\\server\share\...` | Not supported |

**Algorithm:**
1. Replace all backslashes (`\`) with forward slashes (`/`)
2. If path starts with drive letter (e.g., `C:`):
   - Extract drive letter and convert to lowercase
   - Replace `X:` with `/mnt/x`

### 4. Clipboard Path Copy

**Description:** Copies the WSL path back to clipboard after saving.

**Behavior:**
- Replaces the image in clipboard with the WSL path (text)
- Path can be directly pasted into WSL terminal

### 4.1 Project Mode

**Description:** Alternative mode for Claude Code integration.

**Purpose:**
- Save images directly within the project directory
- Use relative paths that Claude Code can reference without additional permissions
- Quickly toggle between normal mode (WSL absolute paths) and project mode (relative paths)

**Behavior (when enabled):**
- Saves images to `{ProjectRootPath}/{ProjectScreenshotsDir}/`
- Copies relative path to clipboard: `{ProjectScreenshotsDir}/filename.png`
- Example: `screenshots/clipboard_20250111_123456.png`

**Behavior (when disabled):**
- Application behaves as before (WSL absolute paths)
- Project root path setting persists for later use

### 5. System Tray Integration

**Description:** Runs as a system tray application with minimal UI.

**Features:**
- Tray icon with custom icon (icon.ico)
- Balloon notifications for save operations
- Context menu with options:
  - Settings
  - Enable Logging (toggle)
  - Exit

### 6. Logging (Optional)

**Description:** Debug logging for troubleshooting.

**Behavior:**
- Disabled by default
- Writes to `<application_directory>/clipboard_log.txt`
- Logs clipboard events, image detection, and errors

**Menu Option:** "Enable Logging" - Toggle logging on/off

---

## Settings

### Save Path

- **Type:** Directory path
- **Default:** `<application_directory>/ClipboardImages/`
- **Validation:** Directory must exist or be creatable
- **Persistence:** Saved to `settings.json`

### Logging Enable

- **Type:** Boolean
- **Default:** `false`
- **Persistence:** Saved to `settings.json`

### Project Mode Enabled

- **Type:** Boolean
- **Default:** `false`
- **Persistence:** Saved to `settings.json`

### Project Root Path

- **Type:** Directory path
- **Default:** Empty string
- **Persistence:** Saved to `settings.json`
- **Note:** Path persists even when Project Mode is disabled

### Project Screenshots Directory

- **Type:** Relative directory name/path
- **Default:** `screenshots`
- **Persistence:** Saved to `settings.json`

### Settings File

- **Location:** `<application_directory>/settings.json`
- **Format:** JSON
- **Created:** Automatically on first settings change

Example:
```json
{
  "SavePath": "C:\\Users\\...\\ClipboardImages",
  "EnableLogging": false,
  "ProjectModeEnabled": false,
  "ProjectRootPath": "",
  "ProjectScreenshotsDir": "screenshots"
}
```

---

## User Interface

### System Tray Icon

- Double-click: Opens Settings dialog
- Right-click: Shows context menu

### Context Menu

```
┌─────────────────┐
│ Settings        │
│ Enable Logging  │
├─────────────────┤
│ Exit            │
└─────────────────┘
```

### Settings Dialog

```
┌─────────────────────────────────────────────────────────┐
│ Settings                                            [X] │
├─────────────────────────────────────────────────────────┤
│                                                         │
│ Save Location:                                          │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ C:\path\to\ClipboardImages                          │ │
│ └─────────────────────────────────────────────────────┘ │
│                                           [Browse...]   │
│                                                         │
│ [x] Enable Project Mode                                 │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ Project Mode Settings                               │ │
│ │                                                     │ │
│ │ Project Root:                                       │ │
│ │ ┌───────────────────────────────────────────────┐   │ │
│ │ │ C:\projects\my-project                        │   │ │
│ │ └───────────────────────────────────────────────┘   │ │
│ │                                      [Browse...]    │ │
│ │                                                     │ │
│ │ Screenshots Folder:                                 │ │
│ │ ┌────────────────────────┐                          │ │
│ │ │ screenshots            │                          │ │
│ │ └────────────────────────┘                          │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                         │
│                              [OK]     [Cancel]          │
└─────────────────────────────────────────────────────────┘
```

### Balloon Notifications

**On successful save:**
- Title: "Image Saved"
- Content: Shows file path and WSL path
- Duration: 3 seconds

**On error:**
- Title: "Error"
- Content: Error message
- Icon: Error icon

---

## Error Handling

| Error | Behavior |
|-------|----------|
| Clipboard access failed | Log error, skip processing |
| Image save failed | Show error balloon notification |
| Directory creation failed | Show error balloon notification |

---

## Limitations

1. **Network paths:** UNC paths (`\\server\share`) are not converted
2. **Image formats:** Only saves as PNG (original format not preserved)
3. **Large images:** No size limit check (may cause memory issues)
