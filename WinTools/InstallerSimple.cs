﻿namespace WinTools;

using System.Text.Json;
using System.IO;
using System.IO.Compression;

/// <summary>
/// Provides a simplified mechanism for installing and uninstalling Windows applications.
/// </summary>
/// <remarks>
/// This class enables installation by extracting application files, setting up shortcuts,
/// adding registry entries, and optionally configuring context menu entries in Windows Explorer.
/// It also supports uninstallation by cleaning up created files, registry keys, and shortcuts.
///
/// The term "software components" refers to the following files associated with the application:
///    (1) The main executable file used to launch the application.
///    (2) The icon file representing the application in the Start Menu.
///    (3) If additional contextual menu entries are included in Windows Explorer: the icon file(s) displayed in those entries.
///    (4) The executable file used to uninstall the application.
///    (5) Any other files required for the application to function properly (ex: JRE, JavaFX components...).
/// 
/// The installer assumes the following requirements for the application's software components:
///    (1) All components must reside in a single directory, referred to as the "application directory".
///    (2) If the application's name is provided as "app-name", the executable used to launch the application must be named "app-name.exe".
///    (3) Similarly, the associated icon file must be named "app-name.ico".
///    (4) The uninstallation program should always be named "Uninstall.exe".
///
/// Example directory structure for the "crypt" application:
///
///     install_dir ┌── crypt.exe              // Main executable for launching the application
///                 ├── crypt.ico              // Icon associated with the application
///                 ├── icon-decrypt.ico       // Icon for a contextual menu entry in Explorer
///                 ├── icon-encrypt.ico       // Another icon for a contextual menu entry
///                 └── Uninstall.exe          // Uninstaller program
/// 
/// </remarks>
public class InstallerSimple
{
    /// <summary>
    /// Represents the file system path to the directory where the application is installed.
    /// This variable is utilized to determine or access the installation directory of the application
    /// during setup, maintenance, or uninstallation processes.
    /// </summary>
    private readonly string _applicationInstallationDirectoryPath;
    /// <summary>
    /// Represents the name of the application.
    /// This attribute is used to identify the application uniquely, often for display purposes,
    /// configuration, or during the installation process.
    /// </summary>
    private readonly string _applicationName;
    /// <summary>
    /// Represents the version of the application.
    /// This information is used to indicate the specific iteration or release
    /// of the application, which can help in version tracking, updates, or compatibility checks.
    /// </summary>
    private readonly string _applicationVersion;
    /// <summary>
    /// Represents the name of the publisher of the application.
    /// This information is typically used to identify and attribute
    /// the organization or individual responsible for creating or distributing the application.
    /// </summary>
    private readonly string _applicationPublisher;
    /// <summary>
    /// Represents the absolute file path to the application's executable file.
    /// This path is used to locate and launch the main application binary.
    /// </summary>
    private readonly string _applicationExecutablePath;
    /// <summary>
    /// Represents the absolute file path to the application's icon file.
    /// This path is used to locate the icon associated with the application, typically used
    /// for display purposes such as desktop shortcuts or in the start menu.
    /// <remarks>
    /// The icon file is generally an .ico file and is located within the application's
    /// installation directory.
    /// </remarks>
    /// </summary>
    private readonly string _applicationIconPath;
    /// <summary>
    /// Represents the absolute file path to the application's uninstaller executable.
    /// This path is used to locate and invoke the uninstaller, enabling proper removal
    /// of the application from the user's system.
    /// <remarks>
    /// The uninstaller file is typically located in the application's installation directory
    /// and named "Uninstall.exe" by default.
    /// </remarks>
    /// </summary>
    private readonly string _applicationUninstallerPath;
    /// <summary>
    /// Represents the registry key associated with the application's installation information.
    /// This key is typically used for storing or retrieving details about the application,
    /// such as its installation path, version, and uninstallation procedures.
    /// <remarks>
    /// This registry key follows the standard path for installed applications in Windows,
    /// under the "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall" hive, and is
    /// named using the application's name to ensure uniqueness.
    /// </remarks>
    /// </summary>
    private readonly string _applicationRegistryKey;
    /// <summary>
    /// Represents the absolute path to the application's Start Menu shortcut file.
    /// Typically, this shortcut is created during the installation process
    /// and is located within the user's Start Menu programs directory.
    /// <remarks>
    /// The path is generated using the application's name and formatted
    /// for the underlying operating system. It is used to provide
    /// quick access to the application's executable via the Start Menu.
    /// </remarks>
    /// </summary>
    private readonly string _applicationStartMenuShortcutPath;
    /// <summary>
    /// Represents the full file path to the application's archive file.
    /// The archive is a compressed ZIP file that contains all necessary components
    /// required for application installation or deployment.
    /// <remarks>
    /// The path is expected to be absolute and correctly formatted for the underlying operating system.
    /// </remarks>
    /// </summary>
    private readonly string _applicationArchivePath;
    /// <summary>
    /// Represents the collection of entries defining the specifications for the contextual menu
    /// in Windows Explorer. Each entry includes configuration such as file pattern, label, command,
    /// and icon path to customize the menu's appearance and behavior.
    /// <remarks>
    /// Please note that the command specifies the full absolute path to the application's executable, along with any
    /// additional parameters that may be required.
    /// Example: "/path/to/application.exe" "encrypt" "%1" 
    /// </remarks>
    /// <remarks>
    /// The icon is specified using its absolute path.
    /// </remarks>
    /// </summary>
    private readonly ExplorerContextualMenuEntry[]? _contextualMenuEntries;
    /// <summary>
    /// Determines whether detailed logs and messages are displayed during the installer's execution.
    /// When set to true, additional information is output to the console to aid in debugging or
    /// providing progress updates during the installation process.
    /// </summary>
    private readonly bool _verbose;

