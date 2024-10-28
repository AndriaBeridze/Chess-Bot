namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;

class BoardUI {
    private Rectangle desk;
    private SquareUI[] squares;

    public BoardUI() {
        desk = new Rectangle(UIHelper.GetScreenX(Theme.IsWhitePerspective ? 0 : 7) - Theme.BorderSize, 
                             UIHelper.GetScreenY(Theme.IsWhitePerspective ? 7 : 0) - Theme.BorderSize, 
                             8 * Theme.SquareSideLength + 2 * Theme.BorderSize, 
                             8 * Theme.SquareSideLength + 2 * Theme.BorderSize
                            );

        squares = new SquareUI[64];
        for (int i = 0; i < 64; i++) {
            squares[i] = new SquareUI(new Coord(i));
        }
    }

    public void SetColor(int index, Color color) {
        squares[index].SetColor(color);
    }

    public void Render() {
        Raylib.DrawRectangleRec(desk, Theme.DeskBackCol);
        for (int i = 0; i < 64; i++) {
            squares[i].Render();
        }
    }
}