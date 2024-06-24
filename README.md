# Rounded Windows Edges

Rounded Windows Edges is a Windows utility that enhances your user interface by adding customizable rounded corners to all application windows across multiple monitors. This application is designed to provide a modern and aesthetically pleasing look to your desktop environment.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)
- [Development](#development)
  - [Code Structure](#code-structure)
  - [Building and Running](#building-and-running)
  - [Contributing](#contributing)
- [License](#license)
- [Acknowledgements](#acknowledgements)
- [Contact](#contact)

## Features

- Automatically detects all connected monitors and applies rounded corners to windows.
- Configurable corner size.
- Runs in the background with a system tray icon for easy access.
- Persistent configuration settings.
- Supports high DPI displays.
- Minimal performance impact.

## Prerequisites

- Windows operating system
- .NET Framework
- Visual Studio (for building from source)

## Installation

1. Clone the repository:

    ```sh
    git clone https://github.com/mauriciobellon/rounded-windows-edges.git
    ```

2. Open the solution in Visual Studio.

3. Restore the required NuGet packages.

4. Build the solution.

## Usage

1. Run the application after building it.

2. The application will automatically detect all connected monitors and apply rounded corners to windows.

3. Use the tray icon to:
   - Open settings and configure the corner size.
   - Exit the application.

## Development

### Code Structure

- `App.xaml.cs`: The main application file that initializes the application and sets up the main windows and tray icon.
- `MainWindow.xaml.cs`: Manages the main window for each screen, applying the rounded corners and handling window events.
- `AppConfig.cs`: Handles the loading and saving of configuration settings.
- `TrayIcon.cs`: Manages the system tray icon and context menu.

### Building and Running

1. Clone the repository:

    ```sh
    git clone https://github.com/mauriciobellon/rounded-windows-edges.git
    ```

2. Open the solution in Visual Studio.

3. Restore the required NuGet packages.

4. Build the solution.

5. Run the application.

### Contributing

1. Fork the repository.

2. Create a new branch:

    ```sh
    git checkout -b feature/your-feature
    ```

3. Make your changes and commit them:

    ```sh
    git commit -m 'Add some feature'
    ```

4. Push to the branch:

    ```sh
    git push origin feature/your-feature
    ```

5. Open a pull request to the main repository.

## To Do

- [x] Add Icon Tray
- [x] Add Settings on Icons Tray
- [x] Add Autostart
- [x] Add support for multiple monitors
- [x] Add support for high DPI displays
- [x] Persistent configuration
- [x] Create Installation Package
- [ ] Add Screenshot to Readme
- [ ] FEATURE: Autorun after Installation
- [ ] ISSUE: Config only applies changes on app restart

## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- Inspired by [BeezBeez/Windows-RoundedScreen](https://github.com/BeezBeez/Windows-RoundedScreen).
- Uses [Newtonsoft.Json](https://www.newtonsoft.com/json) for JSON serialization and deserialization.

## Contact

For any questions or suggestions, feel free to open an issue or contact the maintainer:

- GitHub: [mauriciobellon](https://github.com/mauriciobellon)
