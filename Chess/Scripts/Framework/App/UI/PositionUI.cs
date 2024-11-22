namespace Chess.App;

using System.Numerics;
using Raylib_cs;
using Chess.ChessEngine;
using Chess.API;
using System.Security.Cryptography.X509Certificates;

class PositionUI {
    public List<PieceUI> Pieces;
    private int draggedPiece = -1; // To keep track of the piece during dragging
    private int animatedPiece = -1;

    public PositionUI(Board board) {
        Pieces = new List<PieceUI>();
        SetUpPosition(board);
    }

    // Creating new pieces to render them on the screen
    public void SetUpPosition(Board board) {
        Pieces = new List<PieceUI>();
        for (int i = 0; i < 64; i++) {
            if (!board.Square[i].IsNone) {
                Pieces.Add(new PieceUI(board.Square[i], new Coord(i)));
            }
        }
    }

    public void SetPiece(int index, Piece piece) {
        Pieces[Pieces.FindIndex(piece => piece.Coord == new Coord(index))] = new PieceUI(piece, new Coord(index));
    }

    public void Update(Board board, BoardUI boardUI, bool highlightMoves) {
        Move? move = null;
        
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            int x = Raylib.GetMouseX();
            int y = Raylib.GetMouseY();

            foreach (PieceUI piece in Pieces) {
                // If mouse is not hovering on a piece, ignore it
                Rectangle rect = new Rectangle(piece.X, piece.Y, Theme.SquareSideLength, Theme.SquareSideLength);
                if (!Raylib.CheckCollisionPointRec(new Vector2(x, y), rect)) continue;
                    
                draggedPiece = Pieces.IndexOf(piece); // Drag the piece

                // If the piece is of the current player, highlight its valid moves
                if (board.Square[piece.Coord.SquareIndex].IsWhite == board.IsWhiteTurn && highlightMoves) {
                    boardUI.HighlightValidMoves(MoveGenerator.GenerateMoves(board, piece.Coord.SquareIndex));
                    boardUI.HighlightSquare(piece.Coord.SquareIndex);
                }

                break; // To avoid dragging multiple pieces
            }
        }

        // Center the piece on the mouse cursor
        if (draggedPiece != -1) {
            Pieces[draggedPiece].X = Raylib.GetMouseX() - Theme.SquareSideLength / 2;
            Pieces[draggedPiece].Y = Raylib.GetMouseY() - Theme.SquareSideLength / 2;
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
                if (Pieces.FindIndex(p => p.Coord == new Coord(i)) != -1) {
                    int index = Pieces.FindIndex(p => p.Coord == new Coord(i));
                    Pieces.RemoveAt(index);
                    if (draggedPiece > index) draggedPiece--;
                }

                // Update and record the data
                move = new Move(Pieces[draggedPiece].Coord, new Coord(i));
                boardUI.SetLastMove(move);

                Pieces[draggedPiece].Coord = new Coord(i);
                Pieces[draggedPiece].ResetPosition();
                
                placedOnValidSquare = true;
            }

            if (!placedOnValidSquare && draggedPiece != -1) {
                Pieces[draggedPiece].ResetPosition(); // If it is illegal to move on that square, reset the piece to its original position
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

                int index = Pieces.FindIndex(p => p.Coord.SquareIndex == rookSource);
                Pieces[index].Coord = new Coord(rookTarget);
                Pieces[index].ResetPosition();
            } 
            // Promotion
            if (board.Square[move.Source].IsPawn && (move.Target < 8 || move.Target > 55)) {
                move = new Move(move.Source, move.Target, Move.QueenPromotion);
                
                int index = Pieces.FindIndex(p => p.Coord.SquareIndex == move.Target);
                Pieces[index] = new PieceUI(new Piece(PieceType.Queen, board.IsWhiteTurn ? PieceType.White : PieceType.Black), new Coord(move.Target));
            }
            // En passant
            if (board.Square[move.Source].IsPawn && Math.Abs(move.Source - move.Target) % 8 != 0 && board.Square[move.Target].IsNone) {
                move = new Move(move.Source, move.Target, Move.EnPassant);

                int target = board.IsWhiteTurn ? move.Target - 8 : move.Target + 8;
                int index = Pieces.FindIndex(p => p.Coord.SquareIndex == target);

                Pieces.RemoveAt(index);
            } 
            
