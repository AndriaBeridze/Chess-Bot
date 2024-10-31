namespace Chess.App;

using System.Numerics;
using Raylib_cs;
using Chess.ChessEngine;
using Chess.API;

class PositionUI {
    private List<PieceUI> pieces;
    private int draggedPiece = -1;

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
            if (pieces.IndexOf(piece) != draggedPiece) piece.Render();
        }
        if (draggedPiece != -1) pieces[draggedPiece].Render();
    }

    public Move? Update() {
        Move? move = null;
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            int x = Raylib.GetMouseX();
            int y = Raylib.GetMouseY();

            foreach (PieceUI piece in pieces) {
                if (Raylib.CheckCollisionPointRec(new Vector2(x, y), new Rectangle(piece.X, piece.Y, Theme.SquareSideLength, Theme.SquareSideLength))) {
                    draggedPiece = pieces.IndexOf(piece);
                    break;
                }
            }
        }
        if (draggedPiece != -1) {
            pieces[draggedPiece].X = Raylib.GetMouseX() - Theme.SquareSideLength / 2;
            pieces[draggedPiece].Y = Raylib.GetMouseY() - Theme.SquareSideLength / 2;
        }
        if (Raylib.IsMouseButtonReleased(MouseButton.Left)) {
            bool placedOnValidSquare = false;
            for (int i = 0; i < 64; i++) {
                if (Raylib.CheckCollisionPointRec(new Vector2(Raylib.GetMouseX(), Raylib.GetMouseY()), new Rectangle(UIHelper.GetScreenX(i % 8), UIHelper.GetScreenY(i / 8), Theme.SquareSideLength, Theme.SquareSideLength))) {
                    if (draggedPiece != -1) {
                        if (pieces.FindIndex(p => p.Coord == new Coord(i)) != -1) {
                            pieces.RemoveAt(pieces.FindIndex(p => p.Coord == new Coord(i)));
                        }
                        move = new Move(pieces[draggedPiece].Coord, new Coord(i));
                        pieces[draggedPiece].Coord = new Coord(i);
                        pieces[draggedPiece].ResetPosition();
                        placedOnValidSquare = true;
                    }
                }
            }
            if (!placedOnValidSquare && draggedPiece != -1) {
                pieces[draggedPiece].ResetPosition();
            }
            draggedPiece = -1;
        }
        return move;
    }
}