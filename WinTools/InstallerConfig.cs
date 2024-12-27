namespace WinTools;

public class InstallerConfig
{
    public required string                         Name                  { get; set; }
    public required string                         Version               { get; set; }
    public required string                         Publisher             { get; set; }
    public required string                         IconPath              { get; set; }
    public required string                         ExecutablePath        { get; set; }
    public required string                         RegId                 { get; set; }
    public required ExplorerContextualMenuEntry[]? ContextualMenuEntries { get; set; }


    public void Check() {
        _checkContextualMenuEntries();
    }
    
    private void _checkContextualMenuEntries() {
        if (ContextualMenuEntries is null) {
            return;
        }
        for (var i = 0; i < ContextualMenuEntries.Length; i++) {
            if (ContextualMenuEntries[i] is null) {
                throw new Exception($"Contextual menu entry #{i} is null!");
            }
        }
    }
    
    public void PrefixRootPath(string rootPath) {
        ExecutablePath = Path.Combine(rootPath, ExecutablePath);
        IconPath       = Path.Combine(rootPath, IconPath);


        if (ContextualMenuEntries is null) {
            return;
        }
        
        for (var i = 0; i < ContextualMenuEntries.Length; i++) {
            if (!Path.IsPathRooted(ContextualMenuEntries[i].IconPath)) {
                ContextualMenuEntries[i].IconPath = Path.Combine(rootPath, ContextualMenuEntries[i].IconPath);
            }
        }
    }
}