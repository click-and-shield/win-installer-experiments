namespace fat_generator;
using System;
using System.IO;

class ExeAppender
{
    private readonly string _executablePath;
    private readonly string _filePathToAdd;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExeAppender"/> class.
    /// </summary>
    /// <param name="executablePath">The path to the executable file to append to.</param>
    /// <param name="filePathToAdd">The path to the file to append.</param>
    /// <exception cref="FileNotFoundException">
    /// Thrown when either the specified path to the executable or the file intended for appending does not exist.
    /// </exception>
    public ExeAppender(string executablePath, string filePathToAdd)
    {
        _executablePath = executablePath;
        _filePathToAdd = filePathToAdd;
        if (!File.Exists(executablePath)) {
            throw new FileNotFoundException($"The specified path to the executable ({executablePath}) does not exist.");
        }
        if (!File.Exists(filePathToAdd)) {
            throw new FileNotFoundException($"the specified file to append({filePathToAdd}) was not found.");
        }
    }
    
    /// <summary>
    /// Appends the content of a specified file to the end of an executable file.
    /// The length of the content being appended is recorded at the end of the executable file.
    /// </summary>
    /// <remarks>
    /// The method reads the entire content of the specified file to append and writes it to the executable file.
    /// After appending the content, it writes the length of the content as a 64-bit integer to the executable file.
    /// If any exceptions occur during this process, they are caught and encapsulated in a new exception with a detailed message.
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when an error occurs during the process of reading from the file to append or writing to the executable file.
    /// Contains the original exception message for further diagnostics.
    /// </exception>
    public void Run()
    {
        try
        {
            using (var contentFileStream = new FileStream(_filePathToAdd, FileMode.Open))
            using (var executableStream = new FileStream(_executablePath, FileMode.Append))
            {
                var bytesToAddLength = contentFileStream.Length;

                var buffer = new byte[bytesToAddLength];
                int count;
                while ((count = contentFileStream.Read(buffer, 0, buffer.Length)) > 0) {
                    executableStream.Write(buffer, 0, count);
                }
                
                var lengthBytes = BitConverter.GetBytes(bytesToAddLength);
                executableStream.Write(lengthBytes, 0, lengthBytes.Length);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"an error occurred while appending the content of file \"{this._filePathToAdd}\" to file \"{this._executablePath}\" : {ex.Message}");
        }
    }
}