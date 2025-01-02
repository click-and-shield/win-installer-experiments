namespace WinTools;

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
public class Installer : InstallerBase
{
    /// <summary>
    /// Represents the full file path to the application's archive file.
    /// The archive is a compressed ZIP file that contains all necessary components
    /// required for application installation or deployment.
    /// <remarks>
    /// The path is expected to be absolute and correctly formatted for the underlying operating system.
    /// </remarks>
    /// </summary>
    private readonly string _applicationArchivePath;
    
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
        if (Verbose) Console.WriteLine($"Deleting temporary directory \"{tmpDir}\"...");
        Directory.Delete(tmpDir, recursive: true);
    }
    
    public Installer(string archivePath, string installationPath, bool verbose = false) {
        _applicationArchivePath = archivePath;
        
        // Check if the specified archive path exists.
        if (!File.Exists(archivePath)) {
            throw new Exception($"The specified archive path \"{archivePath}\" does not exist.");
        }
        
        // Load the configuration from the archive containing the application to install.
        var configPath = _getConfigFileFromZip(archivePath, ConfigFileName);
        if (verbose) Console.WriteLine($"Load configuration from \"{configPath}\"");
        var config = _loadConfig(configPath);
        _deleteConfigFile(configPath);
        
        Init(config, installationPath, verbose);
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
    public void Run() {
        try {
#pragma warning disable CA1416
            if (Verbose) Console.WriteLine("Installing...");
            // Create the application's directory and unpack the application archive.
            Directory.CreateDirectory(ApplicationInstallationDirectoryPath);
            InstallerTools.UnpackApplication(_applicationArchivePath, ApplicationInstallationDirectoryPath, Verbose);
            InstallerTools.RegisterApplication(ApplicationRegistryKey, ApplicationName, ApplicationVersion, ApplicationPublisher, ApplicationInstallationDirectoryPath, ApplicationUninstallerPath, Verbose);
            if (ContextualMenuEntries is not null) {
                InstallerTools.AddExplorerContextualMenuEntries(ContextualMenuEntries, Verbose);
            }
            InstallerTools.AddStartMenuShortcut(ApplicationExecutablePath, ApplicationIconPath, ApplicationStartMenuShortcutPath, Verbose);
            if (Verbose) Console.WriteLine("Installation complete.");
        }
        catch (Exception e) {
            throw new Exception($"An error occurred during the installation process: {e.Message}");    
        }
    }
}