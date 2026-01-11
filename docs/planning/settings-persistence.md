# Settings Persistence Design

## Overview

Implement settings persistence to save and load application settings across restarts.

---

## Requirements

### Functional Requirements

1. Save settings to disk when the application exits
2. Load settings from disk when the application starts
3. Use default values if settings file does not exist
4. Persist the following settings:
   - `SavePath` - Directory path for saving clipboard images
   - `EnableLogging` - Boolean flag for debug logging

### Non-Functional Requirements

1. Settings file format: JSON (human-readable, easy to edit)
2. Settings file location: Same directory as the application executable
3. Settings file name: `settings.json`

---

## Design

### SettingsManager Class

```csharp
public class SettingsManager
{
    public string SavePath { get; set; }
    public bool EnableLogging { get; set; }

    public void Load();  // Load from settings.json
    public void Save();  // Save to settings.json
}
```

### Settings File Format

```json
{
  "SavePath": "C:\\Users\\...\\ClipboardImages",
  "EnableLogging": false
}
```

### Default Values

| Setting | Default Value |
|---------|---------------|
| `SavePath` | `<application_directory>/ClipboardImages` |
| `EnableLogging` | `false` |

### Error Handling

| Scenario | Behavior |
|----------|----------|
| Settings file does not exist | Use default values |
| Settings file is corrupted/invalid JSON | Use default values, log warning |
| Settings file write fails | Log error, continue without saving |

---

## Integration with MainForm

### On Application Start

```csharp
// In MainForm constructor
var settings = new SettingsManager();
settings.Load();
savePath = settings.SavePath;
enableLogging = settings.EnableLogging;
```

### On Settings Change

```csharp
// In ShowSettingsDialog()
if (dialog.ShowDialog() == DialogResult.OK)
{
    savePath = dialog.SavePath;
    settings.Save();
}
```

### On Application Exit

```csharp
// In ExitApplication()
settings.Save();
```

---

## Test Cases

1. **Load with no file** - Returns default values
2. **Load with valid file** - Returns saved values
3. **Load with invalid JSON** - Returns default values
4. **Save and Load roundtrip** - Values are preserved
5. **SavePath default** - Uses application directory + ClipboardImages

---

## Status

- [x] Design complete
- [x] Tests written
- [x] Implementation complete
- [x] Documentation updated
