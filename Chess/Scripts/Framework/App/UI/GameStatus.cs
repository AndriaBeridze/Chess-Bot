namespace Chess.API;

using System.IO.Compression;
using System.Numerics;
using Raylib_cs;

public class GameStatus {
    // This is to display the game status on the screen
    // Possible values are "Checkmate", "Stalemate", "Draw"
    public string Text;
    public Color Color = Color.White;

    private Font font;
    private const int fontSize = 90;

    public GameStatus(string text) {
        this.Text = text;
        this.font = UIHelper.LoadFont(fontSize);
    }

    public void Render() { 
        Vector2 textSize = Raylib.MeasureTextEx(font, this.Text, fontSize, 1);
        int x = (Theme.ScreenWidth / 2 - 4 * Theme.SquareSideLength - Theme.BorderSize) / 2 - (int)textSize.X / 2;
        int y = Theme.ScreenHeight / 2 - fontSize / 2;

        Raylib.DrawTextEx(font, this.Text, new Vector2(x, y), fontSize, 1, Color);
    }
}