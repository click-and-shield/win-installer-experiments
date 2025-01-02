namespace WinTools;

/// <summary>
/// Represents an entry in a Windows Explorer contextual menu.
/// </summary>
/// <remarks>
/// A contextual menu entry is used to define custom menu items that appear in the Windows Explorer
/// context menu for specific file patterns. These entries can include a label, an associated command to execute,
/// and an optional icon to display.
/// </remarks>
/// <param name="filePattern">The file pattern to match against.
/// Example: "*" (match any file)
/// </param>
/// <param name="label">The display label for the contextual menu entry.</param>
/// <param name="command">The command to execute when the contextual menu entry is selected.
/// <remarks>
/// Please note that the command specifies the full absolute path to the application's executable, along with any
/// additional parameters that may be required.
/// Example: "/path/to/application.exe" "encrypt" "%1" 
/// </remarks>
/// </param>
/// <param name="iconPath">The absolute path to the icon to display for the contextual menu entry.</param>
public class ExplorerContextualMenuEntry {
    /// <summary>
    /// Gets or sets the file pattern that specifies the types of files or file names
    /// to which the contextual menu entry should be applied.
    /// This can include wildcards or specific file extensions to filter applicable files.
    /// </summary>
    public required string FilePattern { get; set; }

    /// <summary>
    /// Gets or sets the arguments to be passed to the command when the contextual menu entry is selected.
    /// These arguments are typically used as additional parameters for the executable specified in the command.
    /// </summary>
    public required string Arguments   { get; set; }

    /// <summary>
    /// Gets or sets the command to be executed when the contextual menu entry is selected.
    /// Typically, this is the executable file path, followed by the arguments (given by the entry "Arguments").
    /// </summary>
    public string? Command { get; set; }

    /// <summary>
    /// Gets or sets the path to the icon associated with the contextual menu entry.
    /// The icon path is used to visually represent the menu entry in the user interface,
    /// typically pointing to a file containing an image resource.
    /// </summary>
    public required string IconPath { get; set; }

    /// <summary>
    /// Gets or sets the label text displayed in the context menu entry within the Windows Explorer.
    /// This text serves as the visible name of the menu option users interact with.
    /// </summary>
    public required string Label { get; set; }
}