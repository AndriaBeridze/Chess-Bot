namespace Chess.Utility;

class Settings {
    // Screen
    public static int ScreenWidth = 1920;
    public static int ScreenHeight = 1080;

    // Board
    public static bool FromWhitesView = true;

    public static int SquareSideLength = 110;
    public static int BorderSize = 20;

    // Timer
    public static int TimeLimit = (int) (10.0f * 60 * 1000000); // 10 minutes in microseconds (for better precision)
}