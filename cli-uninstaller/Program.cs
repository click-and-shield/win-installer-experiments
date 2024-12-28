// To compile the application into a standalone binary follow this procedure:
//     "Build => Build Clean".
//     "Build => Build Solution".
//     Delete the content of the directory "bin/Release/net9.0/win-x64/publish".
//     dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
//
// Please be advised that the executable will be located in the "bin/Release/net9.0/win-x64/publish" directory.

namespace cli_uninstaller;
using System.Diagnostics;
using System.Runtime.Versioning;
using WinTools;

[SupportedOSPlatform("windows")]
static class Program
{
    [STAThread]
    public static void Main()
    {
        var executablePath      = Process.GetCurrentProcess().MainModule.FileName;
        var installationDirPath = Path.GetDirectoryName(executablePath);
        var uninstaller         = new Uninstaller(installationDirPath, verbose: true);
        uninstaller.Run();
    }
}
