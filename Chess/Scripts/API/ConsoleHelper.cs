namespace Chess.API;

class ConsoleHelper {
    public static void WriteColoredText(string text, ConsoleColor color, bool newLine = true) {
        Console.ForegroundColor = color;
        Console.Write(text + (newLine ? Environment.NewLine : ""));
    }
}