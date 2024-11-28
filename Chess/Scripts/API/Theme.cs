namespace Chess.API;

using Raylib_cs;

// Change this if you want to change the colors, screen resolution, etc.
class Theme {
    // Screen
    public static int ScreenWidth = 1920;
    public static int ScreenHeight = 1080;

    // Board
    public static bool FromWhitesView = true;

    public static int SquareSideLength = 110;
    public static int BorderSize = 20;

    public static Color BackgroundColor = new Color(20, 20, 20, 255);
    
    public static Color DeskBackCol = new Color(36, 38, 46, 255);

    public static Color LightCol = new Color(238, 216, 192, 255);
    public static Color DarkCol = new Color(171, 122, 101, 255);

    public static Color LegalLight = new Color(89, 171, 221, 255);
    public static Color LegalDark = new Color(62, 144, 195, 255);

    public static Color SelectedLight = new Color(221, 208, 124, 255);
    public static Color SelectedDark = new Color(197, 158, 94, 255);

    // Game Status
    public static Color CheckmateTextColor = new Color(200, 63, 73, 255);
    public static Color StalemateTextColor = new Color(73, 151, 208, 255);
    public static Color DrawTextColor = new Color(249, 228, 188, 255);

    // Timer
    public static int TimeLimit = (int) (30.0f * 60 * 1000000); // 10 minutes in microseconds (for better precision)
    public static Color TimerColorLightActive = new Color(200, 200, 200, 255);
    public static Color TimerColorDarkActive = new Color(55, 55, 55, 255);
    public static Color TimerColorLightDisabled = new Color(200, 200, 200, 150);
    public static Color TimerColorDarkDisabled = new Color(55, 55, 55, 150);

    // Data
    public static Color DepthDataColor = new Color(200, 63, 73, 255);
    public static Color EvalDataColor = new Color(73, 151, 208, 255);

    // Buttons
    public static Color ButtonColor = new Color(50, 50, 50, 255);
    public static Color ButtonTextColor = new Color(205, 205, 205, 255);
    public static Color ButtonHoverColor = new Color(50, 125, 233, 255);
    public static Color ButtonHoverTextColor = new Color(25, 25, 25, 255);
}