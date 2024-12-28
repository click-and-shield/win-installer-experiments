namespace WinTools;

using System.Runtime.Versioning;
using System.Text.Json;

[SupportedOSPlatform("windows")]
public class InstallerBase
{
    protected const string ConfigFileName = "config-install.json";
    
    /// <summary>
    /// Represents the file system path to the directory where the application is installed.
    /// This variable is utilized to determine or access the installation directory of the application
    /// during setup, maintenance, or uninstallation processes.
    /// </summary>
    protected string ApplicationInstallationDirectoryPath;
    /// <summary>
    /// Represents the name of the application.
    /// This attribute is used to identify the application uniquely, often for display purposes,
    /// configuration, or during the installation process.
    /// </summary>
    protected string ApplicationName;
    /// <summary>
    /// Represents the version of the application.
    /// This information is used to indicate the specific iteration or release
    /// of the application, which can help in version tracking, updates, or compatibility checks.
    /// </summary>
    protected string ApplicationVersion;
    /// <summary>
    /// Represents the name of the publisher of the application.
    /// This information is typically used to identify and attribute
    /// the organization or individual responsible for creating or distributing the application.
    /// </summary>
    protected string ApplicationPublisher;
    /// <summary>
    /// Represents the absolute file path to the application's executable file.
    /// This path is used to locate and launch the main application binary.
    /// </summary>
    protected string ApplicationExecutablePath;
    /// <summary>
    /// Represents the absolute file path to the application's icon file.
    /// This path is used to locate the icon associated with the application, typically used
    /// for display purposes such as desktop shortcuts or in the start menu.
    /// <remarks>
    /// The icon file is generally an .ico file and is located within the application's
    /// installation directory.
    /// </remarks>
    /// </summary>
    protected string ApplicationIconPath;
    /// <summary>
    /// Represents the absolute file path to the application's uninstaller executable.
    /// This path is used to locate and invoke the uninstaller, enabling proper removal
    /// of the application from the user's system.
    /// <remarks>
    /// The uninstaller file is typically located in the application's installation directory
    /// and named "Uninstall.exe" by default.
    /// </remarks>
    /// </summary>
    protected string ApplicationUninstallerPath;
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
    protected string ApplicationRegistryKey;
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
    protected string ApplicationStartMenuShortcutPath;
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
    protected ExplorerContextualMenuEntry[]? ContextualMenuEntries;
    /// <summary>
    /// Determines whether detailed logs and messages are displayed during the installer's execution.
    /// When set to true, additional information is output to the console to aid in debugging or
    /// providing progress updates during the installation process.
    /// </summary>
    protected bool Verbose;

    /// <summary>
    /// Loads and deserializes the configuration file from the given path.
    /// </summary>
    /// <param name="configPath">The path to the configuration file to be loaded.</param>
    /// <returns>An instance of <see cref="InstallerConfig"/> populated with the data from the configuration file.</returns>
    /// <exception cref="Exception">
    /// Thrown if the configuration file cannot be read, deserialized, or does not contain valid configuration data.
    /// </exception>
    protected static InstallerConfig _loadConfig(string configPath) {
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

    protected void Init(InstallerConfig config, string installationPath, bool verbose = false) {
        // Extract the configuration values from the loaded configuration.
        ApplicationInstallationDirectoryPath = installationPath;
        config.PrefixRootPath(ApplicationInstallationDirectoryPath);
        var applicationExecutableBaseName = Path.GetFileNameWithoutExtension(config.ExecutablePath);
        
        ApplicationName                  = config.Name;
        ApplicationVersion               = config.Version;
        ApplicationPublisher             = config.Publisher;
        ApplicationIconPath              = config.IconPath;
        ApplicationExecutablePath        = config.ExecutablePath;
        ApplicationRegistryKey           = $@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{config.RegId}";
        ApplicationStartMenuShortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), $"{applicationExecutableBaseName}.lnk");
        ContextualMenuEntries            = config.ContextualMenuEntries;
        
        ApplicationUninstallerPath = Path.Combine(ApplicationInstallationDirectoryPath, "uninstall.exe");
        Verbose                    = verbose;
        
        // Prefix the commands specified within the contextual menu entries with the application's executable path.
        if (ContextualMenuEntries is not null) {
            for (var i = 0; i < ContextualMenuEntries.Length; i++) {
                ContextualMenuEntries[i].Command = $"\"{ApplicationExecutablePath}\" {ContextualMenuEntries[i].Arguments}";
            }
        }
    }

    
}