using content_generator;

if (args.Length != 3) {
    Console.WriteLine("Usage: content-generator <source> <destination> <namespace>");
    return;
}

var source = args[0];
var destination = args[1];
var namespaceName = args[2];

Console.WriteLine($"Generating content from {source} to {destination}");

var generator = new ContentGenerator(source, destination, namespaceName);
generator.Run();

