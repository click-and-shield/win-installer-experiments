// To compile the application into a standalone binary follow this procedure:
//     "Build => Build Clean".
//     "Build => Build Solution".
//     Delete the content of the directory "bin/Release/net9.0/win-x64/publish".
//     dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
//
// Please be advised that the executable will be located in the "bin/Release/net9.0/win-x64/publish" directory.

using WinTools;

void Main(string[] args) {
    var          applicationName        = "beeper";
    const string iconEncryptName        = "icon-encrypt.ico";
    const string iconDecryptName        = "icon-decrypt.ico";
    const string applicationArchivePath = "Z:\\beeper.zip";
    var entries = new[]
                  { new ExplorerContextualMenuEntrySimple("*", "encrypt", "\"encrypt\" \"%1%\"", iconEncryptName),
                    new ExplorerContextualMenuEntrySimple("*.skv", "decrypt", "\"decrypt\" \"%1%\"", iconDecryptName) };

    if (0 == args.Length) {
        Console.WriteLine("Usage: installer.exe (install | uninstall)");
        Environment.Exit(1);
    }

    
#pragma warning disable CA1416
    var installer = new InstallerSimple(applicationName, 
                                       "1.0.0", 
                                       "Denis",
                                       applicationArchivePath,
                                       entries);
    if ("install" == args[0]) {
        
        installer.Install();    
    }
    else {
        installer.Uninstall();
    }
}

Main(args);

    
