namespace Chess.App;

using Chess.API;
using Chess.ChessEngine;

class Arbiter {
    public static bool IsCheckmate(Board board) {
        // Two requirements for checkmate:
        // 1. The king is in check
        // 2. There are no legal moves to get out of check
        if (!MoveHelper.IsInCheck(board, board.IsWhiteTurn)) return false;
        if (MoveGenerator.GenerateMoves(board).Count > 0) return false;
        return true;
    }

    public static bool IsStalemate(Board board) {
        // Two requirements for stalemate:
        // 1. The king is not in check
        // 2. There are no legal moves
        if (MoveHelper.IsInCheck(board, board.IsWhiteTurn)) return false;
        if (MoveGenerator.GenerateMoves(board).Count > 0) return false;
        return true;
    }

    public static bool IsDraw(Board board) {
        if (board.HalfMoveClock >= 100) return true;

        if (!board.Type[Piece.Rook].IsEmpty) return false;
        if (!board.Type[Piece.Queen].IsEmpty) return false;
        if (!board.Type[Piece.Pawn].IsEmpty) return false;

        // If there is two bishops on the different color squares, checkmate still can be delivered
        if ((board.Type[Piece.Bishop] & board.Color[true]).Count() >= 2) {
            Bitboard bishops = board.Type[Piece.Bishop] & board.Color[true];
            int k = 0;
            while (!bishops.IsEmpty) {
                int index = bishops.FirstBit;
                k |= (index / 8 + index % 8) % 2;
            }

            if ((k ^ 0b11) == 0) return false;
        }

        if ((board.Type[Piece.Bishop] & board.Color[false]).Count() >= 2) {
            Bitboard bishops = board.Type[Piece.Bishop] & board.Color[false];
            int k = 0;
            while (!bishops.IsEmpty) {
                int index = bishops.FirstBit;
                k |= (index / 8 + index % 8) % 2;
            }

            if ((k ^ 0b11) == 0) return false;
        }

        // If there is a bishop and a knight, checkmate still can be delivered
        if  (!(board.Type[Piece.Bishop] & board.Color[true]).IsEmpty && 
             !(board.Type[Piece.Knight] & board.Color[true]).IsEmpty) return false;
        
        if  (!(board.Type[Piece.Bishop] & board.Color[false]).IsEmpty &&
             !(board.Type[Piece.Knight] & board.Color[false]).IsEmpty) return false;

        return true;
    }

    public static string Status(Board board) {
        if (IsCheckmate(board)) return "Checkmate";
        if (IsStalemate(board)) return "Stalemate";
        if (IsDraw(board)) return "Draw";

        return "";
    }
}