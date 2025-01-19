using System.Globalization;

namespace MarcoZechner.ColorString;

public record Color(byte RedValue, byte GreenValue, byte BlueValue, byte AlphaValue = 255) : IFormattable
{
    public byte RedValue { get; set; } = RedValue;
    public byte GreenValue { get; set; } = GreenValue;
    public byte BlueValue { get; set; } = BlueValue;
    public byte AlphaValue { get; set; } = AlphaValue;

    public Color(string pattern) : this(0, 0, 0)
    {
        if (!char.IsNumber(pattern.First()) && !pattern.StartsWith('#'))
            throw new ArgumentException("Invalid color string. Use hex (#RRGGBBAA) or rgb (RRR,GGG,BBB,AAA).");
        byte red;
        byte green;
        byte blue;
        byte alpha;
        if (pattern.StartsWith('#'))
        {
            if (pattern.Length != 7 && pattern.Length != 9)
                throw new ArgumentException("Hex string must be 7 or 9 characters long. (e.g. #FF0000 or #FF0000FF)");

            if (byte.TryParse(pattern.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out red) &&
                byte.TryParse(pattern.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out green) &&
                byte.TryParse(pattern.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out blue))
            {
                RedValue = red;
                GreenValue = green;
                BlueValue = blue;
                AlphaValue = pattern.Length == 9 && byte.TryParse(pattern.AsSpan(7, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out alpha) ? alpha : (byte)255;
                return;
            }

            throw new ArgumentException($"Invalid hex color string: {pattern}");
        }

        string[] parts = pattern.Split(',');
        if (parts.Length != 3 && parts.Length != 4)
            throw new ArgumentException("RGB string must have 3 or 4 number-sections. (e.g. 100,100,100 or 255,0,0,255)");

        if (byte.TryParse(parts[0], out red) &&
            byte.TryParse(parts[1], out green) &&
            byte.TryParse(parts[2], out blue))
        {
            RedValue = red;
            GreenValue = green;
            BlueValue = blue;
            AlphaValue = parts.Length == 4 && byte.TryParse(parts[3], out alpha) ? alpha : (byte)255;
            return;
        }

        throw new ArgumentException($"Invalid RGB color string: {pattern}");
    }

    public Color(Color original){
        RedValue = original.RedValue;
        GreenValue = original.GreenValue;
        BlueValue = original.BlueValue;
        AlphaValue = original.AlphaValue;
    }

    public Color LayerWith(Color topColor)
    {
        // Normalize alpha values to 0–1 for computation
        float alphaBase = AlphaValue / 255f;
        float alphaTop = topColor.AlphaValue / 255f;

        // Compute resulting alpha
        float resultAlpha = alphaTop + alphaBase * (1 - alphaTop);

        if (resultAlpha == 0)
        {
            // Fully transparent result
            return new Color(255, 255, 255, 0);
        }

        // Compute each color channel
        byte resultRed = (byte)((topColor.RedValue * alphaTop + RedValue * alphaBase * (1 - alphaTop)) / resultAlpha);
        byte resultGreen = (byte)((topColor.GreenValue * alphaTop + GreenValue * alphaBase * (1 - alphaTop)) / resultAlpha);
        byte resultBlue = (byte)((topColor.BlueValue * alphaTop + BlueValue * alphaBase * (1 - alphaTop)) / resultAlpha);

        // Scale alpha back to 0–255
        byte resultAlphaByte = (byte)(resultAlpha * 255);

        return new Color(resultRed, resultGreen, resultBlue, resultAlphaByte);
    }

    public static double ColorDistance(Color color1, Color color2)
    {
        int rDiff = color1.RedValue - color2.RedValue;
        int gDiff = color1.GreenValue - color2.GreenValue;
        int bDiff = color1.BlueValue - color2.BlueValue;

        return Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
    }

    public override string ToString()
    {
        return ToString("hex");
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (format == null)
            return ToString();

        return format switch
        {
            "hex" => $"#{RedValue:X2}{GreenValue:X2}{BlueValue:X2}",
            "hex4" => $"#{RedValue:X2}{GreenValue:X2}{BlueValue:X2}{AlphaValue:X2}",
            "rgb" => $"rgb({RedValue}, {GreenValue}, {BlueValue})",
            "rgba" => $"rgba({RedValue}, {GreenValue}, {BlueValue}, {AlphaValue})",
            _ => throw new ArgumentException("Invalid format string. Use 'hex', 'hex4', 'rgb' or 'rgba'.")
        };
    }

    public static implicit operator string(Color color)
    {
        return color.ToString();
    }

    public ColoredString For(string text)
    {
        return new(this, text);
    }

    /// <summary>
    /// #000000, Black, rgb(0, 0, 0)
    /// </summary>
    public static Color Black => new(0, 0, 0);
    /// <summary>
    /// #000080, DarkBlue, rgb(0, 0, 128)
    /// </summary>
    public static Color DarkBlue => new(0, 0, 128);
    /// <summary>
    /// #008000, DarkGreen, rgb(0, 128, 0)
    /// </summary>
    public static Color DarkGreen => new(0, 128, 0);
    /// <summary>
    /// #008080, DarkCyan, rgb(0, 128, 128)
    /// </summary>
    public static Color DarkCyan => new(0, 128, 128);
    /// <summary>
    /// #800000, DarkRed, rgb(128, 0, 0)
    /// </summary>
    public static Color DarkRed => new(128, 0, 0);
    /// <summary>
    /// #800080, DarkMagenta, rgb(128, 0, 128)
    /// </summary>
    public static Color DarkMagenta => new(128, 0, 128);
    /// <summary>
    /// #808000, DarkYellow, rgb(128, 128, 0)
    /// </summary>
    public static Color DarkYellow => new(128, 128, 0);
    /// <summary>
    /// #808080, Gray, rgb(128, 128, 128)
    /// </summary>
    public static Color Gray => new(128, 128, 128);
    /// <summary>
    /// #A9A9A9, DarkGray, rgb(169, 169, 169)
    /// </summary>
    public static Color DarkGray => new(169, 169, 169);
    /// <summary>
    /// #0000FF, Blue, rgb(0, 0, 255)
    /// </summary>
    public static Color Blue => new(0, 0, 255);
    /// <summary>
    /// #00FF00, Green, rgb(0, 255, 0)
    /// </summary>
    public static Color Green => new(0, 255, 0);
    /// <summary>
    /// #00FFFF, Cyan, rgb(0, 255, 255)
    /// </summary>
    public static Color Cyan => new(0, 255, 255);
    /// <summary>
    /// #FF0000, Red, rgb(255, 0, 0)
    /// </summary>
    public static Color Red => new(255, 0, 0);
    /// <summary>
    /// #FF00FF, Magenta, rgb(255, 0, 255)
    /// </summary>
    public static Color Magenta => new(255, 0, 255);
    /// <summary>
    /// #FFFF00, Yellow, rgb(255, 255, 0)
    /// </summary>
    public static Color Yellow => new(255, 255, 0);
    /// <summary>
    /// #FFFFFF, White, rgb(255, 255, 255)
    /// </summary>
    public static Color White => new(255, 255, 255);
}