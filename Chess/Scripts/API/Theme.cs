namespace Chess.API;

using Raylib_cs;

// Change this if you want to change the colors, screen resolution, etc.
class Theme {
    public static bool IsWhitePerspective = true;

    public static int ScreenWidth = 2560;
    public static int ScreenHeight = 1440;

    public static int SquareSideLength = 150;
    public static int BorderSize = 20;

    public static Color BackgroundColor = new Color(18, 19, 23, 255);
    
    public static Color DeskBackCol = new Color(36, 38, 46, 255);

    public static Color LightCol = new Color(238, 238, 210, 255);
    public static Color DarkCol = new Color(118, 150, 86, 255);

    public static Color LegalLight = new Color(89, 171, 221, 255);
    public static Color LegalDark = new Color(62, 144, 195, 255);

    public static Color SelectedLight = new Color(245, 246, 130, 255);
    public static Color SelectedDark = new Color(185, 202, 67, 255);

    public static Color CheckmateTextColor = new Color(200, 63, 73, 255);
    public static Color StalemateTextColor = new Color(73, 151, 208, 255);
    public static Color DrawTextColor = new Color(249, 228, 188, 255);

}