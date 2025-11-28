# RyCookie Text - Windows Text Expander

A lightweight, efficient text expander for Windows built with C# and WPF. Expand frequently-used text snippets with custom keywords.

## Features

- 🚀 **Global Keyboard Monitoring** - Works in any application
- 📝 **Snippet Management** - Easy-to-use UI for creating and organizing snippets
- 🎯 **Smart Matching** - Respects word boundaries and trigger characters
- 🌐 **Unicode Support** - Full support for multi-line and Unicode text
- 💾 **JSON Storage** - Simple, portable snippet storage
- 🎨 **System Tray Integration** - Runs quietly in the background
- ⚡ **Lightweight** - Minimal resource usage

## Installation

### Prerequisites
- Windows 10 or later
- .NET 8 Runtime

### Build from Source

1. Clone the repository:
```bash
git clone <repository-url>
cd rycookietext
```

2. Build the project:
```bash
dotnet build -c Release
```

3. Run the application:
```bash
dotnet run
```

## Usage

### Basic Usage

1. **Launch the app** - The application will start in the system tray
2. **Create snippets** - Right-click the tray icon → "Open Snippet Manager"
3. **Type a keyword** - Type any keyword followed by space, tab, or punctuation
4. **Watch it expand** - The keyword is automatically replaced with your snippet content

### Example Snippets

| Keyword | Expands To |
|---------|------------|
| `btw` | by the way |
| `omw` | on my way |
| `addr` | Your full address (multi-line) |
| `sig` | Your email signature |

### Managing Snippets

- **Add**: Click "Add Snippet" in the Snippet Manager
- **Edit**: Double-click a snippet or select and click "Edit"
- **Delete**: Select a snippet and click "Delete"
- **Search**: Use the search box to filter snippets
- **Enable/Disable**: Toggle individual snippets or the entire expansion feature

### System Tray Menu

- **Open Snippet Manager** - View and manage your snippets
- **Enable/Disable Expansion** - Quickly toggle text expansion on/off
- **Exit** - Close the application

## Architecture

```
RyCookieText/
├── Models/
│   ├── Snippet.cs          # Snippet data model
│   └── AppSettings.cs      # Application settings
├── Services/
│   ├── KeyboardHook.cs     # Global keyboard hook
│   ├── BufferManager.cs    # Typed character buffer
│   ├── SnippetManager.cs   # CRUD operations
│   ├── SnippetMatcher.cs   # Keyword matching
│   ├── SnippetReplacer.cs  # Text replacement engine
│   ├── ExpansionEngine.cs  # Orchestration
│   └── TrayIconManager.cs  # System tray integration
├── Views/
│   ├── MainWindow.xaml     # Snippet manager UI
│   └── SnippetEditDialog.xaml  # Edit dialog
├── Helpers/
│   └── Win32Api.cs         # Win32 API declarations
└── App.xaml                # Application entry point
```

## Configuration

Settings are stored in `settings.json`:

```json
{
  "IsExpansionEnabled": true,
  "StartWithWindows": false,
  "SnippetsFilePath": "snippets.json",
  "BufferSize": 50,
  "KeystrokeDelayMs": 5
}
```

Snippets are stored in `snippets.json` in the application directory.

## Development

### Technologies Used
- **.NET 8** - Modern C# framework
- **WPF** - Windows Presentation Foundation for UI
- **Hardcodet.NotifyIcon.Wpf** - System tray support
- **Newtonsoft.Json** - JSON serialization
- **Win32 API** - Keyboard hooks and input simulation

### Key Components

1. **KeyboardHook** - Uses `SetWindowsHookEx` to capture global keyboard events
2. **BufferManager** - Maintains a rolling buffer of recent keystrokes
3. **SnippetMatcher** - Efficiently matches typed text against snippet keywords
4. **SnippetReplacer** - Uses `SendInput` API to simulate backspace and typing
5. **ExpansionEngine** - Coordinates all components for seamless expansion

## Troubleshooting

### Snippets not expanding
- Ensure "Enable Expansion" is checked in the tray menu
- Check that the snippet is enabled in the Snippet Manager
- Verify you're typing the exact keyword followed by a trigger character (space, tab, etc.)

### Application not starting
- Ensure .NET 8 Runtime is installed
- Check Windows Event Viewer for error details
- Try running as Administrator

### High CPU usage
- Reduce `BufferSize` in settings.json
- Check for conflicting keyboard hook applications

## License

MIT License - See LICENSE file for details

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues.

## Acknowledgments

Inspired by [Beeftext](https://beeftext.org/) and other text expander applications.
