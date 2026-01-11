# AddClipboardFormatListener Implementation Plan

## Status: COMPLETED

## Overview

Replace the current timer-based polling mechanism with the event-driven `AddClipboardFormatListener` API for more efficient clipboard monitoring.

## Current Implementation Analysis

### Current Approach: Timer Polling

```
MainForm
    │
    ├── Timer (1 second interval)
    │       │
    │       └── ClipboardTimer_Tick()
    │               │
    │               └── Check Clipboard.ContainsImage()
    │
    └── Clipboard Chain (partially implemented, only logging)
            │
            └── WM_DRAWCLIPBOARD → HandleClipboardChange() → Log only
```

### Problems with Current Approach

| Issue | Description |
|-------|-------------|
| CPU Usage | Timer fires every 1 second regardless of clipboard activity |
| Latency | Up to 1 second delay between clipboard change and detection |
| Complexity | Clipboard chain partially implemented but not utilized |
| Reliability | Timer-based approach may miss rapid clipboard changes |

## Proposed Implementation

### AddClipboardFormatListener API

Windows Vista+ API that provides reliable, event-driven clipboard notifications.

```
MainForm
    │
    ├── AddClipboardFormatListener(hWnd) ← Register on startup
    │
    ├── WndProc
    │       │
    │       └── WM_CLIPBOARDUPDATE → ProcessClipboardChange()
    │
    └── RemoveClipboardFormatListener(hWnd) ← Unregister on exit
```

### API Specifications

```csharp
// Win32 API declarations
[DllImport("user32.dll", SetLastError = true)]
static extern bool AddClipboardFormatListener(IntPtr hwnd);

[DllImport("user32.dll", SetLastError = true)]
static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

// Windows message
const int WM_CLIPBOARDUPDATE = 0x031D;
```

### Advantages

| Advantage | Description |
|-----------|-------------|
| Efficiency | Only triggered when clipboard actually changes |
| Low Latency | Immediate notification on clipboard change |
| Reliability | More reliable than clipboard chain (SetClipboardViewer) |
| Simplicity | No chain management required |

## Implementation Steps

### Step 1: Add API Declarations

Add Win32 API declarations to MainForm.cs:

```csharp
[DllImport("user32.dll", SetLastError = true)]
static extern bool AddClipboardFormatListener(IntPtr hwnd);

[DllImport("user32.dll", SetLastError = true)]
static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

const int WM_CLIPBOARDUPDATE = 0x031D;
```

### Step 2: Register Listener on Startup

In MainForm constructor, after handle is created:

```csharp
public MainForm()
{
    InitializeComponent();
    // ... existing code ...

    // Register for clipboard notifications
    if (!AddClipboardFormatListener(this.Handle))
    {
        WriteLog("Failed to add clipboard format listener");
    }
}
```

### Step 3: Handle WM_CLIPBOARDUPDATE

Update WndProc to handle the new message:

```csharp
protected override void WndProc(ref Message m)
{
    switch (m.Msg)
    {
        case WM_CLIPBOARDUPDATE:
            ProcessClipboardChange();
            break;
        // ... existing cases ...
    }
    base.WndProc(ref m);
}

private void ProcessClipboardChange()
{
    if (isProcessingClipboard) return;

    try
    {
        isProcessingClipboard = true;

        if (Clipboard.ContainsImage())
        {
            WriteLog("Clipboard update: contains image");
            Image? image = Clipboard.GetImage();

            if (image != null)
            {
                string hash = GetImageHash(image);

                if (hash != lastClipboardHash)
                {
                    WriteLog($"New image: {image.Width}x{image.Height}");
                    lastClipboardHash = hash;
                    SaveImageAndConvertPath(image);
                }
            }
        }
    }
    catch (Exception ex)
    {
        WriteLog($"Clipboard processing error: {ex.Message}");
    }
    finally
    {
        isProcessingClipboard = false;
    }
}
```

### Step 4: Unregister on Exit

