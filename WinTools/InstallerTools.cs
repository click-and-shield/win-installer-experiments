using System.Security.AccessControl;

namespace WinTools;

using System;
using Microsoft.Win32;
using System.Runtime.Versioning;
using ShellLink;
using System.IO.Compression;

/// <summary>
/// Provides tools for managing Windows application installations and integrations,
/// including registry registration, explorer contextual menus, and start menu shortcuts.
/// </summary>
[SupportedOSPlatform("windows")]
public class InstallerTools
{
    /// <summary>
    /// Extracts the contents of an application archive (ZIP file) into a specified installation directory.
    /// Ensures that the installation directory is created if it does not already exist.
    /// </summary>
    /// <param name="applicationZipFilePath">The path to the ZIP file containing the application files.</param>
    /// <param name="applicationInstallationDirectoryPath">The target directory where the application files will be extracted.</param>
    /// <param name="verbose">Enables detailed logging of the unpacking process for debugging or monitoring purposes.</param>
    /// <exception cref="Exception">
    /// Thrown if there are issues during the directory creation or the extraction process,
    /// such as invalid file paths, inaccessible files, or other I/O related errors.
    /// </exception>
    public static void UnpackApplication(string applicationZipFilePath, string applicationInstallationDirectoryPath, bool verbose = false) {
        // Create the installation directory if it does not exist.
        if (!Directory.Exists(applicationInstallationDirectoryPath)) {
            if (verbose) Console.WriteLine($"The installation directory does not exist: {applicationInstallationDirectoryPath}. Create it now.");
            try {
                Directory.CreateDirectory(applicationInstallationDirectoryPath);
            }
            catch (Exception e) {
                throw new Exception($"An error occurred while creating the installation directory ({applicationInstallationDirectoryPath}): {e.Message}.");
            }
        }
        
        // Extract every file from the application's archive into the designated installation directory of the application.
        try {
            if (verbose) Console.WriteLine($"Unzip from {applicationZipFilePath} into {applicationInstallationDirectoryPath}");
            ZipFile.ExtractToDirectory(applicationZipFilePath, applicationInstallationDirectoryPath);
        }
        catch (Exception e) {
            throw new Exception($"An error occurred while extracting the application archive ({applicationZipFilePath}) in to the directory \"{applicationInstallationDirectoryPath}\": {e.Message}.");
        }
    }

    /// <summary>
    /// Removes all files and directories for a given application installation directory.
    /// Ensures the directory and its contents are completely deleted.
    /// </summary>
    /// <param name="applicationInstallationDirectoryPath">The path to the directory where the application is installed.</param>
    /// <param name="verbose">Indicates whether detailed logging of the removal process is enabled.</param>
    /// <exception cref="Exception">
    /// Thrown if an error occurs during the deletion process, such as directory access issues or insufficient permissions.
    /// </exception>
    public static void RemoveApplicationFiles(string applicationInstallationDirectoryPath, bool verbose = false) {
        if (verbose) Console.WriteLine($"Remove application from {applicationInstallationDirectoryPath}");
        try {
            Directory.Delete(applicationInstallationDirectoryPath, true);
        }
        catch (Exception e) {
            throw new Exception($"An error occurred while removing the application files from the directory \"{applicationInstallationDirectoryPath}\": {e.Message}.");
        }
    }
    
