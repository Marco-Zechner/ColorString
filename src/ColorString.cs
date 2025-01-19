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

    public ColoredString(string text, List<ColorMarker> colorMarkers)
    {
        _text = text;
        _colorMarkers = ValidateColorMarkers(colorMarkers, text.Length);
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
        colorMarkers = ValidateColorMarkers(colorMarkers, text.Length);
        return new ColoredString(text) { _colorMarkers = [.. colorMarkers] };
    }

    private static List<ColorMarker> ValidateColorMarkers(List<ColorMarker> colorMarkers, int textLength) {
        colorMarkers = [.. colorMarkers.Where(marker => marker.StartIndex > 0 && marker.StartIndex < textLength)];
        colorMarkers.Sort((a, b) => a.StartIndex.CompareTo(b.StartIndex));
        for (int i = 0; i < colorMarkers.Count - 1; i++)
        {
            if (colorMarkers[i].Color == colorMarkers[i + 1].Color)
            {
                colorMarkers.RemoveAt(i + 1);
                i--;
            }
            if (colorMarkers[i].StartIndex == colorMarkers[i + 1].StartIndex)
            {
                colorMarkers.RemoveAt(i);
                i--;
            }
        }
        return colorMarkers;
    }

    // convert string to ColoredString
    public static implicit operator ColoredString(string text)
    {
        return new ColoredString(text);
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

    public ColoredString PadRight(int totalWidth)
    {
        return new ColoredString(_text.PadRight(totalWidth)) { _colorMarkers = [.. _colorMarkers] };
    }

    public ColoredString PadLeft(int totalWidth)
    {
        int length = _text.Length;
        string text = _text.PadLeft(totalWidth);
        int diff = totalWidth - length;
        var colorMarkers = _colorMarkers.Select(marker => new ColorMarker(marker.Color, marker.StartIndex + diff)).ToList();
        return new ColoredString(text) { _colorMarkers = [.. colorMarkers] };
    }

    #endregion
}

public record ColorMarker(Color Color, int StartIndex)
{
    public Color Color { get; set; } = Color;
    public int StartIndex { get; set; } = StartIndex;
}
