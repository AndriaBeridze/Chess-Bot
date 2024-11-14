namespace Chess.Bot;

using Chess.ChessEngine;
using Chess.API;

class Evaluation {
    static int pawn = 100;
    static int knight = 320;
    static int bishop = 330;
    static int rook = 500;
    static int queen = 900;

    public static int Evaluate(Board board) {
        int eval = 0;

        eval += CalculateMaterialWorth(board, true);
        eval -= CalculateMaterialWorth(board, false);

        return eval * (board.IsWhiteTurn ? 1 : -1);
    }   

    private static int CalculateMaterialWorth(Board board, bool isWhite) {
        int eval = 0;

        eval += (board.Type[PieceType.Pawn] & board.Color[isWhite]).Count() * pawn;
        eval += (board.Type[PieceType.Knight] & board.Color[isWhite]).Count() * knight;
        eval += (board.Type[PieceType.Bishop] & board.Color[isWhite]).Count() * bishop;
        eval += (board.Type[PieceType.Rook] & board.Color[isWhite]).Count() * rook;
        eval += (board.Type[PieceType.Queen] & board.Color[isWhite]).Count() * queen;

        return eval;
    }
}