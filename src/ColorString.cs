using System.Text;

namespace MarcoZechner.ColorString;

public class ColoredString
{
    public List<ColorMarker> ColorMarkers {
        get => _colorMarkers;
        set => _colorMarkers = value;
    }

    private List<ColorMarker> _colorMarkers = [];

    public string Text {
        get => _text;
        set => _text = value;
    }

    private string _text;

    public ColoredString(string text)
    {
        _text = text;
        _colorMarkers = [new ColorMarker(Color.White, 0)];
    }

    public ColoredString()
    {
        _text = string.Empty;
        _colorMarkers = [new ColorMarker(Color.White, 0)];
    }

    public ColoredString(Color color, string text)
    {
        _text = text;
        _colorMarkers = [new ColorMarker(color, 0)];
    }

    // + operator to concatenate ColoredStrings
    public static ColoredString operator +(ColoredString a, ColoredString b)
    {
        var text = a._text + b._text;
        var colorMarkersB = b._colorMarkers.Select(marker => new ColorMarker(marker.Color, marker.StartIndex + a._text.Length)).ToList();
        List<ColorMarker> colorMarkersA = [..a._colorMarkers];
        colorMarkersB.RemoveAll(marker => colorMarkersA.Any(markerA => markerA.StartIndex == marker.StartIndex));
        var colorMarkers = colorMarkersA.Concat(colorMarkersB).ToList();
        colorMarkers.Sort((a, b) => a.StartIndex.CompareTo(b.StartIndex));
        // remove the 2. marker if it has the same color as the 1. marker of any 2 markers
        for (int i = 0; i < colorMarkers.Count - 1; i++)
        {
            if (colorMarkers[i].Color == colorMarkers[i + 1].Color)
            {
                colorMarkers.RemoveAt(i + 1);
                i--;
            }
        }

        return new ColoredString(text) { _colorMarkers = [.. colorMarkers] };
    }

    // convert string to ColoredString
    public static implicit operator ColoredString(string text)
    {
        return new ColoredString(text);
    }

    public static implicit operator string(ColoredString coloredString)
    {
        return coloredString._text;
    }

    public override string ToString()
    {
        return _text;
    }

    public string ToFormattedString()
    {
        var sb = new StringBuilder();
        var colorMarkers = _colorMarkers.OrderBy(marker => marker.StartIndex);
        var index = 0;
        foreach (var marker in colorMarkers)
        {
            if (index < marker.StartIndex)
            {
                sb.Append(_text.AsSpan(index, marker.StartIndex - index));
                index = marker.StartIndex;
            }
            sb.Append(marker.Color);
        }
        if (index < _text.Length)
            sb.Append(_text.AsSpan(index));
        return sb.ToString();
    }

    #region String Manipulation

    public static ColoredString Join(ColoredString separator, IEnumerable<ColoredString> values)
    {
        List<ColoredString> toJoin = [.. values];
        if (toJoin.Count == 0)
            return new ColoredString();
        ColoredString result = toJoin[0];
        for (int i = 1; i < toJoin.Count; i++)
        {
            result += separator + toJoin[i];
        }
        return result;
    }

    #endregion
}

public record ColorMarker(Color Color, int StartIndex)
{
    public Color Color { get; set; } = Color;
    public int StartIndex { get; set; } = StartIndex;
}
