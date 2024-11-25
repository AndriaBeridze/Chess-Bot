namespace Chess.App;

using Chess.API;
using Chess.ChessEngine;
using System.Numerics;
using Raylib_cs;

class PositionUI {
    private List<PieceUI> pieces;
    private int draggedPiece = -1; // To keep track of the piece during dragging
    private int animatedPiece = -1; // To keep track of the piece during animation

    private int promotionLastChecked = -1;

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

    public void Update(Board board, BoardUI boardUI, bool highlightMoves) {
        Move move = Move.NullMove;
        
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            int x = Raylib.GetMouseX();
            int y = Raylib.GetMouseY();

            foreach (PieceUI piece in pieces) {
                // If mouse is not hovering on a piece, ignore it
                Rectangle rect = new Rectangle(piece.X, piece.Y, Theme.SquareSideLength, Theme.SquareSideLength);
                if (!Raylib.CheckCollisionPointRec(new Vector2(x, y), rect)) continue;
                    
                draggedPiece = pieces.IndexOf(piece); // Drag the piece

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
                Sounds.Play("illegal");
                pieces[draggedPiece].ResetPosition(); // If it is illegal to move on that square, reset the piece to its original position
            }

            // Reset data
            draggedPiece = -1;
            boardUI.Clear();
        }
        if (!move.IsNull) {
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
                pieces[index] = new PieceUI(new Piece(Piece.Queen, board.IsWhiteTurn ? Piece.White : Piece.Black), new Coord(move.Target));
            }
            // En passant
            if (board.Square[move.Source].IsPawn && Math.Abs(move.Source - move.Target) % 8 != 0 && board.Square[move.Target].IsNone) {
                move = new Move(move.Source, move.Target, Move.EnPassant);

                int target = board.IsWhiteTurn ? move.Target - 8 : move.Target + 8;
                int index = pieces.FindIndex(p => p.Coord.SquareIndex == target);

                pieces.RemoveAt(index);
            } 
            
            PlaySound(move, board);
            board.MakeMove(move, record : true);
        }
    }

    public void AnimateMove(Move move, Board board) {
        // If any piece is dragged, reset its position
        if (draggedPiece != -1) pieces[draggedPiece].ResetPosition();
        draggedPiece = -1;

        int index = pieces.FindIndex(p => p.Coord.SquareIndex == move.Source);

        // Remove killed piece
        if (pieces.FindIndex(p => p.Coord.SquareIndex == move.Target) != -1) {
            int targetIndex = pieces.FindIndex(p => p.Coord.SquareIndex == move.Target);
            pieces.RemoveAt(targetIndex);
            if (index > targetIndex) index--;
        }

        Animate(index, move);

        // Just because floating point numbers are not precise, reset the position of the piece to ensure it is pixel perfect
        pieces[index].Coord = new Coord(move.Target);
        pieces[index].ResetPosition();

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

        PlaySound(move, board);
    }

    public void Animate(int index, Move move) {
        int frames = 25;
        double startX = UIHelper.GetScreenX(BoardHelper.ColumnIndex(move.Source));
        double startY = UIHelper.GetScreenY(BoardHelper.RowIndex(move.Source));
        double endX = UIHelper.GetScreenX(BoardHelper.ColumnIndex(move.Target));
        double endY = UIHelper.GetScreenY(BoardHelper.RowIndex(move.Target));
        double dx = (endX - startX) / frames;
        double dy = (endY - startY) / frames;

        animatedPiece = index;
        // Move piece by a small amount each frame, and wait for a short time
        // Creates an animation effect
        for (int i = 0; i < frames; i++) {
            pieces[index].X = (int)(startX + dx * i);
            pieces[index].Y = (int)(startY + dy * i);
            
            Thread.Sleep(10);
        }
        animatedPiece = -1;
    }

    private void PlaySound(Move move, Board board) {
        bool soundPlayed = false;
        
        board.MakeMove(move);
        if (Arbiter.Status(board) != "") { soundPlayed = true; Sounds.Play("game-over"); }
        else if (MoveHelper.IsInCheck(board, true) || MoveHelper.IsInCheck(board, false)) { soundPlayed = true; Sounds.Play("check"); }
        board.UnmakeMove(move);
        
        if (soundPlayed) return;

        if (board.Square[move.Target].Type != Piece.None) Sounds.Play("capture");
        else if (move.IsPromotion) Sounds.Play("promotion");
        else if (move.IsCastling) Sounds.Play("castle");
        else Sounds.Play("move");
    }

    // Special case that needs to be handled separately due to the bug before
    public void AnimatePromotion(Board board) {
        if (board.MovesMade.Count == 0) return;
        if (!board.MovesMade[board.MovesMade.Count - 1].IsPromotion) return;
        if (promotionLastChecked == board.MovesMade.Count) return;

        Move move = board.MovesMade[board.MovesMade.Count - 1];
        int color = board.IsWhiteTurn ? Piece.Black : Piece.White;
        int index = pieces.FindIndex(piece => piece.Coord == new Coord(move.Target));
        
        if (index == -1) return;
        promotionLastChecked = board.MovesMade.Count;

        pieces[index] = new PieceUI(new Piece(color, move.PromotingTo), new Coord(move.Target));
    }

    public void Render() {
        foreach (PieceUI piece in pieces) {
            piece.Render();
        }
        if (draggedPiece != -1) pieces[draggedPiece].Render();
        if (animatedPiece != -1) pieces[animatedPiece].Render();
    }
}