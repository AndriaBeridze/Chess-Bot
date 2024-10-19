

namespace Chess.API;

class UIHelper {
    public static void WriteColoredText(string text, ConsoleColor color, bool newLine = true) {
        Console.ForegroundColor = color;
        Console.Write(text + (newLine ? Environment.NewLine : ""));
    }

    public static int GetScreenX(int x) {
        return Theme.ScreenWidth / 2 - (Theme.IsWhitePerspective ? (4 - x) : (x - 3)) * Theme.SquareSideLength;
    }

    public static int GetScreenY(int y) {
        return Theme.ScreenHeight / 2 - (Theme.IsWhitePerspective ? (y - 3) : (4 - y)) * Theme.SquareSideLength;
    }
}