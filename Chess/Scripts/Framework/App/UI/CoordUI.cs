namespace Chess.App;

using System.Numerics;
using Raylib_cs;
using Chess.API;

class CoordUI() {
    private static string rowNames = Theme.IsWhitePerspective ? "87654321" : "12345678";
    private static string colNames = Theme.IsWhitePerspective ? "ABCDEFGH" : "HGFEDCBA";

    private static int offset = 5;
    private static int fontSize = 35; // Change this if you want to adjust the font size

    private Font font = UIHelper.LoadFont(fontSize);

    public void Render() {
        for (int i = 0; i < 8; i++) {
            // Find the i-th square in the bottommost row from the rendering perspective and get its screen coordinates
            Coord coord = new Coord(Theme.IsWhitePerspective ? -1 : 8, Theme.IsWhitePerspective ? i + 1 : 6 - i);
            int x = UIHelper.GetScreenX(coord);
            int y = UIHelper.GetScreenY(coord);

            // Add an offset to the x and y coordinates to create a padding effect
            x -= offset + Raylib.MeasureText(colNames[i].ToString(), fontSize);
            y -= offset / 2 + fontSize;

            Raylib.DrawTextEx(font, colNames[i].ToString(), new Vector2(x, y), fontSize, 1, coord.IsLightColor ? Theme.DarkCol : Theme.LightCol);
        }
        for (int i = 0; i < 8; i++) {
            // Find the i-th square in the rightmost column from the rendering perspective and get its screen coordinates
            Coord coord = new Coord(Theme.IsWhitePerspective ? 7 - i : i, Theme.IsWhitePerspective ? 0 : 7);
            int x = UIHelper.GetScreenX(coord);
            int y = UIHelper.GetScreenY(coord);

            // Add an offset to the x and y coordinates to create a padding effect
            x += offset;
            y += offset;

            Raylib.DrawTextEx(font, rowNames[i].ToString(), new Vector2(x, y), fontSize, 1, coord.IsLightColor ? Theme.DarkCol : Theme.LightCol);
        }
    }
}