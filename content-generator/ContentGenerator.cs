namespace content_generator;




public class ContentGenerator(string sourcePath, string destinationPath, string namespaceName)
{

    private string _generateBytesSequence(string name, byte[] bytes, int length) {
        var bytesAsString = new List<string>(); 
        
        for (var i = 0; i < length; i++) {
            bytesAsString.Add($"0x{bytes[i]:X2}");
        }        
        
        return $"private static readonly byte[] {name} = [ {string.Join(",", bytesAsString)} ];";
    }
    
    
    public void Run()
    {
        const int bufferSize = 1024;
        
        try
        {
            using (var sourceStream = new FileStream(sourcePath, FileMode.Open))
            using (var outputWriter = new StreamWriter(destinationPath)) {
                var buffer       = new byte[bufferSize];
                int count, index =0;
                var names        = new List<string>();
                var indent       = "   ";
                
                outputWriter.WriteLine("// ReSharper disable all");
                outputWriter.WriteLine($"namespace {namespaceName}");
                outputWriter.WriteLine();
                outputWriter.WriteLine("public class Content {");
                while ((count = sourceStream.Read(buffer, 0, bufferSize)) > 0) {
                    var varName = $"Content{index}";
                    Console.WriteLine($"Generating variable {varName} of size {count}");
                    var bytesAsString = _generateBytesSequence(varName, buffer, count);
                    outputWriter.WriteLine($"{indent}{bytesAsString}");
                    names.Add(varName);
                    index += 1;
                }
                outputWriter.WriteLine();
                outputWriter.WriteLine($"{indent}private readonly byte[][] _content = [ {string.Join(", ", names)} ];");
                outputWriter.WriteLine();
                outputWriter.WriteLine($"{indent}public byte[][] GetContent() => _content;");                
                outputWriter.WriteLine("}");                
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"an error occurred while converting the content of file \"{sourcePath}\" to file \"{destinationPath}\" : {ex.Message}");
        }
    }
}