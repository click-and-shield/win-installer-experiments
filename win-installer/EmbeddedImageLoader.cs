namespace win_installer;
using System;
using System.Drawing;
using System.Reflection;

public class EmbeddedImageLoader
{
    public static Image LoadEmbeddedImage(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new Exception($"Resource not found : {resourceName}");
        return Image.FromStream(stream);
    }
}

