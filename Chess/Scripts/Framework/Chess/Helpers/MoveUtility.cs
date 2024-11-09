namespace Chess.ChessEngine;

using Chess.API;

class MoveUtility {
    static List<Board> history = new List<Board>();

    public static void MakeMove(Board board, Move move) {
        history.Add(board);

        board.EnPassantSquare = -1;

        // Update bitboards
        Piece sourcePiece = board.Square[move.Source];
        Piece targetPiece = board.Square[move.Target];

        // IMPORTANT: Update enemy bitboard before updating friendly bitboard
        // This will cause the problem if the moved piece and the captured piece are the same
        if (targetPiece.Type != PieceType.None) {
            // Update target piece bitboard
            board.Bitboard[targetPiece.Type].ClearBit(move.Target);

            // Update enemy color bitboard
            board.Bitboard[board.IsWhiteTurn ? PieceType.Black : PieceType.White].ClearBit(move.Target);
        }

        // Update source piece bitboard
        board.Bitboard[sourcePiece.Type].ClearBit(move.Source);
        board.Bitboard[sourcePiece.Type].SetBit(move.Target);

        // Update friendly color bitboard
        board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].ClearBit(move.Source);
        board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].SetBit(move.Target);

        // Update square array
        board.Square[move.Target] = board.Square[move.Source];
        board.Square[move.Source] = new Piece(PieceType.None);

        // Promotion
        bool isPromoting = false;
        Piece promotingTo = new Piece(PieceType.None); 

        if (move.Flag == Move.QueenPromotion) {
            isPromoting = true;
            promotingTo = new Piece(PieceType.Queen, board.IsWhiteTurn ? PieceType.White : PieceType.Black);
        } else if (move.Flag == Move.BishopPromotion) {
            isPromoting = true;
            promotingTo = new Piece(PieceType.Bishop, board.IsWhiteTurn ? PieceType.White : PieceType.Black);
        } else if (move.Flag == Move.KnightPromotion) {
            isPromoting = true;
            promotingTo = new Piece(PieceType.Knight, board.IsWhiteTurn ? PieceType.White : PieceType.Black);
        } else if (move.Flag == Move.RookPromotion) {
            isPromoting = true;
            promotingTo = new Piece(PieceType.Rook, board.IsWhiteTurn ? PieceType.White : PieceType.Black);
        }

        // If promoting, update bitboards and square array
        if (isPromoting) {
            board.Bitboard[promotingTo.Type].SetBit(move.Target);
            board.Bitboard[PieceType.Pawn].ClearBit(move.Target);

            board.Square[move.Target] = promotingTo;
        }

        // Castling
        if (move.Flag == Move.Castling) {
            int rookSource = move.Target + (move.Target == 62 || move.Target == 6 ? 1 : -2);
            int rookTarget = move.Target + (move.Target == 62 || move.Target == 6 ? -1 : 1);

            // Update rook bitboard
            board.Bitboard[PieceType.Rook].ClearBit(rookSource);
            board.Bitboard[PieceType.Rook].SetBit(rookTarget);

            // Update friendly color bitboard
            board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].ClearBit(rookSource);
            board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].SetBit(rookTarget);

            // Update square array
            board.Square[rookTarget] = board.Square[rookSource]; 
            board.Square[rookSource] = new Piece(PieceType.None);
        }

        // En passant
        if (move.Flag == Move.EnPassant) {
            int target = board.IsWhiteTurn ? move.Target - 8 : move.Target + 8;

            // Update target piece bitboard
            board.Bitboard[PieceType.Pawn].ClearBit(target);

            // Update enemy color bitboard
            board.Bitboard[board.IsWhiteTurn ? PieceType.Black : PieceType.White].ClearBit(target);

            // Update square array
            board.Square[target] = new Piece(PieceType.None);
        }

        if (sourcePiece.Type == PieceType.Pawn && Math.Abs(move.Source - move.Target) == 16) {
            board.EnPassantSquare = board.IsWhiteTurn ? move.Source + 8 : move.Source - 8;
        }
    }

    public static void UnmakeMove(ref Board board, Move move) {
        board = history.Last();
        history.RemoveAt(history.Count - 1);
    }
}