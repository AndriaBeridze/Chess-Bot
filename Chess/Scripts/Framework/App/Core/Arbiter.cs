namespace Chess.App;

using System.IO.Compression;
using Chess.API;
using Chess.ChessEngine;

class Arbiter {
    public static bool IsCheckmate(Board board) {
        return BitboardHelper.IsInCheck(board, board.IsWhiteTurn) && MoveGenerator.GenerateMoves(board).Count == 0;
    }

    public static bool IsStalemate(Board board) {
        return !BitboardHelper.IsInCheck(board, board.IsWhiteTurn) && MoveGenerator.GenerateMoves(board).Count == 0;
    }

    public static bool IsDraw(Board board) {
        if (board.HalfMoveClock >= 100) return true;

        if (!board.Type[PieceType.Rook].IsEmpty) return false;
        if (!board.Type[PieceType.Queen].IsEmpty) return false;
        if (!board.Type[PieceType.Pawn].IsEmpty) return false;

        // If there is two bishops on the different color squares, checkmate still can be delivered
        if ((board.Type[PieceType.Bishop] & board.Color[true]).Count() >= 2 || 
            (board.Type[PieceType.Bishop] & board.Color[false]).Count() >= 2) {
            int k = 0;
            Bitboard temp = board.Type[PieceType.Bishop] & board.Color[true];
            while(!temp.IsEmpty) {
                int index = temp.FirstBit;
                k |= 1 << index;

                temp.ClearBit(index);
            }

            if (k == 3) return false;

            k = 0;
            temp = board.Type[PieceType.Bishop] & board.Color[true];
            while(!temp.IsEmpty) {
                int index = temp.FirstBit;
                k |= 1 << index;

                temp.ClearBit(index);
            }

            if (k == 3) return false;
        }

        // If there is a bishop and a knight, checkmate still can be delivered
        if  (((board.Type[PieceType.Bishop] & board.Color[true]).Count() >= 1 && 
                (board.Type[PieceType.Knight] & board.Color[true]).Count() >= 1) ||
            ((board.Type[PieceType.Bishop] & board.Color[false]).Count() >= 1 && 
                (board.Type[PieceType.Knight] & board.Color[false]).Count() >= 1)) return false;

        return true;
    }

    public static string Status(Board board) {
        if (IsCheckmate(board)) return "Checkmate";
        if (IsStalemate(board)) return "Stalemate";
        if (IsDraw(board)) return "Draw";

        return "";
    }
}