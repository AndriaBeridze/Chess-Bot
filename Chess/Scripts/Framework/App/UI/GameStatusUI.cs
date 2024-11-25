namespace Chess.API;

using Raylib_cs;
using System.Numerics;

public class GameStatusUI {
    // This is to display the game status on the screen
    // Possible values are "Checkmate", "Stalemate", "Draw"
    private string text;
    private Color color = Color.White;

    private Font font;
    private const int fontSize = 90;

    public GameStatusUI(string text) {
        this.text = text;
        this.font = UIHelper.LoadFont(fontSize);
    }

    public GameStatusUI(string text, Color color) {
        this.text = text;
        this.color = color;
        this.font = UIHelper.LoadFont(fontSize);
    }

    public void Render() { 
        Vector2 textSize = Raylib.MeasureTextEx(font, this.text, fontSize, 1);
        int x = 3 * (Theme.ScreenWidth / 2 - 4 * Theme.SquareSideLength - Theme.BorderSize) / 2 + 8 * Theme.SquareSideLength + 2 * Theme.BorderSize - (int) textSize.X / 2;
        int y = Theme.ScreenHeight / 2 - fontSize / 2;

        Raylib.DrawTextEx(font, this.text, new Vector2(x, y), fontSize, 1, color);
    }
}