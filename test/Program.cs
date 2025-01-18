using MarcoZechner.ColorString;

namespace MarcoZechner.Test;

public class Program
{
    public static void Main()
    {
        ColoredString coloredString = "\nDefault String\n";
        ColoredString coloredString2 = new(new Color("#FF0000"), "Hello, ");
        ColoredString coloredString3 = ">[#00FF00FF]World!";
        ColoredString coloredString4 = coloredString + coloredString2;
        ColoredString coloredString5 = "Hello, " + coloredString3;
        ColoredString coloredString6 = coloredString4 + ">[255,255,0]World!";
        ColoredString escapedString =  ">>[#00FF00FF]World!";
        Color myFavoriteColor = new("#FF00FF");
        ColoredString interpolatedString = $"{myFavoriteColor:color}This my favorite color!";
        ColoredString interpolatedStringWrong = $"{myFavoriteColor}This my favorite color!";

        Console.Write(coloredString5); // Writes only the text without any color
        Console.WriteLine(coloredString6); // Writes only the text without any color
        Console.WriteLine(escapedString); // Writes only the text without any color
        ColoredConsole.Write(coloredString5); // Writes the text correctly colored
        ColoredConsole.WriteLine(coloredString6); // Writes the text correctly colored
        ColoredConsole.WriteLine(escapedString); // Writes the text correctly colored

        ColoredConsole.WriteLine(interpolatedString); // Writes the text correctly colored
        ColoredConsole.WriteLine(interpolatedStringWrong); // Writes the text without any color

        
        ColoredString testColorString = "Hello >[#0000FF]World!";
        for (int i = 0; i < testColorString.Length; i++)
        {
            var pair = testColorString[i];
            if (pair.Color != Color.Blue)
                pair.Color = new Color("#FF0000");
            pair.String = pair.String.ToUpper();
            testColorString[i] = pair;           
        }

        ColoredConsole.WriteLine(testColorString); // Writes the text correctly colored and uppercased

        string test = testColorString.ToString("color");
        Console.WriteLine(test); // Writes the text correctly colored and uppercased
        testColorString = test;

        ColoredConsole.WriteLine(testColorString); // Writes the text correctly colored and uppercased

        test = testColorString;
        Console.WriteLine(test); // Writes the text correctly colored and uppercased
        testColorString = test;

        ColoredConsole.WriteLine(testColorString); // Writes the text correctly colored and uppercased
    }
}