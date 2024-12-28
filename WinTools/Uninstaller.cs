namespace WinTools;

using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
public class Uninstaller : InstallerBase
{
    /// <summary>
    /// Represents a utility for uninstalling an application.
    /// It extends the functionality of the InstallerBase,
    /// using configuration data to safely remove all application artifacts
    /// such as registry entries, shortcuts, and installation files.
    /// </summary>
    /// <remarks>
    /// This class is designed to support Windows platforms.
    /// Ensure the correct initialization of configuration data during usage.
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown if an error occurs during the initialization or runtime execution of the uninstaller.
    /// </exception>
    public Uninstaller(string installationPath, bool verbose = false) {
        // Get the path to the uninstaller configuration file.
        var configFilePath = Path.Combine(installationPath, ConfigFileName);
        // Load the configuration from the file.
        var config = _loadConfig(configFilePath);
        
        Init(config, installationPath, verbose);
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
    public void Run() {
        try {
            if (ContextualMenuEntries is not null) {
                InstallerTools.RemoveExplorerContextualMenuEntries(ContextualMenuEntries, Verbose);    
            }
            InstallerTools.RemoveStartMenuShortcut(ApplicationStartMenuShortcutPath, Verbose);
            InstallerTools.UnregisterApplication(ApplicationRegistryKey, Verbose);
            InstallerTools.RemoveApplicationFiles(ApplicationInstallationDirectoryPath, Verbose);
        }
        catch (Exception e) {
            throw new Exception($"An error occurred during the uninstallation process: {e.Message}");
        }
    }
}