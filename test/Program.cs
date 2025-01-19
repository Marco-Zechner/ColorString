using MarcoZechner.ColorString;

namespace MarcoZechner.Test;

public class Program
{
    public static void Main()
    {
        // ColoredString coloredString = "\nDefault String\n";
        // ColoredString coloredString2 = new(new Color("#FF0000"), "Hello, ");
        // ColoredString coloredString3 = Color.Green.For("World!");
        // ColoredString coloredString4 = coloredString + coloredString2;
        // ColoredString coloredString5 = "Hello, " + coloredString3;
        // ColoredString coloredString6 = coloredString4 + new Color("255,255,0").For("World!");
        // Color myFavoriteColor = new("#FF00FF");
        // ColoredString interpolatedString = myFavoriteColor.For($"This my favorite color: {myFavoriteColor:rgba}");

        // Console.Write(coloredString5); // Hello, World!
        // Console.WriteLine(coloredString6); // \nDefault String\nHello, World!
        // ColoredConsole.Write(coloredString5);  // White:Hello, Green:World!
        // ColoredConsole.WriteLine(coloredString6); // White:\nDefault String\nRed:Hello, Yellow:World!

        // ColoredConsole.WriteLine(interpolatedString); // Magenta:This my favorite color: rgba(255, 0, 255, 255)

        
        // ColoredString testColorString = "Hello " + Color.Blue.For("World!");
        // testColorString.Text = testColorString.Text.ToUpper();
        // // replace all colors that are not blue with red
        // testColorString.ColorMarkers = [.. testColorString.ColorMarkers.Select(marker => marker.Color != Color.Blue ? new ColorMarker(Color.Red, marker.StartIndex) : marker)];

        // ColoredConsole.WriteLine(testColorString); // Red:HELLO Blue:WORLD!

        // string test = testColorString.ToFormattedString();
        // Console.WriteLine(test); // #FF0000HELLO #0000FFWORLD!
        // testColorString = test;

        // ColoredConsole.WriteLine(testColorString); // #FF0000HELLO #0000FFWORLD!

        // test = $"{testColorString}";
        // Console.WriteLine(test); // #FF0000HELLO #0000FFWORLD!
        // testColorString = test;

        // ColoredConsole.WriteLine(testColorString); // #FF0000HELLO #0000FFWORLD!

        // ColoredConsole.WriteLine(Color.Yellow.For("Hello, World!")); // Yellow:Hello, World!

        ColoredString coloredString = new("Hello, World!");
        coloredString = coloredString.Replace("World", Color.Blue.For("Universe"));
        ColoredConsole.WriteLine(coloredString); // White:Hello, Blue:Universe!

        coloredString = "Hello " + Color.Red.For("Word") + "!";
        coloredString = coloredString.Replace("or", Color.Blue.For("orl"));
        ColoredConsole.WriteLine(coloredString); // White:Hello Red:W Blue:orl Red:d White:!

        ColoredString[] parts = coloredString.Split("l", stringSplitOptions: StringSplitOptions.RemoveEmptyEntries);
        foreach (ColoredString part in parts)
        {
            ColoredConsole.WriteLine(part);
        }
    }
}