    /// <summary>
    /// Facilitates integration of the application into the Windows registry, ensuring its presence 
    /// in the Add/Remove Programs menu. This process involves creating a registry entry populated 
    /// with critical details, including the application's display name, version, publisher, 
    /// installation path, and uninstaller location.
    /// </summary>
    /// <param name="applicationRegistryKey">The registry key used to declare the application in the Add/Remove Programs menu.
    /// Typically: $"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{applicationName}"
    /// </param>
    /// <param name="applicationName">The name of the application to be registered.</param>
    /// <param name="applicationVersion">The version number of the application being registered.</param>
    /// <param name="applicationPublisher">The name of the application's publisher or organization.</param>
    /// <param name="applicationInstallationDirectoryPath">The directory where the application is installed.</param>
    /// <param name="applicationUninstallerPath">The path to the application's uninstaller file.</param>
    /// <param name="verbose">Enables verbose mode, providing detailed output for logging and debugging purposes.</param>
    /// <exception cref="Exception">
    /// Thrown when an error is encountered during the creation or modification of the application's registry entry.
    /// </exception>
    public static void RegisterApplication(string applicationRegistryKey, 
                                           string applicationName,
                                           string applicationVersion,
                                           string applicationPublisher, 
                                           string applicationInstallationDirectoryPath, 
                                           string applicationUninstallerPath,
                                           bool verbose = false)
    {
        // Please note that this code is exclusively designed for the Windows platform.
        // To accomplish this, we have incorporated specific configurations within the C# project file: 
        //    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
        // In addition, we have included the following directive at the beginning of this class definition:
        //    [SupportedOSPlatform("windows")]
        
        if (verbose) {
            Console.WriteLine($"Create registry sub-key: \"{applicationRegistryKey}\"");
            Console.WriteLine($"  - set value: \"DisplayName\" = \"{applicationName}\"");
            Console.WriteLine($"  - set value: \"DisplayVersion\" = \"{applicationVersion}\"");
            Console.WriteLine($"  - set value: \"Publisher\" = \"{applicationPublisher}\"");
            Console.WriteLine($"  - set value: \"InstallLocation\" = \"{applicationInstallationDirectoryPath}\"");
            Console.WriteLine($"  - set value: \"UninstallString\" = \"{applicationUninstallerPath}\"");
        }

        try {
            using (var key = Registry.CurrentUser.CreateSubKey(applicationRegistryKey)) {
                key.SetValue("DisplayName", applicationName);
                key.SetValue("DisplayVersion", applicationVersion);
                key.SetValue("Publisher", applicationPublisher);
                key.SetValue("InstallLocation", applicationInstallationDirectoryPath);
                key.SetValue("UninstallString", applicationUninstallerPath);
            }            
        }
        catch (Exception e) {
            throw new Exception($"An error occurred while creating the registry key \"{applicationRegistryKey}\" : {e.Message}");
        }
    }

    /// <summary>
    /// Facilitates integration of the application into the Windows registry, ensuring its presence 
    /// in the Add/Remove Programs menu. This process involves creating a registry entry populated 
    /// with critical details, including the application's display name, version, publisher, 
    /// installation path, and uninstaller location.
    /// </summary>
    /// <remarks>The registry key is automatically generated from the name of the application.</remarks>
    /// <param name="applicationName">The name of the application to be registered.</param>
    /// <param name="applicationVersion">The version number of the application being registered.</param>
    /// <param name="applicationPublisher">The name of the application's publisher or organization.</param>
    /// <param name="applicationInstallationDirectoryPath">The directory where the application is installed.</param>
    /// <param name="applicationUninstallerPath">The path to the application's uninstaller file.</param>
    /// <param name="verbose">Enables verbose mode, providing detailed output for logging and debugging purposes.</param>
    /// <exception cref="Exception">
    /// Thrown when an error is encountered during the creation or modification of the application's registry entry.
    /// </exception>
    public static void RegisterApplication(string applicationName,
                                           string applicationVersion, 
                                           string applicationPublisher,
                                           string applicationInstallationDirectoryPath, 
                                           string applicationUninstallerPath,
                                           bool verbose = false)
    {
        // Please note that this code is exclusively designed for the Windows platform.
        // To accomplish this, we have incorporated specific configurations within the C# project file: 
        //    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
        // In addition, we have included the following directive at the beginning of this class definition:
        //    [SupportedOSPlatform("windows")]
        
        var applicationRegistryKey = @$"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{applicationName}";
        RegisterApplication(applicationRegistryKey, applicationName, applicationVersion, applicationPublisher, applicationInstallationDirectoryPath, applicationUninstallerPath, verbose);
    }
    
    /// <summary>
    /// Removes the application's corresponding registry entry, effectively unregistering the application.
    /// This removes any traces of the application's registry key typically used for identification in
    /// the system, such as in the Add/Remove Programs menu.
    /// </summary>
    /// <param name="applicationRegistryKey">The path or name of the registry key associated with the application to be removed.</param>
    /// <param name="verbose">Enables verbose mode, providing detailed output for logging and debugging purposes.</param>
    /// <exception cref="Exception">
    /// Thrown when an error occurs during the process of removing the specified registry key.
    /// </exception>
    public static void UnregisterApplication(string applicationRegistryKey,
                                             bool verbose = false) {
        if (verbose) {
            Console.WriteLine($"Remove registry sub-key: \"{applicationRegistryKey}\"");
        }
        
        try {
            Registry.CurrentUser.DeleteSubKeyTree(applicationRegistryKey, throwOnMissingSubKey: false);    
        } catch (Exception ex) {
            throw new Exception($"An error occurred while removing the registry key \"{applicationRegistryKey}\" : {ex.Message}");
        }
    }
    