            board.MakeMove(move, record : true);
        }
    }

    public void AnimateMove(Move move, Board board) {
        if (draggedPiece != -1) Pieces[draggedPiece].ResetPosition();
        draggedPiece = -1;

        int index = Pieces.FindIndex(p => p.Coord.SquareIndex == move.Source);

        if (Pieces.FindIndex(p => p.Coord.SquareIndex == move.Target) != -1) {
            int targetIndex = Pieces.FindIndex(p => p.Coord.SquareIndex == move.Target);
            Pieces.RemoveAt(targetIndex);
            if (index > targetIndex) index--;
        }

        int frames = 30;
        double startX = UIHelper.GetScreenX(BoardHelper.ColumnIndex(move.Source));
        double startY = UIHelper.GetScreenY(BoardHelper.RowIndex(move.Source));
        double endX = UIHelper.GetScreenX(BoardHelper.ColumnIndex(move.Target));
        double endY = UIHelper.GetScreenY(BoardHelper.RowIndex(move.Target));
        double dx = (endX - startX) / frames;
        double dy = (endY - startY) / frames;

        animatedPiece = index;
        for (int i = 0; i < frames; i++) {
            Pieces[index].X = (int)(startX + dx * i);
            Pieces[index].Y = (int)(startY + dy * i);
            
            Thread.Sleep(10);
        }
        animatedPiece = -1;

        Pieces[index].Coord = new Coord(move.Target);
        Pieces[index].ResetPosition();

        // Promotion
        // Replace the pawn with a queen
        if (move.Flag == Move.QueenPromotion) {
            Pieces[index] = new PieceUI(new Piece(board.IsWhiteTurn ? PieceType.White : PieceType.Black, PieceType.Queen), new Coord(move.Target));
        } else if (move.Flag == Move.KnightPromotion) {
            Pieces[index] = new PieceUI(new Piece(board.IsWhiteTurn ? PieceType.White : PieceType.Black, PieceType.Knight), new Coord(move.Target));
        } else if (move.Flag == Move.BishopPromotion) {
            Pieces.Add(new PieceUI(new Piece(board.IsWhiteTurn ? PieceType.White : PieceType.Black, PieceType.Bishop), new Coord(move.Target)));
            Pieces.RemoveAt(index);
        } else if (move.Flag == Move.RookPromotion) {
            Pieces.Add(new PieceUI(new Piece(board.IsWhiteTurn ? PieceType.White : PieceType.Black, PieceType.Rook), new Coord(move.Target)));
            Pieces.RemoveAt(index);
        }

        // Castle
        // Move the rook to the other side of the king
        if (move.Flag == Move.Castling) {
            int rookSource = move.Target + (move.Target == 62 || move.Target == 6 ? 1 : -2);
            int rookTarget = move.Target + (move.Target == 62 || move.Target == 6 ? -1 : 1);

            int rookIndex = Pieces.FindIndex(p => p.Coord.SquareIndex == rookSource);

            Pieces[rookIndex].Coord = new Coord(rookTarget);
            Pieces[rookIndex].ResetPosition();
        }

        // En passant
        // Remove the piece that was killed
        if (move.Flag == Move.EnPassant) {
            int target = board.IsWhiteTurn ? move.Target - 8 : move.Target + 8;
            index = Pieces.FindIndex(p => p.Coord.SquareIndex == target);
            Pieces.RemoveAt(index);
        }
    }

    public void Render() {
        foreach (PieceUI piece in Pieces) {
            piece.Render();
        }
        if (draggedPiece != -1) Pieces[draggedPiece].Render();
        if (animatedPiece != -1) Pieces[animatedPiece].Render();
    }
}