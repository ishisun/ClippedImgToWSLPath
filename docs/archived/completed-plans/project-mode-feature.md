# Project Mode Feature Implementation Plan

## Overview

Add a "Project Mode" feature that allows saving clipboard images to a project-specific directory and copying relative paths to the clipboard instead of WSL absolute paths.

---

## Purpose

Enable seamless integration with Claude Code during development:
- Save images directly within the project directory
- Use relative paths that Claude Code can reference without additional permissions
- Quickly toggle between normal mode (WSL absolute paths) and project mode (relative paths)

---

## Requirements

### Functional Requirements

1. **Project Mode Toggle**
   - Add a setting to enable/disable Project Mode
   - When disabled, the application behaves as before (WSL absolute paths)
   - Project root path setting persists even when Project Mode is OFF

2. **Project Root Path Setting**
   - Allow users to select a project root directory via folder browser
   - Persist the setting in `settings.json`
   - Independent of the existing Save Path setting

3. **Project Screenshots Directory Setting**
   - Configurable subdirectory name within the project root
   - Default value: `screenshots`
   - Persist the setting in `settings.json`

4. **Image Saving Behavior (Project Mode ON)**
   - Save images to `{ProjectRootPath}/{ProjectScreenshotsDir}/`
   - Create the directory if it doesn't exist

5. **Clipboard Path Behavior (Project Mode ON)**
   - Copy relative path from project root: `{ProjectScreenshotsDir}/filename.png`
   - Example: `screenshots/clipboard_20250111_123456.png`

### Non-Functional Requirements

1. Settings must be backward compatible (existing `settings.json` files should still work)
2. UI should clearly indicate current mode

---

## Design

### Settings Structure

```json
{
  "SavePath": "C:\\path\\to\\ClipboardImages",
  "EnableLogging": false,
  "ProjectModeEnabled": false,
  "ProjectRootPath": "",
  "ProjectScreenshotsDir": "screenshots"
}
```

### Settings Dialog UI

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
│ ─────────────────────────────────────────────────────── │
│                                                         │
│ [x] Enable Project Mode                                 │
│                                                         │
│ Project Root:                                           │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ C:\projects\my-project                              │ │
│ └─────────────────────────────────────────────────────┘ │
│                                           [Browse...]   │
│                                                         │
│ Screenshots Folder:                                     │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ screenshots                                         │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                         │
│                              [OK]     [Cancel]          │
└─────────────────────────────────────────────────────────┘
```

### Path Generation Logic

```
IF ProjectModeEnabled AND ProjectRootPath is valid:
    savePath = {ProjectRootPath}/{ProjectScreenshotsDir}/
    clipboardPath = {ProjectScreenshotsDir}/{filename}
ELSE:
    savePath = SavePath
    clipboardPath = ConvertToWSLPath(savePath + filename)
```

---

## Implementation Steps (TDD Approach)

### Phase 1: Update SettingsManager

1. **Write tests for new settings properties**
   - Test default values for new properties
   - Test loading settings with new properties
   - Test loading legacy settings (backward compatibility)
   - Test saving settings with new properties

2. **Implement SettingsManager changes**
   - Add `ProjectModeEnabled` property (default: `false`)
   - Add `ProjectRootPath` property (default: `""`)
   - Add `ProjectScreenshotsDir` property (default: `"screenshots"`)
   - Update `SettingsData` class
   - Update `Load()` and `Save()` methods

### Phase 2: Add ProjectPathResolver Class

1. **Write tests for ProjectPathResolver**
   - Test `GetSavePath()` returns correct path based on mode
   - Test `GetClipboardPath()` returns WSL path in normal mode
   - Test `GetClipboardPath()` returns relative path in project mode
   - Test directory creation logic

2. **Implement ProjectPathResolver**
   - Create new utility class for path resolution
   - Method: `GetSavePath(settings)` - returns directory to save images
   - Method: `GetClipboardPath(settings, filePath)` - returns path for clipboard

### Phase 3: Update SettingsDialog

1. **Update UI**
   - Add Project Mode checkbox
   - Add Project Root path text box and browse button
   - Add Screenshots Folder text box
   - Enable/disable project settings based on checkbox state

2. **Update dialog logic**
   - Pass new settings to/from dialog
   - Validate project root path when Project Mode is enabled

### Phase 4: Update MainForm

1. **Integrate ProjectPathResolver**
   - Use `ProjectPathResolver.GetSavePath()` for image saving
   - Use `ProjectPathResolver.GetClipboardPath()` for clipboard

2. **Update SaveImageAndConvertPath method**
   - Use resolved paths instead of hardcoded logic

### Phase 5: Update Documentation

1. Update `docs/reference/feature-specification.md`
2. Update `docs/reference/architecture-overview.md`
3. Move this plan to `docs/archived/completed-plans/`

---

## Test Plan

### Unit Tests

| Test Case | Description |
|-----------|-------------|
| `SettingsManager_DefaultValues_ProjectModeDisabled` | New settings default to disabled |
| `SettingsManager_Load_LegacySettings` | Old settings files load without error |
| `SettingsManager_Load_NewSettings` | New settings load correctly |
| `SettingsManager_Save_IncludesNewProperties` | Saved JSON includes new properties |
| `ProjectPathResolver_GetSavePath_NormalMode` | Returns SavePath when ProjectMode disabled |
| `ProjectPathResolver_GetSavePath_ProjectMode` | Returns project path when enabled |
| `ProjectPathResolver_GetClipboardPath_NormalMode` | Returns WSL path |
| `ProjectPathResolver_GetClipboardPath_ProjectMode` | Returns relative path |

### Manual Testing

1. Fresh install - verify default behavior unchanged
2. Enable Project Mode - verify images save to project directory
3. Verify relative path copied to clipboard in Project Mode
4. Toggle Project Mode OFF - verify WSL path behavior restored
5. Restart application - verify settings persist
6. Test with existing `settings.json` - verify backward compatibility

---

## Affected Files

| File | Changes |
|------|---------|
| `SettingsManager.cs` | Add new properties and update serialization |
| `SettingsDialog.cs` | Add Project Mode UI controls |
| `MainForm.cs` | Integrate ProjectPathResolver |
| `ProjectPathResolver.cs` | New file - path resolution logic |
| `ClippedImgToWSLPath.Tests/SettingsManagerTests.cs` | New tests for settings |
| `ClippedImgToWSLPath.Tests/ProjectPathResolverTests.cs` | New test file |

---

## Status

- [x] Requirements defined
- [x] Design completed
- [x] Phase 1: SettingsManager update
- [x] Phase 2: ProjectPathResolver implementation
- [x] Phase 3: SettingsDialog update
- [x] Phase 4: MainForm integration
- [x] Phase 5: Documentation update

**COMPLETED**: 2026-01-11
