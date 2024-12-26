// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using common;

try
{
    var executablePath = Process.GetCurrentProcess().MainModule.FileName;
    Console.WriteLine($"Extract the payload from file \"{executablePath}\".");
    var destinationPath = Path.Combine(Path.GetDirectoryName(executablePath)!, "payload.rar");
    var extractor = new ExeExtractor(executablePath, destinationPath);
    extractor.Run();
    Console.WriteLine($"The payload has been successfully extracted into file \"{destinationPath}\".");
}
catch (Exception e)
{
    Console.WriteLine($"An error occurred while extracting the payload : {e.Message}\n\n{e.StackTrace}\n");
}

