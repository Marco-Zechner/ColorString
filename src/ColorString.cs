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

    public ColoredString(List<ColorMarker> colorMarkers, string text)
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
        colorMarkers = [.. colorMarkers.Where(marker => marker.StartIndex >= 0 && marker.StartIndex < textLength)];
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

    public static implicit operator ColoredString(char character)
    {
        return new ColoredString(character.ToString());
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

    public ColoredString Replace(string oldValue, ColoredString newValue) {
        int accoumulatedOffset = 0;
        var colorMarkers = new List<ColorMarker>();
        string text = string.Empty;
        for (int i = 0; i < _text.Length; i++)
        {
            if (_text[i..].StartsWith(oldValue))
            {
                var lastMarker = colorMarkers.LastOrDefault(marker => marker.StartIndex < text.Length);
                colorMarkers.AddRange(newValue.ColorMarkers.Select(marker => new ColorMarker(marker.Color, marker.StartIndex + text.Length)));
                if (lastMarker != null)
                    colorMarkers.Add(new ColorMarker(lastMarker.Color, text.Length + newValue.Text.Length));
                text += newValue.Text;
                i += oldValue.Length - 1;
                accoumulatedOffset += newValue.Text.Length - oldValue.Length;
            }
            else
            {
                text += _text[i];
                var marker = _colorMarkers.FirstOrDefault(marker => marker.StartIndex == i);
                if (marker != null)
                    colorMarkers.Add(new ColorMarker(marker.Color, marker.StartIndex + accoumulatedOffset));
            }
        }
        return new ColoredString(colorMarkers, text);
    }

    public ColoredString[] Split(string[] separator, int count = 0, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
    {
        List<ColoredString> result = [];
        ColoredString textSection = new();
        ColorMarker? lastMarker = null;
        for (int i = 0; i < _text.Length; i++)
        {
            string? foundSeperator = separator.ToList().FirstOrDefault(sep => sep != null && _text[i..].StartsWith(sep), null);

            if (foundSeperator != null)
            {
                if (stringSplitOptions == StringSplitOptions.TrimEntries)
                    textSection = textSection.Trim();

                if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries || textSection.Text.Length > 0) {
                    result.Add(textSection);
                }

                // Early exit if we reached the max count
                if (result.Count == count -1) {
                    textSection = new ColoredString(_text[(i + foundSeperator.Length)..]);
                    if (lastMarker != null)
                        textSection.ColorMarkers.Add(new ColorMarker(lastMarker.Color, textSection.Text.Length));
                    textSection.ColorMarkers = [.. _colorMarkers.Select(marker => new ColorMarker(marker.Color, marker.StartIndex - i - foundSeperator.Length)).Where(marker => marker.StartIndex >= 0)];
                    
                    if (stringSplitOptions == StringSplitOptions.TrimEntries)
                        textSection = textSection.Trim();

                    if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries || textSection.Text.Length > 0) {
                        result.Add(textSection);
                    }
                    return [.. result];
                }

                // Prepare next text section and continue the previous color if present
                textSection = new ColoredString();
                i += foundSeperator.Length - 1;
                if (lastMarker != null)
                    textSection.ColorMarkers.Add(new ColorMarker(lastMarker.Color, textSection.Text.Length));
            }
            else
            {
                var marker = _colorMarkers.FirstOrDefault(marker => marker.StartIndex == i);
                if (marker != null) {
                    textSection.ColorMarkers.Add(new ColorMarker(marker.Color, textSection.Text.Length));
                    lastMarker = marker;
                }
                textSection.Text += _text[i];
            }
        }
        if (stringSplitOptions == StringSplitOptions.TrimEntries)
            textSection = textSection.Trim();

        if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries || textSection.Text.Length > 0) {
            result.Add(textSection);
        }

        return [.. result];
    }

    public ColoredString[] Split(char[] separator, int count = 0, StringSplitOptions stringSplitOptions = StringSplitOptions.None) 
        => Split(separator.Select(c => c.ToString()).ToArray(), count, stringSplitOptions);

    public ColoredString[] Split(params string[] separator) => Split(separator, 0, StringSplitOptions.None);

    public ColoredString[] Split(params char[] separator) => Split(separator, 0, StringSplitOptions.None);

    public ColoredString[] Split(string separator, int count = 0, StringSplitOptions stringSplitOptions = StringSplitOptions.None) 
        => Split([separator], count, stringSplitOptions);

    public ColoredString[] Split(char separator, int count = 0, StringSplitOptions stringSplitOptions = StringSplitOptions.None)	
        => Split([separator], count, stringSplitOptions);

    public ColoredString Trim() => TrimStart().TrimEnd();

    public ColoredString TrimStart()
    {
        int startIndex = 0;
        while (startIndex < _text.Length && char.IsWhiteSpace(_text[startIndex]))
            startIndex++;
        ColorMarker? lastWhiteSpaceMarker = _colorMarkers.LastOrDefault(marker => marker.StartIndex < startIndex);
        var trimmedString = new ColoredString(_text[startIndex..]) { _colorMarkers = [.. _colorMarkers.Select(marker => new ColorMarker(marker.Color, marker.StartIndex - startIndex)).Where(marker => marker.StartIndex >= 0)] };
        if (trimmedString.ColorMarkers.OrderBy(marker => marker.StartIndex).FirstOrDefault()?.StartIndex > 0 && lastWhiteSpaceMarker != null)
            trimmedString.ColorMarkers.Add(new ColorMarker(lastWhiteSpaceMarker.Color, 0));
        trimmedString.ColorMarkers = ValidateColorMarkers(trimmedString.ColorMarkers, trimmedString.Text.Length);
        return trimmedString;
    }

    public ColoredString TrimEnd()
    {
        int endIndex = _text.Length - 1;
        while (endIndex >= 0 && char.IsWhiteSpace(_text[endIndex]))
            endIndex--;
        return new ColoredString(_text[..(endIndex + 1)]) { _colorMarkers = [.. _colorMarkers.Where(marker => marker.StartIndex <= endIndex)] };
    }

    #endregion
}

public record ColorMarker(Color Color, int StartIndex)
{
    public Color Color { get; set; } = Color;
    public int StartIndex { get; set; } = StartIndex;
}
