namespace MarcoZechner.ColorString;

public static class ColoredConsole
{
    public static void Write(ColoredString colorString, bool resetColor = true)
    {
        foreach (var pair in colorString)
        {
            Console.ForegroundColor = ClosestConsoleColor(pair.Color);
            Console.Write(pair.String);
        }
        if (resetColor)
            Console.ResetColor();
    }

    public static void WriteLine(ColoredString colorString, bool resetColor = true)
    {
        Write(colorString, resetColor);
        Console.WriteLine();
    }

    public static ColoredString ReadLine(){
        string? input = Console.ReadLine();
        if (input == null)
            return new ColoredString(); 
        return new ColoredString(input);
    }

    private static ConsoleColor ClosestConsoleColor(Color color) 
        => ConsoleColorToColor.OrderBy(kvp => Color.ColorDistance(kvp.Value, color)).First().Key;

    private static readonly Dictionary<ConsoleColor, Color> ConsoleColorToColor = new()
    {
        { ConsoleColor.Black, Color.Black },
        { ConsoleColor.DarkBlue, Color.DarkBlue },
        { ConsoleColor.DarkGreen, Color.DarkGreen },
        { ConsoleColor.DarkCyan, Color.DarkCyan },
        { ConsoleColor.DarkRed, Color.DarkRed },
        { ConsoleColor.DarkMagenta, Color.DarkMagenta },
        { ConsoleColor.DarkYellow, Color.DarkYellow },
        { ConsoleColor.Gray, Color.Gray },
        { ConsoleColor.DarkGray, Color.DarkGray },
        { ConsoleColor.Blue, Color.Blue },
        { ConsoleColor.Green, Color.Green },
        { ConsoleColor.Cyan, Color.Cyan },
        { ConsoleColor.Red, Color.Red },
        { ConsoleColor.Magenta, Color.Magenta },
        { ConsoleColor.Yellow, Color.Yellow },
        { ConsoleColor.White, Color.White }
    };
}
