namespace java_launcher_gui_win;
using System.Diagnostics;
using System.IO;
using System;
using System.Windows.Forms;
using common;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        _launchJavaFxApp(args);
        Application.Exit();
    }
    
    private static void _launchJavaFxApp(string[] args)
    {
        try
        {
            var currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var configPath       = Path.Combine(currentDirectory, "config.json");
           
            var launcher = new JavaLauncher(configPath, args);
            launcher.Launch();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to launch JavaFX application: {ex.Message}", ex);
        }
    }
}
