namespace java_launcher_cli;

using System.Diagnostics;
using System;
using common;
using System.Reflection;


internal static class Program {
    [STAThread]
    public static void Main(string[] args) {
        _launchJavaFxApp(args);
    }
    
    private static void _launchJavaFxApp(string[] args) {
        try {
            Console.WriteLine("Launching JavaFX application...");

            var currentDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var configPath       = Path.Combine(currentDirectory, "config.json");
            Console.WriteLine($"Config path: {configPath}");
            
            // ex: encrypt C:\Users\denis\Documents\github\shadow\test-data\input.txt
            var launcher = new JavaLauncher(configPath, args);
            launcher.Launch();
        }
        catch (Exception ex) {
            throw new Exception($"Failed to launch JavaFX application: {ex.Message}", ex);
        }
    }
}
