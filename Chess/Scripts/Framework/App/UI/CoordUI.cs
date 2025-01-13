namespace Chess.App;

using Chess.API;
using Chess.Utility;
using Raylib_cs;
using System.Numerics;

class CoordUI() {
    private static string rowNames = Settings.FromWhitesView ? "87654321" : "12345678";
    private static string colNames = Settings.FromWhitesView ? "ABCDEFGH" : "HGFEDCBA";

    private static int offset = 5;
    private static int fontSize = 28; // Change this if you want to adjust the font size

    private Font font = UIHelper.LoadFont(fontSize);

    public void Render() {
        rowNames = Settings.FromWhitesView ? "87654321" : "12345678";
        colNames = Settings.FromWhitesView ? "ABCDEFGH" : "HGFEDCBA";
        
        for (int i = 0; i < 8; i++) {
            // Find the i-th square in the bottommost row from the rendering perspective and get its screen coordinates
            Coord coord = new Coord(Settings.FromWhitesView ? -1 : 8, Settings.FromWhitesView ? i + 1 : 6 - i);
            int x = UIHelper.GetScreenX(coord);
            int y = UIHelper.GetScreenY(coord);

            // Add an offset to the x and y coordinates to create a padding effect
            x -= offset + Raylib.MeasureText(colNames[i].ToString(), fontSize);
            y -= offset / 2 + fontSize;

            Raylib.DrawTextEx(font, colNames[i].ToString(), new Vector2(x, y), fontSize, 1, coord.IsLightColor ? Theme.DarkCol : Theme.LightCol);
        }
        for (int i = 0; i < 8; i++) {
            // Find the i-th square in the rightmost column from the rendering perspective and get its screen coordinates
            Coord coord = new Coord(Settings.FromWhitesView ? 7 - i : i, Settings.FromWhitesView ? 0 : 7);
            int x = UIHelper.GetScreenX(coord);
            int y = UIHelper.GetScreenY(coord);

            // Add an offset to the x and y coordinates to create a padding effect
            x += offset;
            y += offset;

            Raylib.DrawTextEx(font, rowNames[i].ToString(), new Vector2(x, y), fontSize, 1, coord.IsLightColor ? Theme.DarkCol : Theme.LightCol);
        }
    }
}