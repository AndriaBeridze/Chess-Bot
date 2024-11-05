namespace Chess.App;

using System.Numerics;
using Raylib_cs;
using Chess.ChessEngine;
using Chess.API;

class PositionUI {
    private List<PieceUI> pieces;
    private int draggedPiece = -1; // To keep track of the piece during dragging

    public PositionUI(Board board) {
        pieces = new List<PieceUI>();
        SetUpPosition(board);
    }

    // Creating new pieces to render them on the screen
    public void SetUpPosition(Board board) {
        pieces = new List<PieceUI>();
        for (int i = 0; i < 64; i++) {
            if (!board.Square[i].IsNone) {
                pieces.Add(new PieceUI(board.Square[i], new Coord(i)));
            }
        }
    }

    public void Update(Board board, BoardUI boardUI) {
        Move? move = null;
        
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            int x = Raylib.GetMouseX();
            int y = Raylib.GetMouseY();

            foreach (PieceUI piece in pieces) {
                // If mouse is not hovering on a piece, ignore it
                Rectangle rect = new Rectangle(piece.X, piece.Y, Theme.SquareSideLength, Theme.SquareSideLength);
                if (!Raylib.CheckCollisionPointRec(new Vector2(x, y), rect)) continue;
                    
                draggedPiece = pieces.IndexOf(piece); // Drag the piece

                // If the piece is of the current player, highlight its valid moves
                if (board.Square[piece.Coord.SquareIndex].IsWhite == board.IsWhiteTurn) {
                    boardUI.HighlightValidMoves(MoveGenerator.GenerateMoves(board, piece.Coord.SquareIndex));
                    boardUI.HighlightSquare(piece.Coord.SquareIndex);
                }

                break; // To avoid dragging multiple pieces
            }
        }

        // Center the piece on the mouse cursor
        if (draggedPiece != -1) {
            pieces[draggedPiece].X = Raylib.GetMouseX() - Theme.SquareSideLength / 2;
            pieces[draggedPiece].Y = Raylib.GetMouseY() - Theme.SquareSideLength / 2;
        }

        if (Raylib.IsMouseButtonReleased(MouseButton.Left)) {
            bool placedOnValidSquare = false;

            for (int i = 0; i < 64; i++) {
                // If the mouse is not hovering on a square, ignore it
                Rectangle rect = new Rectangle(UIHelper.GetScreenX(i % 8), UIHelper.GetScreenY(i / 8), Theme.SquareSideLength, Theme.SquareSideLength);
                if (!Raylib.CheckCollisionPointRec(new Vector2(Raylib.GetMouseX(), Raylib.GetMouseY()), rect)) continue;
                
                // If the piece is not dragged or the square is not a valid move, ignore
                if (draggedPiece == -1 || !boardUI.IsValidToMove(i)) continue;

                // If other piece was killed, remove it from the list
                if (pieces.FindIndex(p => p.Coord == new Coord(i)) != -1) {
                    int index = pieces.FindIndex(p => p.Coord == new Coord(i));
                    pieces.RemoveAt(index);
                    if (draggedPiece > index) draggedPiece--;
                }

                // Update and record the data
                move = new Move(pieces[draggedPiece].Coord, new Coord(i));
                boardUI.SetLastMove(move);

                pieces[draggedPiece].Coord = new Coord(i);
                pieces[draggedPiece].ResetPosition();
                
                placedOnValidSquare = true;
            }

            if (!placedOnValidSquare && draggedPiece != -1) {
                pieces[draggedPiece].ResetPosition(); // If it is illegal to move on that square, reset the piece to its original position
            }

            // Reset data
            draggedPiece = -1;
            boardUI.Clear();
        }
        if (move != null) {
            board.MakeMove(move);
        }
    }

    public void Render() {
        foreach (PieceUI piece in pieces) {
            if (pieces.IndexOf(piece) != draggedPiece) piece.Render();
        }
        if (draggedPiece != -1) pieces[draggedPiece].Render();
    }
}