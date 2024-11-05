namespace Chess.App;

using Raylib_cs;
using Chess.API;

class SquareUI {
    public Coord Coord;
    public Color Color;

    // Custom color is set
    public SquareUI(Coord coord, Color color) {
        this.Coord = coord;
        this.Color = color;
    }

    // Default color is set based on the square's position
    public SquareUI(Coord coord) {
        this.Coord = coord;
        Color = coord.IsLightColor ? Theme.LightCol : Theme.DarkCol;
    }

    public void SetColor(Color color) {
        this.Color = color;
    }

    public void Render() {
        int x = UIHelper.GetScreenX(Coord.ColumnIndex);
        int y = UIHelper.GetScreenY(Coord.RowIndex);

        Raylib.DrawRectangle(x, y, Theme.SquareSideLength, Theme.SquareSideLength, Color);
    }
}