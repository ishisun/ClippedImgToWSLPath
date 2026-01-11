# Development Setup Guide

## Prerequisites

### Required Software

| Software | Version | Purpose |
|----------|---------|---------|
| .NET SDK | 8.0+ | Build and run the application |
| Git | Any | Version control |
| WSL/WSL2 | Any | For testing WSL path conversion |

### Optional Software

| Software | Purpose |
|----------|---------|
| Visual Studio 2022 | IDE with Windows Forms designer |
| Visual Studio Code | Lightweight editor |
| JetBrains Rider | Cross-platform IDE |

---

## Installation

### 1. Clone the Repository

```bash
git clone git@github.com:ishisun/ClippedImgToWSLPath.git
cd ClippedImgToWSLPath
```

### 2. Verify .NET SDK

```bash
dotnet --version
# Should output 8.0.x or higher
```

### 3. Restore Dependencies

```bash
dotnet restore
```

---

## Building

### Build on Windows

```bash
dotnet build
```

### Build on WSL (Cross-compile for Windows)

The project includes `EnableWindowsTargeting` in the .csproj file, allowing builds on non-Windows systems.

```bash
dotnet build
```

### Build for Release

```bash
dotnet publish -r win-x64 -c Release
```

Output location: `bin/Release/net8.0-windows/win-x64/publish/`

---

## Running

### Run from Source (Windows only)

```bash
dotnet run
```

### Run Published Executable

```bash
# From Windows
bin/Release/net8.0-windows/win-x64/publish/ClippedImgToWSLPath.exe

# From WSL (via Windows path)
/mnt/c/.../ClippedImgToWSLPath.exe
```

---

## Testing

### Run All Tests

```bash
dotnet test
```

### Run Tests with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Project Structure

```
ClippedImgToWSLPath/
├── ClippedImgToWSLPath.csproj    # Main project
├── ClippedImgToWSLPath.Tests/    # Test project
├── ClippedImgToWSLPath.sln       # Solution file
├── docs/                         # Documentation
├── MainForm.cs                   # Main application
├── SettingsDialog.cs             # Settings dialog
├── Program.cs                    # Entry point
├── IconGenerator.cs              # Icon utilities
└── icon.ico                      # Application icon
```

---

## Development Workflow

### 1. Create Feature Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Changes

Follow TDD approach:
1. Write failing test
2. Implement feature
3. Verify test passes

### 3. Run Tests

```bash
dotnet test
```

### 4. Build and Verify

```bash
dotnet build
dotnet publish -r win-x64 -c Release
```

### 5. Commit and Push

```bash
git add .
git commit -m "Add your feature description"
git push origin feature/your-feature-name
```

---

## Common Issues

### Issue: NETSDK1100 on WSL

**Error:** `error NETSDK1100: To build a project targeting Windows...`

**Solution:** Already fixed. The project includes:
```xml
<EnableWindowsTargeting>true</EnableWindowsTargeting>
```

### Issue: System.Drawing Not Found

**Error:** `System.Drawing.Common is not supported on this platform`

**Solution:** This is expected on non-Windows platforms. The application must run on Windows.

---

## IDE Setup

### Visual Studio 2022

1. Open `ClippedImgToWSLPath.sln`
2. Set startup project to `ClippedImgToWSLPath`
3. Press F5 to run

### Visual Studio Code

1. Install C# extension
2. Open folder
3. Use `dotnet build` and `dotnet run` from terminal

### JetBrains Rider

1. Open `ClippedImgToWSLPath.sln`
2. Build and run using toolbar buttons