    /// <summary>
    /// Inserts an option into the contextual menu of the Windows File Explorer.
    /// </summary>
    /// <param name="label">The descriptive text displayed in the contextual menu.</param>
    /// <param name="commandToExecute">The command to run.
    /// Example: @'"c:\path\to\app.exe" "param" "%1"'
    /// </param>
    /// <param name="fileType">The file type associated with this menu item.</param>
    /// <param name="iconPath">The path to the icon to display for this menu item.</param>
    /// <param name="verbose">Enables verbose mode, providing detailed output for logging and debugging purposes.</param>
    public static void AddExplorerContextualMenuEntry(string label, 
                                                      string commandToExecute, 
                                                      string fileType,
                                                      string iconPath,
                                                      bool verbose = false) {

        var keyPath = $@"Software\Classes\{fileType}\shell\{label}";
        if (verbose) {
            Console.WriteLine($"Add key: \"{keyPath}\" (add entry to contextual menu)");
            Console.WriteLine($"   Command to execute: \"{commandToExecute}\"");
            Console.WriteLine($"   Icon path: \"{iconPath}\"");
            Console.WriteLine($"   Label: \"{label}\"");
        }

        try {
            using (var shellKey = Registry.CurrentUser.CreateSubKey(keyPath)) {
                if (shellKey is null) {
                    throw new Exception($"Cannot create registry key: \"{keyPath}\"");
                }
                shellKey.SetValue("", label);
                shellKey.SetValue("Icon", iconPath);
                using (var commandKey = shellKey.CreateSubKey("command")) {
                    if (commandKey is null) {
                        throw new Exception($"Cannot create registry key: \"{keyPath}\\command\"");
                    }
                    commandKey.SetValue("", $"{commandToExecute}");
                }
            }
        }
        catch (Exception ex) {
            throw new Exception($"An error occurred while adding the contextual menu entry \"{label}\" : {ex.Message}");
        }
    }
    
    /// <summary>
    /// Removes a specified key from the Windows Registry under the given file type. This operation is
    /// typically used to delete a contextual menu entry associated with the provided label and file type.
    /// </summary>
    /// <param name="label">The label of the contextual menu entry to remove.</param>
    /// <param name="fileType">The file type associated with the contextual menu entry, represented as the registry key path.</param>
    /// <param name="verbose">Enables verbose mode, providing detailed output for logging and debugging purposes.</param>
    /// <exception cref="Exception">
    /// Thrown if the specified registry key does not exist, or if an error occurs while trying to access or modify the registry.
    /// </exception>
    public static void RemoveExplorerContextualMenuEntry(string label, 
                                                         string fileType, 
                                                         bool verbose = false) {
        var keyPath = $@"Software\Classes\{fileType}\shell\{label}";
        if (verbose) {
            Console.WriteLine($"Remove key: \"{keyPath}\" (remove entry from contextual menu)");
        }
        
        try { Registry.CurrentUser.DeleteSubKeyTree(keyPath, throwOnMissingSubKey: false); }
        catch (UnauthorizedAccessException ex) {
            throw new Exception($"The process does not have the permission to remove the \"{label}\" from the Explorer contextual menu : {ex.Message}");
        } catch (Exception ex) {
            throw new Exception($"An error occurred while removing the contextual menu entry \"{label}\" : {ex.Message}");
        }
    }

    /// <summary>
    /// Adds multiple contextual menu entries to the Windows Explorer file context menu. Each entry is defined by a
    /// combination of file pattern, label, command to execute, and optional icon path. This operation allows for
    /// customization of context menus in Windows Explorer for specified file types or patterns.
    /// </summary>
    /// <param name="entries">An array of <c>WinExplorerContextualMenuEntry</c> objects, where each object specifies the
    /// file type, context menu label, associated command to execute, and optional icon path.</param>
    /// <param name="verbose">Enables verbose mode, providing detailed output for logging and debugging purposes.</param>
    /// <exception cref="Exception">
    /// Thrown if any error occurs during the creation of one or more contextual menu entries.
    /// </exception>
    public static void AddExplorerContextualMenuEntries(ExplorerContextualMenuEntry[] entries, bool verbose = false) {
        foreach (var entry in entries) {
            AddExplorerContextualMenuEntry(entry.Label, 
                                           entry.Command,
                                           entry.FilePattern,
                                           entry.IconPath,
                                           verbose);
        }
    }

