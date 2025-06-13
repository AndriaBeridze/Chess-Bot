namespace Chess.Utility;

class Settings {
    // Screen
    public static int ScreenWidth = 1920;
    public static int ScreenHeight = 1080;

    // Board
    public static bool FromWhitesView = true;

    public static int SquareSideLength = 110;
    public static int BorderSize = 20;

    // Board Margin
    public static int BoardMarginLeft = (ScreenWidth - 8 * SquareSideLength) / 2;

    // Timer
    public static float TimeLimit = 20f * 60; // 10 minutes in seconds
}