In ExitApplication method:

```csharp
private void ExitApplication()
{
    RemoveClipboardFormatListener(this.Handle);
    // ... existing cleanup code ...
}
```

### Step 5: Remove/Modify Timer Code

Options:
1. **Remove completely**: Delete timer-related code
2. **Keep as fallback**: Reduce interval to 5+ seconds as backup
3. **Make optional**: Keep "Enable Timer" menu for manual fallback

**Recommended: Option 1 (Remove completely)**

### Step 6: Remove Old Clipboard Chain Code

Remove deprecated clipboard chain code:
- `SetClipboardViewer` / `ChangeClipboardChain` calls
- `WM_DRAWCLIPBOARD` / `WM_CHANGECBCHAIN` handling
- `nextClipboardViewer` field

## Code Changes Summary

| File | Changes |
|------|---------|
| `MainForm.cs` | Add API declarations, update WndProc, remove timer/chain code |

### Lines to Remove

```csharp
// Remove these declarations
[DllImport("user32.dll")]
static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

[DllImport("user32.dll")]
static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

const int WM_DRAWCLIPBOARD = 0x308;
const int WM_CHANGECBCHAIN = 0x30D;

IntPtr nextClipboardViewer;

// Remove timer-related code
private System.Windows.Forms.Timer clipboardTimer;
private bool timerEnabled = true;

// Remove from constructor
clipboardTimer = new System.Windows.Forms.Timer();
clipboardTimer.Interval = 1000;
clipboardTimer.Tick += ClipboardTimer_Tick;
clipboardTimer.Start();

nextClipboardViewer = SetClipboardViewer(this.Handle);

// Remove ClipboardTimer_Tick method
// Remove timer menu item from SetupSystemTray
// Remove HandleClipboardChange method
```

## Menu Changes

| Current | Proposed |
|---------|----------|
| Enable Timer (toggle) | Remove (no longer needed) |
| Enable Logging (toggle) | Keep |
| Settings | Keep |
| Exit | Keep |

## Test Plan

### Manual Testing

1. **Basic Functionality**
   - Copy image from browser → Verify saved and path converted
   - Copy image from screenshot tool → Verify saved
   - Copy text → Verify no action taken

2. **Rapid Changes**
   - Copy multiple images quickly → Verify all processed
   - Copy same image twice → Verify only processed once (hash check)

3. **Edge Cases**
   - Copy large image → Verify handled correctly
   - Copy while processing → Verify no race conditions

4. **Application Lifecycle**
   - Start app → Verify listener registered
   - Exit app → Verify listener unregistered
   - Minimize/restore → Verify continues working

### Verification Commands

```bash
# Build
dotnet build

# Run existing tests (should still pass)
dotnet test

# Build release
dotnet publish -r win-x64 -c Release
```

## Rollback Plan

If issues are discovered:
1. Revert MainForm.cs to previous version
2. Timer-based approach is well-tested fallback

## Documentation Updates

After implementation:
1. Update `docs/reference/architecture-overview.md`
   - Update architecture diagram
   - Update MainForm description
   - Remove timer references
2. Update `docs/reference/feature-specification.md`
   - Update clipboard monitoring description
   - Remove "Enable Timer" menu documentation
3. Update `CLAUDE.md`
   - Update key classes description

## Timeline

| Phase | Description |
|-------|-------------|
| 1 | Implement AddClipboardFormatListener |
| 2 | Remove timer code |
| 3 | Remove old clipboard chain code |
| 4 | Manual testing on Windows |
| 5 | Update documentation |
| 6 | Commit and push |

## References

- [AddClipboardFormatListener (MSDN)](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-addclipboardformatlistener)
- [RemoveClipboardFormatListener (MSDN)](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-removeclipboardformatlistener)
- [WM_CLIPBOARDUPDATE (MSDN)](https://docs.microsoft.com/en-us/windows/win32/dataxchg/wm-clipboardupdate)
