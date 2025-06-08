namespace Chess.UI;

using Chess.API;
using Chess.Utility;
using Raylib_cs;

class Square {
    public Coord Coord;
    public Color Color;

    // Custom color is set
    public Square(Coord coord, Color color) {
        this.Coord = coord;
        this.Color = color;
    }

    // Default color is set based on the square's position
    public Square(Coord coord) {
        this.Coord = coord;
        Color = coord.IsLightColor ? Theme.LightCol : Theme.DarkCol;
    }

    public void SetColor(Color color) {
        this.Color = color;
    }

    public void Render() {
        int x = UIHelper.GetScreenX(Coord.ColumnIndex);
        int y = UIHelper.GetScreenY(Coord.RowIndex);

        Raylib.DrawRectangle(x, y, Settings.SquareSideLength, Settings.SquareSideLength, Color);
    }
}