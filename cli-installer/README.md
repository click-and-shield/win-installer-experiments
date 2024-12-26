# Creating a Self-Contained Executable

There are two main approaches to generate a self-contained executable:

1. **Command Line Method**
2. **Rider Interface Method**

## Command Line Method

To create a self-contained executable using the command line, please follow these instructions:

1. Begin by cleaning up any previous build artifacts: navigate to the `Build` menu and select `Clean Solution`.
2. Next, compile your solution by selecting `Build` and then `Build Solution`.
3. Empty the contents of the `bin/Release/net9.0/win-x64/publish` directory.
4. Execute the following command:

   ```shell
   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
   ```

   Once complete, you will find the executable within the `bin/Release/net9.0/win-x64/publish` directory.

## Rider Interface Method

To create a self-contained executable using Rider's graphical interface:

1. Right-click on the project within the solution explorer.
2. Choose `Publish...`
3. Click on `To folder`.
4. Configure the publication settings as illustrated in the accompanying screenshot.

![](doc/images/publish%20config.png)

# Ask for administrator authorization

1. Create the file `app.manifest` in the project folder.

   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <!-- This document was crafted with the assistance of JetBrain AI. -->
   <assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
       <trustInfo xmlns="urn:schemas-microsoft-com:asm.v3">
           <security>
               <!-- Request administrative privileges upon application launch. -->
               <requestedPrivileges>
                   <requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
               </requestedPrivileges>
           </security>
       </trustInfo>
   </assembly>
   ```
2. Declare the file `app.manifest` in the file `installer/installer.csproj`:

   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
       
       <PropertyGroup>
           <OutputType>Exe</OutputType>
           <TargetFramework>net9.0</TargetFramework>
           <RootNamespace>installer</RootNamespace>
           <ImplicitUsings>enable</ImplicitUsings>
           <Nullable>enable</Nullable>
           <RuntimeIdentifier>win-x64</RuntimeIdentifier>
           <ApplicationManifest>app.manifest</ApplicationManifest>
       </PropertyGroup>
       
       <ItemGroup>
         <PackageReference Include="securifybv.ShellLink" Version="0.1.0" />
       </ItemGroup>
   
       <ItemGroup>
         <ProjectReference Include="..\common\common.csproj" />
       </ItemGroup>
   
       <ItemGroup>
         <Folder Include="doc\images\" />
       </ItemGroup>
   
   </Project>
   ```

> In the solution explorer, right click ont the project and select "`Edit`".
> Then click on "`Edit installer.csproj`".

# Search the registry for a given key

```
Get-ChildItem -Recurse 'HKCU:Software\Classes\*\shell\encrypt'
Get-ChildItem -Recurse 'HKCU:Software\Classes\*.skv\shell\decrypt'
Get-ChildItem -Recurse 'HKCU:SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall'
```

> See: https://www.sans.org/blog/month-of-powershell-working-with-the-registry/

# VM Configuration

## Add the guest additions for copy/past between the host the the guest

1. Add an optical device to the VM: "`Machine`" => "`Configuration`" => "`Stockage`" (let it empty)
2. "`Périphériques`" => "`Upgrade Guest Additions`"
3. "`Périphériques`" => "`Insérer l'image des Additions invité`" (then exécute the CD)
4. "`Périphériques`" => "`Presse-papier partagé`"

> See: https://www.techrepublic.com/article/how-to-enable-copy-and-paste-in-virtualbox/

# Prompt

> Je souhaite ajouter une entrée au menu contextuel de l'explorateur de fichier de Windows.
>
> Je souhaite que lorsque l'utilisateur clique avec le bouton droit sur un fichier, une entrée apparaisse dans le menu contextuel de l'explorateur de fichier de Windows.
>
> Je souhaite que cette entrée présente une petite icône et un label.
>
> Peux-tu me fournir le code C# qui permet de réaliser cette opération ?
> 
> Merci

```csharp
using System;
using Microsoft.Win32;

class Program
{
    static void Main()
    {
        // Nom de l'application pour le menu contextuel
        string menuText = "Votre Libellé"; // Remplacez par votre texte
        string command = @"C:\Chemin\Vers\VotreApplication.exe"; // Remplacez par le chemin de votre exécutable
        string fileType = "*"; // Utilisez "*" pour tous les types de fichiers

        // Chemin de la clé pour ajouter l'entrée au menu contextuel
        string keyPath = $@"Software\Classes\{fileType}\shell\{menuText}";

        try
        {
            // Ouvrir ou créer la clé de registre sous "shell"
            using (RegistryKey shellKey = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (shellKey != null)
                {
                    shellKey.SetValue("", menuText);

                    // Clé command pour exécuter votre programme
                    using (RegistryKey commandKey = shellKey.CreateSubKey("command"))
                    {
                        if (commandKey != null)
                        {
                            commandKey.SetValue("", $"\"{command}\" \"%1\"");
                        }
                    }
                }
            }

            Console.WriteLine("L'entrée du menu contextuel a été ajoutée avec succès.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
        }
    }
}
```

# Prompt

> Je souhaite ajouter une entrée au menu contextuel de l'explorateur de fichier de Windows.
>
> Je souhaite que lorsque l'utilisateur clique avec le bouton droit sur un fichier, une entrée apparaisse dans le menu contextuel de l'explorateur de fichier de Windows.
>
> Je souhaite que cette entrée présente une petite icône et un label.
>
> Quel format d'image faut-il utiliser pour la petite icône ?
>
> Merci
