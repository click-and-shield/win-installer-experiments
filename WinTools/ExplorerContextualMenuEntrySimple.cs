namespace WinTools;

/// <summary>
/// Represents an entry in the Windows Explorer contextual menu. This class encapsulates all the
/// necessary information for defining a contextual menu entry, including the file pattern it applies
/// to, a display label, command-line arguments, and an optional icon name.
/// </summary>
/// <param name="filePattern">The file pattern to match against.
/// Example: "*" (match any file)
/// </param>
/// <param name="label">The display label for the contextual menu entry.</param>
/// <param name="arguments">The command-line arguments to pass to the executable when the contextual menu entry is selected.
/// Example: "\"encrypt\" \"%1\""
/// </param>
/// <param name="iconBaseName">The basename of the icon to display for the contextual menu entry.
/// Example: "app_small.ico"
/// </param>
public class ExplorerContextualMenuEntrySimple(in string filePattern, in string label, in string arguments, in string iconBaseName)
{
    private readonly string _filePattern  = filePattern;
    private readonly string _arguments    = arguments;
    private readonly string _iconBaseName = iconBaseName;
    private readonly string _label        = label;

    public string FilePattern() {
        return _filePattern; 
    }

    public string Arguments() {
        return _arguments; 
    }
    
    public string IconBaseName() {
        return _iconBaseName; 
    }
    
    public string Label() {
        return _label; 
    }
}