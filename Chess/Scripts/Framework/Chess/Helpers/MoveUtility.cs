namespace Chess.ChessEngine;

using System.Drawing;
using Chess.API;

class MoveUtility {
    static List<Piece> killedPiece = new List<Piece>();
    static List<int> castlingRights = new List<int>();


    public static void MakeMove(Board board, Move move) {
        killedPiece.Add(board.Square[move.Target]);
        castlingRights.Add(board.CastlingRights);

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

        if (sourcePiece.Type == PieceType.King) {
            board.CastlingRights &= board.IsWhiteTurn ? 0b0011 : 0b1100;
        }

        if (sourcePiece.Type == PieceType.Rook) {
            if (move.Source == 0) {
                board.CastlingRights &= 0b1011;
            } else if (move.Source == 7) {
                board.CastlingRights &= 0b0111;
            } else if (move.Source == 56) {
                board.CastlingRights &= 0b1110;
            } else if (move.Source == 63) {
                board.CastlingRights &= 0b1101;
            } 
        }

        if (targetPiece.Type == PieceType.Rook) {
            if (move.Target == 0) {
                board.CastlingRights &= 0b1011;
            } else if (move.Target == 7) {
                board.CastlingRights &= 0b0111;
            } else if (move.Target == 56) {
                board.CastlingRights &= 0b1110;
            } else if (move.Target == 63) {
                board.CastlingRights &= 0b1101;
            } 
        }
    }

    public static void UnmakeMove(Board board, Move move) {
        board.EnPassantSquare = -1;

        // Restore source piece bitboard
        board.Bitboard[board.Square[move.Target].Type].ClearBit(move.Target);
        board.Bitboard[board.Square[move.Target].Type].SetBit(move.Source);

        // Restore friendly color bitboard
        board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].ClearBit(move.Target);
        board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].SetBit(move.Source);

        // Undo en passant
        if (move.Flag == Move.EnPassant) {
            int target = board.IsWhiteTurn ? move.Target - 8 : move.Target + 8;

            // Restore target piece bitboard
            board.Bitboard[PieceType.Pawn].SetBit(target);

            // Restore enemy color bitboard
            board.Bitboard[board.IsWhiteTurn ? PieceType.Black : PieceType.White].SetBit(target);

            // Restore square array
            board.Square[target] = new Piece(board.IsWhiteTurn ? PieceType.Black : PieceType.White, PieceType.Pawn);

            // Update en passant square
            board.EnPassantSquare = move.Target;
        }

        // Undo castling
        if (move.Flag == Move.Castling) {
            int rookSource = move.Target + (move.Target == 62 || move.Target == 6 ? 1 : -2);
            int rookTarget = move.Target + (move.Target == 62 || move.Target == 6 ? -1 : 1);

            // Restore rook bitboard
            board.Bitboard[PieceType.Rook].SetBit(rookSource);
            board.Bitboard[PieceType.Rook].ClearBit(rookTarget);

            // Restore friendly color bitboard
            board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].SetBit(rookSource);
            board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].ClearBit(rookTarget);

            // Restore square array
            board.Square[rookSource] = board.Square[rookTarget];
            board.Square[rookTarget] = new Piece(PieceType.None);
        }

        // Undo promotion
        if (move.Flag == Move.QueenPromotion || move.Flag == Move.BishopPromotion || move.Flag == Move.KnightPromotion || move.Flag == Move.RookPromotion) {
            int promotingToType;

            if (move.Flag == Move.QueenPromotion) promotingToType = PieceType.Queen;
            else if (move.Flag == Move.RookPromotion) promotingToType = PieceType.Rook;
            else if (move.Flag == Move.BishopPromotion) promotingToType = PieceType.Bishop;
            else promotingToType = PieceType.Knight;

            // Restore bitboards
            board.Bitboard[promotingToType].ClearBit(move.Source);
            board.Bitboard[PieceType.Pawn].SetBit(move.Source);

            // Restore square array
            board.Square[move.Target] = new Piece(PieceType.Pawn, board.IsWhiteTurn ? PieceType.White : PieceType.Black);
        }

        // Restore target piece bitboard if it was captured
        if (killedPiece[killedPiece.Count - 1].Type != PieceType.None) {
            board.Bitboard[killedPiece[killedPiece.Count - 1].Type].SetBit(move.Target);
            board.Bitboard[board.IsWhiteTurn ? PieceType.Black : PieceType.White].SetBit(move.Target);
        }

        // Restore square array
        board.Square[move.Source] = board.Square[move.Target];
        board.Square[move.Target] = killedPiece[killedPiece.Count - 1];

        // Restore castling rights
        board.CastlingRights = castlingRights[castlingRights.Count - 1];

        // Remove last elements from history
        killedPiece.RemoveAt(killedPiece.Count - 1);
        castlingRights.RemoveAt(castlingRights.Count - 1);
    }
}