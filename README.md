# Development Environment Quicklaunch

A WPF application for quickly launching your development environment, including Visual Studio solution files, SQL Server Management Studio, your preferred text editor, and any additional applications. Easily configure paths, manage your solution list, and control startup behavior.

## Features

- **Add/Remove Solution Files:**  
  Manage a list of Visual Studio `.sln` files to launch.

- **Individual Solution Launch:**  
  Right-click any solution in the list to launch it directly.

- **Batch Launch:**  
  Start all configured solutions, SQL Server Management Studio, text editor, and additional apps with one click.

- **Configurable Paths:**  
  Set custom paths for Visual Studio, SSMS, text editor, and any number of additional applications.

- **Startup Options:**  
  - Launch text editor automatically.
  - Optionally launch SSMS only if not already running.
  - Launch applications as Administrator.
  - Launch Quicklaunch itself on Windows startup.

- **Theme Selection:**  
  Choose between Light, Dark, and custom themes.

- **Persistent Settings:**  
  All settings and solution lists are saved to your user profile.

## Getting Started

### Prerequisites

- Windows 10 or later
- .NET 10
- Visual Studio 2026 (for development)
- [Windows Script Host Object Model](https://learn.microsoft.com/en-us/previous-versions/windows/script-host/cc364421(v=vs.85)) (COM reference for startup shortcut feature)

### Installation

1. **Clone the repository:**
2. **Open the solution in Visual Studio 2026.**

3. **Restore NuGet packages (if any).**

4. **Add COM Reference:**
- Right-click your project > Add > Reference...
- Go to COM > Type Libraries.
- Add **Windows Script Host Object Model**.

5. **Build and run the application.**

### Usage

1. **Add solution files** using the "Add Solution" button.
2. **Configure application paths** in the Settings window.
3. **Set your preferences** (launch options, theme, etc.).
4. **Click "Start"** to launch your full environment.
5. **Right-click a solution** in the list to launch it individually.

### Settings

- **Visual Studio Path:** Select the executable for your preferred Visual Studio version.
- **SQL Server Management Studio Path:** Select the SSMS executable.
- **Text Editor Path:** Select your favorite text editor.
- **Additional Applications:** Add any other tools you want to launch.
- **Launch on Startup:** Automatically start Quicklaunch when Windows starts.
- **Theme:** Choose your preferred UI theme.

## Screenshots

> _Add screenshots of the main window and settings window here._

## Contributing

Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License

MIT License. See [LICENSE](LICENSE) for details.

## Acknowledgements

- [Microsoft WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Windows Script Host](https://learn.microsoft.com/en-us/previous-versions/windows/script-host/cc364421(v=vs.85))
