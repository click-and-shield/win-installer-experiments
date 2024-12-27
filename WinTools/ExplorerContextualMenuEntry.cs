﻿namespace WinTools;

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
    public required string FilePattern { get; set; }
    public required string Arguments   { get; set; }
    public  string? Command    { get; set; }
    public required string IconPath    { get; set; }
    public required string Label       { get; set; }
}