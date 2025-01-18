using System.Collections;

namespace MarcoZechner.ColorString;

public class ColoredString : IEnumerable<ColorStringPair>, IFormattable
{
    private readonly List<ColorStringPair> colorStringPairs = [];
    public int TextLength => colorStringPairs.Sum(pair => pair.String.Length);
    public string Text => ToString();
    public int Length => colorStringPairs.Count;
    public ColorStringPair this[int index] {
        get {
            ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length, nameof(index));
            return colorStringPairs[index];
        }
        set {
            ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length, nameof(index));
            colorStringPairs[index] = value;
        }
    }

    public ColoredString() { }

    public ColoredString(ColoredString original){
        colorStringPairs.AddRange(original.colorStringPairs.Select(pair => new ColorStringPair(new Color(pair.Color), pair.String)));
    } 

    // Constructor accepting a plain string (no color)
    /// <summary>
    /// <para>Defining color in the string is done by using the following pattern: "<c>>[colorPattern]followingColoredText</c>"<br></br>
    /// colorPattern can be either a hex color "<c>#RRGGBBAA</c>" or an rgb color "<c>RRR,GGG,BBB,AAA</c>", <c>AA/AAA</c> is optional. <br></br>
    /// Example: "<c>>[#FF0000]Hello, World!</c>" or "<c>>[255,0,0]Hello, World!</c>" </para>
    /// <para><b>Escaping</b> the color pattern is done by using the following pattern: "<c>>>[...</c>"<br></br>
    /// Example: "<c>>>[#FF0000]Hello, World!</c>" </para>
    /// </summary>
    /// <param name="text"></param>
    public ColoredString(string text)
    {
        colorStringPairs = ParseString(text);
    }

    // Constructor accepting a color and string
    public ColoredString(Color color, string text)
    {
        colorStringPairs = ParseString(text, color);
    }

    // Implicit conversion from string to ColorString
    public static implicit operator ColoredString(string text)
    {
        return new ColoredString(text);
    }

    // Concatenation operator
    public static ColoredString operator +(ColoredString cs1, ColoredString cs2)
    {
        var result = new ColoredString(cs1);
        result.colorStringPairs.AddRange(cs2.colorStringPairs);
        return result;
    }

    public static implicit operator string(ColoredString v)
    {
        return v.ToString();
    }

    private static List<ColorStringPair> ParseString(string text, Color? startColor = null) {
        var colorStringPairs = new List<ColorStringPair>();
        var color = startColor ?? Color.White;
        var str = "";
        var i = 0;
        while (i < text.Length) {
            if (text[i] != '>' || i == text.Length - 1 || text[i + 1] != '[')
            {
                str += text[i++];
                continue;
            }

            if (i > 0 && text[i - 1] == '>') {
                i++;
                continue;
            }

            if (str.Length > 0)
            {
                colorStringPairs.Add(new ColorStringPair(color, str));
                str = "";
            }

            var j = i + 2;
            var colorPattern = "";
            while (j < text.Length)
            {
                if (text[j] == ']')
                {
                    break;
                }
                if (j >= text.Length - 1)
                {
                    throw new ArgumentException("Invalid color string. Could not find end of pattern. Use hex >[#RRGGBBAA] or rgb >[RRR,GGG,BBB,AAA].");
                }
                colorPattern += text[j];
                j++;
            }
            if (j >= text.Length -1)
                break;
            color = new Color(colorPattern);
            i = j + 1;
        }
        if (str.Length > 0) {
            colorStringPairs.Add(new ColorStringPair(color, str));
        }
        return colorStringPairs;
    }

    public override string ToString()
    {
        return ToString(null);
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        return format switch
        {
            "color" => string.Concat(colorStringPairs.Select(pair => $"{pair.Color:color}{pair.String}")),
            _ => string.Concat(colorStringPairs.Select(pair => pair.String)),
        };
    }

    public IEnumerator<ColorStringPair> GetEnumerator()
    {
        return colorStringPairs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ColoredString other)
            return false;
        if (Length != other.Length)
            return false;
        for (int i = 0; i < Length; i++)
        {
            if (!this[i].Equals(other[i]))
                return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(colorStringPairs.GetHashCode());
    }

    public static bool operator ==(ColoredString left, ColoredString right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ColoredString left, ColoredString right)
    {
        return !(left == right);
}
}

public class ColorStringPair(Color color, string str)
{
    public Color Color { get; set; } = color;
    public string String { get; set; } = str;

    public ColorStringPair(ColorStringPair original) : this(new Color(original.Color), original.String){ }

    public override bool Equals(object? obj)
    {
        if (obj is not ColorStringPair other)
            return false;
        return Color.Equals(other.Color) && String.Equals(other.String);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Color.GetHashCode(), String.GetHashCode());
    }

    public static bool operator ==(ColorStringPair left, ColorStringPair right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ColorStringPair left, ColorStringPair right)
    {
        return !(left == right);
    }
}
