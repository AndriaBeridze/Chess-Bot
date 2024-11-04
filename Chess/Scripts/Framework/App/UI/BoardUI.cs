namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;

class BoardUI {
    private Rectangle desk;
    private SquareUI[] squares;
    private Move? lastMove;

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

    public bool IsValidMove(int index) {
        return squares[index].Color.Equals(Theme.LegalLight) || squares[index].Color.Equals(Theme.LegalDark);
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

    public void HighlightValidMoves(List<Move> moves) {
        foreach (Move move in moves) {
            squares[move.EndingCoord.SquareIndex].SetColor(move.EndingCoord.IsLightColor ? Theme.LegalLight : Theme.LegalDark);
        }
    }

    public void HighlightSquare(int index) {
        squares[index].SetColor(new Coord(index).IsLightColor ? Theme.SelectedLight : Theme.SelectedDark);
    }

    public void SetLastMove(Move? move) {
        lastMove = move;
    }

    public void Clear() {
        for (int i = 0; i < 64; i++) {
            squares[i] = new SquareUI(new Coord(i));
        }
        if (lastMove != null) {
            HighlightSquare(lastMove.StartingCoord.SquareIndex);
            HighlightSquare(lastMove.EndingCoord.SquareIndex);
        }
    }
}