    public string ApplicationInstallationDirectoryPath => _applicationInstallationDirectoryPath;


    private static InstallerConfig _loadConfig(string configPath) {
        try {
            var json   = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<InstallerConfig>(json);
            if (config is null) {
                throw new Exception($"Failed to deserialize configuration file \"{configPath}\"");
            }
            config.Check();
            return config;
        }
        catch (Exception e) {
            throw new Exception($"Failed to load configuration file \"{configPath}\": {e.Message}", e);
        }
    }
    
    private static string _getConfigFileFromZip(string zipFilePath, string configFileName) {
        var              zipFile     = ZipFile.OpenRead(zipFilePath);
        ZipArchiveEntry? entry       = zipFile.GetEntry(configFileName);
        var              extractPath = common.Os.CreateTemporaryDirectory();

        if (entry != null)
        {
            var destinationPath = Path.Combine(extractPath, entry.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
            entry.ExtractToFile(destinationPath, overwrite: true);
            return destinationPath;
        }
        throw new Exception($"The configuration file \"{configFileName}\" was not found in the archive \"{zipFilePath}\".");
    }

    private void _deleteConfigFile(string configFilePath) {
        var tmpDir= Path.GetDirectoryName(configFilePath);
        if (tmpDir is null) {
            throw new Exception("Failed to get the directory name of the configuration file.");
        }
        if (_verbose) Console.WriteLine($"Deleting temporary directory \"{tmpDir}\"...");
        Directory.Delete(tmpDir, recursive: true);
    }
    
    public InstallerSimple(string archivePath, string installationPath, bool verbose = false) {
        // Load the configuration from the configuration file.
        var configPath = _getConfigFileFromZip(archivePath, "config-install.json");
        if (verbose) Console.WriteLine($"Load configuration from \"{configPath}\"");
        var config     = _loadConfig(configPath);
        _deleteConfigFile(configPath);
        
        // Extract the configuration values from the loaded configuration.
        _applicationInstallationDirectoryPath = installationPath;
        config.PrefixRootPath(_applicationInstallationDirectoryPath);
        var applicationExecutableBaseName = Path.GetFileNameWithoutExtension(config.ExecutablePath);
        
        _applicationName                  = config.Name;
        _applicationVersion               = config.Version;
        _applicationPublisher             = config.Publisher;
        _applicationIconPath              = config.IconPath;
        _applicationExecutablePath        = config.ExecutablePath;
        _applicationRegistryKey           = $@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{config.RegId}";
        _applicationStartMenuShortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), $"{applicationExecutableBaseName}.lnk");
        _contextualMenuEntries            = config.ContextualMenuEntries;
        
