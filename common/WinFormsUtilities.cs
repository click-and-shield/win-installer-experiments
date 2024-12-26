namespace common;
using System.Drawing;

public static class WinFormsUtilities
{
    /// <summary>
    /// Converts a given Color object to its hexadecimal string representation.
    /// The resulting string includes the '#' prefix, followed by the RGB components
    /// in hexadecimal format.
    /// </summary>
    /// <param name="color">The Color object to convert to hexadecimal format.</param>
    /// <returns>A string representing the hexadecimal format of the given color.</returns>
    public static string ColorToHex(Color color) {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}