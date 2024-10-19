namespace Chess.App;

using Raylib_cs;
using Chess.API;

class SquareUI {
    private Coord coord;
    private Color color;

    public SquareUI(Coord coord, Color color) {
        this.coord = coord;
        this.color = color;
    }

    public SquareUI(Coord coord) {
        this.coord = coord;
        color = coord.IsLightColor ? Theme.LightCol : Theme.DarkCol;
    }

    public Coord Coord => coord;
    public Color Color => color;

    public void SetColor(Color color) {
        this.color = color;
    }

    public void Render() {
        int x = UIHelper.GetScreenX(coord.ColumnIndex);
        int y = UIHelper.GetScreenY(coord.RowIndex);

        Raylib.DrawRectangle(x, y, Theme.SquareSideLength, Theme.SquareSideLength, color);
    }
}