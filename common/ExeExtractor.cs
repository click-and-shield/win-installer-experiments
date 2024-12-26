namespace common;

public class ExeExtractor
{
    private readonly string _executablePath;
    private readonly string _destinationPath;
    
    public ExeExtractor(string executablePath, string destinationPath)
    {
        _executablePath = executablePath;
        _destinationPath = destinationPath;
        if (!File.Exists(executablePath)) {
            throw new FileNotFoundException($"the specified path to the executable ({executablePath}) does not exist.");
        }
    }

    public void Run()
    {
        using (var destinationStream = new FileStream(_destinationPath, FileMode.Create, FileAccess.Write))
        using (var executableStream = new FileStream(_executablePath, FileMode.Open, FileAccess.Read))
        {
            // Determine the length of the appended data
            executableStream.Seek(-sizeof(long), SeekOrigin.End);
            var lengthBytes = new byte[sizeof(long)];
            executableStream.ReadExactly(lengthBytes, 0, lengthBytes.Length);
            var lengthOfAddedData = BitConverter.ToInt64(lengthBytes, 0);

            // Determine the starting position of the appended data
            var startPosition = executableStream.Length - lengthOfAddedData - sizeof(long);

            // Retrieve the appended data and store it in the destination file.
            executableStream.Seek(startPosition, SeekOrigin.Begin);
            var buffer = new byte[1024];
            
            var remainder = lengthOfAddedData % buffer.Length;
            var loopCount = (lengthOfAddedData - remainder) / buffer.Length;
            
            for (var i = 0; i < loopCount; i++) {
                executableStream.ReadExactly(buffer, 0, buffer.Length);
                destinationStream.Write(buffer, 0, buffer.Length);
            }
            if (0 == remainder) return;
            executableStream.ReadExactly(buffer, 0, (int)remainder);
            destinationStream.Write(buffer, 0, (int)remainder);
        }
    }
}