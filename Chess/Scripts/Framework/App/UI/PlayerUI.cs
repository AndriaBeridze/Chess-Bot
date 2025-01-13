namespace Chess.App;

using Chess.API;
using Chess.Utility;
using Raylib_cs;
using System.Numerics;

class PlayerUI {
    private string whitePlayer;
    private string blackPlayer;

    public PlayerUI(string whitePlayer, string blackPlayer) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;
    }

    private static int fontSize = 48;
    private static int offset = 10;

    private Font font = UIHelper.LoadFont(fontSize);
    private Color fontColor = new Color(190, 230, 235, 255);

    private void RenderWhite() {
        int spaceBetween = Settings.ScreenHeight / 2 - (Settings.SquareSideLength * 4 + Settings.BorderSize);

        int x, y;

        x = UIHelper.GetScreenX(Settings.FromWhitesView ? 0 : 7);
        if (Settings.FromWhitesView) y = spaceBetween + Settings.SquareSideLength * 8 + Settings.BorderSize * 2 + offset;
        else y = spaceBetween - offset - fontSize;
        
        Raylib.DrawTextEx(font, $"White: { whitePlayer }", new Vector2(x, y), fontSize, 1, fontColor);
    }

    private void RenderBlack() {
        int spaceBetween = Settings.ScreenHeight / 2 - (Settings.SquareSideLength * 4 + Settings.BorderSize);

        int x, y;

        x = UIHelper.GetScreenX(Settings.FromWhitesView ? 0 : 7);
        if (Settings.FromWhitesView) y = spaceBetween - offset + 5 - fontSize;
        else y = spaceBetween + Settings.SquareSideLength * 8 + Settings.BorderSize * 2 + offset;

        Raylib.DrawTextEx(font, $"Black: { blackPlayer }", new Vector2(x, y), fontSize, 1, fontColor);
    }

    public void Render() {
        RenderWhite();
        RenderBlack();
    }
}