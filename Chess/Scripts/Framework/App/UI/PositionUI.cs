namespace Chess.App;

using Raylib_cs;
using Chess.ChessEngine;
using Chess.API;

class PositionUI {
    private List<PieceUI> pieces;

    public PositionUI(Board board) {
        pieces = new List<PieceUI>();
        for (int i = 0; i < 64; i++) {
            if (!board.Square[i].IsNone) {
                pieces.Add(new PieceUI(board.Square[i], new Coord(i)));
            }
        }
    }

    public void Render() {
        foreach (PieceUI piece in pieces) {
            piece.Render();
        }
    }
}