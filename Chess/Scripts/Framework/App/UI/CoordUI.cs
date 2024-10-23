namespace Chess.App;

using System.Numerics;
using Raylib_cs;
using Chess.API;

class CoordUI() {
    private static string rowNames = Theme.IsWhitePerspective ? "87654321" : "12345678";
    private static string colNames = Theme.IsWhitePerspective ? "ABCDEFGH" : "HGFEDCBA";

    private static int offset = 5;
    private static int fontSize = 30;

    private Font font = UIHelper.LoadFont(fontSize);

    public void Render() {
        for (int i = 0; i < 8; i++) {
            Coord coord = new Coord(Theme.IsWhitePerspective ? i + 1 : 6 - i, Theme.IsWhitePerspective ? -1 : 8);
            int x = UIHelper.GetScreenX(coord);
            int y = UIHelper.GetScreenY(coord);

            x -= offset + Raylib.MeasureText(colNames[i].ToString(), fontSize);
            y -= offset / 2 + fontSize;

            Raylib.DrawTextEx(font, colNames[i].ToString(), new Vector2(x, y), fontSize, 1, coord.IsLightColor ? Theme.DarkCol : Theme.LightCol);
        }
        for (int i = 0; i < 8; i++) {
            Coord coord = new Coord(Theme.IsWhitePerspective ? 0 : 7, Theme.IsWhitePerspective ? 7 - i : i);
            int x = UIHelper.GetScreenX(coord);
            int y = UIHelper.GetScreenY(coord);

            x += offset;
            y += offset;

            Raylib.DrawTextEx(font, rowNames[i].ToString(), new Vector2(x, y), fontSize, 1, coord.IsLightColor ? Theme.DarkCol : Theme.LightCol);
        }
    }
}