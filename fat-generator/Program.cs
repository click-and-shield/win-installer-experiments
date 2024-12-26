using System.Reflection;
using fat_generator;
using common;

Main(args);
return;

void Append(string executablePath, string filePathToAdd) {
    Console.WriteLine($"Append the content of file \"{filePathToAdd}\" to file \"{executablePath}\".");
    var appender = new ExeAppender(executablePath, filePathToAdd);
    appender.Run();
    Console.WriteLine("The content of the file has been successfully appended.");
}

void Extract(string executablePath, string destinationPath) {
    Console.WriteLine($"Extract the payload from the executable \"{executablePath}\" into \"{destinationPath}\".");
    var extractor = new ExeExtractor(executablePath, destinationPath);
    extractor.Run();
    Console.WriteLine("The payload has been successfully extracted.");
}

void Main(string[] args) {
    // Check the command line arguments.
    if (args.Length != 3) {
        var programName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        Console.WriteLine($"Usage 1: {programName} append <executable file> <file to append>");
        Console.WriteLine($"Usage 2: {programName} extract <executable file> <destination file>");
        return; 
    }
    
    var command = args[0];

    switch (command)
    {
        case "append":
            try { Append(args[1], args[2]); } catch (Exception e) {
                Console.WriteLine($"An error occurred while appending the content of file \"{args[1]}\" to file \"{args[0]}\" : {e.Message}");
            } break;
        case "extract":
            try { Extract(args[1], args[2]); } catch (Exception e) {
                Console.WriteLine($"An error occurred while extracting the payload from the executable \"{args[1]}\": {e.Message}\n\n{e.StackTrace}");
            } break;
        default: Console.WriteLine($"Unknown command: {command}"); break;
    }
}