    /// <summary>
    /// Removes multiple contextual menu entries from Windows Explorer for specific file types.
    /// This operation iterates through a collection of contextual menu entries and removes them
    /// by invoking the removal process for each entry.
    /// </summary>
    /// <param name="entries">An array of contextual menu entries to be removed.
    /// Each entry holds details such as the file type and label for the menu entry.</param>
    /// <param name="verbose">If set to true, provides detailed output for logging and debugging purposes.</param>
    /// <exception cref="Exception">
    /// Thrown if an error occurs while attempting to remove a specific menu entry.
    /// </exception>
    public static void RemoveExplorerContextualMenuEntries(ExplorerContextualMenuEntry[] entries, bool verbose = false) {
        foreach (var entry in entries) {
            RemoveExplorerContextualMenuEntry(entry.Label, 
                                              entry.FilePattern,
                                              verbose);
        }
    }
    
    /// <summary>
    /// Creates a shortcut for the application in the Start Menu. The shortcut will point to the application's
    /// executable file and use the specified icon.
    /// </summary>
    /// <remarks>
    /// The NuGet package "securifybv.ShellLink" has been added as a project dependencies.
    /// For further details, please visit: https://www.nuget.org/packages/securifybv.ShellLink
    /// To add a new dependency:
    ///    1. Right-click on the "Dependencies" tag in the "solution explorer".
    ///    2. Select "Manage NuGet packages".
    /// </remarks>
    /// <param name="applicationExecutablePath">The path to the application's executable file to which the shortcut will point.</param>
    /// <param name="applicationIconPath">The path to the icon file that will be used for the shortcut.</param>
    /// <param name="applicationStartMenuShortcutPath">The fully qualified path for the Start Menu shortcut file to be created.
    /// It should be: `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), $"{applicationName}.lnk")`
    /// </param>
    /// <param name="verbose">Enables verbose mode, displaying detailed information about the shortcut creation process in the console output.</param>
    /// <exception cref="Exception">
    /// Thrown if there is an error during the creation or writing of the Start Menu shortcut.
    /// </exception>
    public static void AddStartMenuShortcut(string applicationExecutablePath, 
                                            string applicationIconPath, 
                                            string applicationStartMenuShortcutPath, 
                                            bool verbose = false) {
        if (verbose) {
            Console.WriteLine($"Create application shortcut \"{applicationStartMenuShortcutPath}\"");
            Console.WriteLine($"  - the path to the actual executable: \"{applicationExecutablePath}\"");
            Console.WriteLine($"  - the path to the icon: \"{applicationIconPath}\"");
            Console.WriteLine($"  - the index of the icon: \"0\"");
        }

        try {
            Shortcut.CreateShortcut(
                                    // Argument 1: The path to the actual executable. This serves as the final destination of the shortcut link.
                                    applicationExecutablePath,
                                    // Argument 2: The path to the icon that will be displayed for the shortcut link.
                                    applicationIconPath,
                                    // Argument 3: The index of the icon in the icon file.
                                    0   
                                   ).WriteToFile(applicationStartMenuShortcutPath);
        }
        catch (Exception e) {
            throw new Exception($"An error occurred while creating the application shortcut \"{applicationStartMenuShortcutPath}\" : {e.Message}");
        }
    }

    /// <summary>
    /// Removes a shortcut from the Start Menu by deleting the specified shortcut file path.
    /// </summary>
    /// <param name="applicationStartMenuShortcutPath">The full file path of the Start Menu shortcut to be removed.
    /// It should be: `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), $"{applicationName}.lnk")`
    /// </param>
    /// <param name="verbose">Enables verbose mode, providing detailed output for logging and debugging purposes.</param>
    /// <exception cref="Exception">
    /// Thrown when an error occurs during the process of removing the Start Menu shortcut.
    /// </exception>
    public static void RemoveStartMenuShortcut(string applicationStartMenuShortcutPath, bool verbose = false) {
        if (verbose) {
            Console.WriteLine($"Remove the application shortcut \"{applicationStartMenuShortcutPath}\"");
        }

        try {
            if (File.Exists(applicationStartMenuShortcutPath)) {
                File.Delete(applicationStartMenuShortcutPath);
            }
        }
        catch (Exception ex) {
            throw new Exception($"An error occurred while removing the Start Menu shortcut \"{applicationStartMenuShortcutPath}\": {ex.Message}");
        }
    }
    

}