        _applicationUninstallerPath = Path.Combine(_applicationInstallationDirectoryPath, "uninstall.exe");
        _applicationArchivePath     = archivePath;
        _verbose                    = verbose;

        // Prefix the commands specified within the contextual menu entries with the application's executable path.
        if (_contextualMenuEntries is not null) {
            for (var i = 0; i < _contextualMenuEntries.Length; i++) {
                _contextualMenuEntries[i].Command = $"\"{_applicationExecutablePath}\" {_contextualMenuEntries[i].Arguments}";
            }
        }
    }
    
    /// <summary>
    /// Installs the specified application by performing the following steps:
    /// (1) Unpacks the application's ZIP archive to the designated installation directory.
    /// (2) Registers the application in the Windows Registry, providing details such as its name, version, publisher,
    /// installation directory, and uninstaller path.
    /// (3) Configures the contextual menu entries in Windows Explorer if applicable.
    /// (4) Creates a shortcut in the Windows Start Menu pointing to the application's main executable.
    /// This method relies on the <see cref="InstallerTools"/> utility class to handle the core installation tasks.
    /// It ensures that the application's components are correctly deployed, registered, and integrated into the
    /// operating system environment.
    /// In verbose mode, detailed logs are printed to the console during the installation process.
    /// Potential exceptions that occur during the installation process are logged to the console and rethrown to
    /// indicate failure.
    /// </summary>
    /// <exception cref="System.Exception">Thrown if any of the installation steps fail.</exception>
    public void Install() {
        try {
#pragma warning disable CA1416
            if (_verbose) Console.WriteLine("Installing...");
            InstallerTools.UnpackApplication(_applicationArchivePath, _applicationInstallationDirectoryPath, _verbose);
            InstallerTools.RegisterApplication(_applicationRegistryKey, _applicationName, _applicationVersion, _applicationPublisher, _applicationInstallationDirectoryPath, _applicationUninstallerPath, _verbose);
            if (_contextualMenuEntries is not null) {
                InstallerTools.AddExplorerContextualMenuEntries(_contextualMenuEntries, _verbose);
            }
            InstallerTools.AddStartMenuShortcut(_applicationExecutablePath, _applicationIconPath, _applicationStartMenuShortcutPath, _verbose);
            if (_verbose) Console.WriteLine("Installation complete.");
        }
        catch (Exception e) {
            throw new Exception($"An error occurred during the installation process: {e.Message}");    
        }
    }

    /// <summary>
    /// Uninstalls the application by performing a series of cleanup operations.
    /// This includes removing contextual menu entries in Windows Explorer,
    /// removing the application's start menu shortcut, unregistering the application from the registry,
    /// and deleting the application's installation directory files.
    /// </summary>
    /// <exception cref="Exception">
    /// Thrown if an error occurs during any of the uninstallation steps.
    /// </exception>
    public void Uninstall() {
        try {
            if (_contextualMenuEntries is not null) {
                InstallerTools.RemoveExplorerContextualMenuEntries(_contextualMenuEntries, _verbose);    
            }
            InstallerTools.RemoveStartMenuShortcut(_applicationStartMenuShortcutPath, _verbose);
            InstallerTools.UnregisterApplication(_applicationRegistryKey, _verbose);
            InstallerTools.RemoveApplicationFiles(_applicationInstallationDirectoryPath, _verbose);
        }
        catch (Exception e) {
            throw new Exception($"An error occurred during the uninstallation process: {e.Message}");
        }
    }
}