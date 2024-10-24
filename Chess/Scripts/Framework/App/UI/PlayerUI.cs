namespace Chess.App;

using Raylib_cs;
using Chess.API;
using System.Numerics;

class PlayerUI {
    private string whitePlayer;
    private string blackPlayer;

    public PlayerUI(string whitePlayer, string blackPlayer) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;
    }

    private static int fontSize = 60;
    private static int offset = 10;

    private Font font = UIHelper.LoadFont(fontSize);
    private Color fontColor = new Color(190, 230, 235, 255);

    private void RenderWhite() {
        int spaceBetween = Theme.ScreenHeight / 2 - (Theme.SquareSideLength * 4 + Theme.BorderSize);

        int x, y;
        if (Theme.IsWhitePerspective) {
            x = UIHelper.GetScreenX(Theme.IsWhitePerspective ? 0 : 7);
            y = spaceBetween + Theme.SquareSideLength * 8 + Theme.BorderSize * 2 + offset;
        } else {
            x = UIHelper.GetScreenX(Theme.IsWhitePerspective ? 0 : 7);
            y = spaceBetween - offset + 5 - fontSize;
        }

        Raylib.DrawTextEx(font, $"White: { whitePlayer }", new Vector2(x, y), fontSize, 1, fontColor);
    }

    private void RenderBlack() {
        int spaceBetween = Theme.ScreenHeight / 2 - (Theme.SquareSideLength * 4 + Theme.BorderSize);

        int x, y;
        if (Theme.IsWhitePerspective) {
            x = UIHelper.GetScreenX(Theme.IsWhitePerspective ? 0 : 7);
            y = spaceBetween - offset + 5 - fontSize;
        } else {
            x = UIHelper.GetScreenX(Theme.IsWhitePerspective ? 0 : 7);
            y = spaceBetween + Theme.SquareSideLength * 8 + Theme.BorderSize * 2 + offset;
        }

        Raylib.DrawTextEx(font, $"Black: { blackPlayer }", new Vector2(x, y), fontSize, 1, fontColor);
    }

    public void Render() {
        RenderWhite();
        RenderBlack();
    }
}