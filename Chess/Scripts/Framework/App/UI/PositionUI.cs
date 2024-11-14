namespace Chess.App;

using System.Numerics;
using Raylib_cs;
using Chess.ChessEngine;
using Chess.API;
using System.Security.Cryptography.X509Certificates;

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

    public void SetPiece(int index, Piece piece) {
        pieces[pieces.FindIndex(piece => piece.Coord == new Coord(index))] = new PieceUI(piece, new Coord(index));
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
            // Castle
            if (board.Square[move.Source].IsKing && Math.Abs(move.Source - move.Target) == 2) {
                move = new Move(move.Source, move.Target, Move.Castling);
                int rookSource = move.Target + (move.Target == 62 || move.Target == 6 ? 1 : -2);
                int rookTarget = move.Target + (move.Target == 62 || move.Target == 6 ? -1 : 1);

                int index = pieces.FindIndex(p => p.Coord.SquareIndex == rookSource);
                pieces[index].Coord = new Coord(rookTarget);
                pieces[index].ResetPosition();
            } 
            // Promotion
            if (board.Square[move.Source].IsPawn && (move.Target < 8 || move.Target > 55)) {
                move = new Move(move.Source, move.Target, Move.QueenPromotion);
                
                int index = pieces.FindIndex(p => p.Coord.SquareIndex == move.Target);
                pieces[index] = new PieceUI(new Piece(PieceType.Queen, board.IsWhiteTurn ? PieceType.White : PieceType.Black), new Coord(move.Target));
            }
            // En passant
            if (board.Square[move.Source].IsPawn && Math.Abs(move.Source - move.Target) % 8 != 0 && board.Square[move.Target].IsNone) {
                move = new Move(move.Source, move.Target, Move.EnPassant);

                int target = board.IsWhiteTurn ? move.Target - 8 : move.Target + 8;
                int index = pieces.FindIndex(p => p.Coord.SquareIndex == target);

                pieces.RemoveAt(index);
            } 
            
            board.MakeMove(move);
        }
    }

    public float Distance(float startX, float startY, float endX, float endY) {
        return (float) Math.Sqrt((startX - endX) * (startX - endX) + (startY - endY) * (startY - endY));
    }

    public async Task AnimateMove(Move move, Board board) {
        int index = pieces.FindIndex(p => p.Coord.SquareIndex == move.Source);

        if (pieces.FindIndex(p => p.Coord.SquareIndex == move.Target) != -1) {
            int targetIndex = pieces.FindIndex(p => p.Coord.SquareIndex == move.Target);
            pieces.RemoveAt(targetIndex);
            if (index > targetIndex) index--;
        }
        // Swap the piece at index and the last piece
        var temp = pieces[index];
        pieces[index] = pieces[pieces.Count - 1];
        pieces[pieces.Count - 1] = temp;
        index = pieces.Count - 1;

        // Animate the move
        int frames = 15;
        
        float startX = pieces[index].X;
        float startY = pieces[index].Y;
        float endX = UIHelper.GetScreenX(move.Target % 8);
        float endY = UIHelper.GetScreenY(move.Target / 8);
        float dx = (endX - startX) / frames;
        float dy = (endY - startY) / frames;

        await Task.Run(async () => {
            for (int i = 0; i < frames; i++) {
                pieces[index].X += dx;
                pieces[index].Y += dy;
                await Task.Delay(1000 / 60);
            }
        });

        pieces[index].Coord = new Coord(move.Target);
        pieces[index].ResetPosition();

        await Task.Delay(10);

        // Promotion
        // Replace the pawn with a queen
        // if (move.Flag == Move.QueenPromotion) {
        //     pieces.Add(new PieceUI(new Piece(board.IsWhiteTurn ? PieceType.White : PieceType.Black, PieceType.Queen), new Coord(move.Target)));
        //     pieces.RemoveAt(index);
        // } else if (move.Flag == Move.KnightPromotion) {
        //     pieces.Add(new PieceUI(new Piece(board.IsWhiteTurn ? PieceType.White : PieceType.Black, PieceType.Knight), new Coord(move.Target)));
        //     pieces.RemoveAt(index);
        // } else if (move.Flag == Move.BishopPromotion) {
        //     pieces.Add(new PieceUI(new Piece(board.IsWhiteTurn ? PieceType.White : PieceType.Black, PieceType.Bishop), new Coord(move.Target)));
        //     pieces.RemoveAt(index);
        // } else if (move.Flag == Move.RookPromotion) {
        //     pieces.Add(new PieceUI(new Piece(board.IsWhiteTurn ? PieceType.White : PieceType.Black, PieceType.Rook), new Coord(move.Target)));
        //     pieces.RemoveAt(index);
        // }

        // Castle
        // Move the rook to the other side of the king
        if (move.Flag == Move.Castling) {
            int rookSource = move.Target + (move.Target == 62 || move.Target == 6 ? 1 : -2);
            int rookTarget = move.Target + (move.Target == 62 || move.Target == 6 ? -1 : 1);

            int rookIndex = pieces.FindIndex(p => p.Coord.SquareIndex == rookSource);

            pieces[rookIndex].Coord = new Coord(rookTarget);
            pieces[rookIndex].ResetPosition();
        }

        // En passant
        // Remove the piece that was killed
        if (move.Flag == Move.EnPassant) {
            int target = board.IsWhiteTurn ? move.Target - 8 : move.Target + 8;
            index = pieces.FindIndex(p => p.Coord.SquareIndex == target);
            pieces.RemoveAt(index);
        }
    }

    public void Render() {
        foreach (PieceUI piece in pieces) {
            if (pieces.IndexOf(piece) != draggedPiece) piece.Render();
        }
        if (draggedPiece != -1) pieces[draggedPiece].Render();
    }
}