namespace Chess.Bot;

using Chess.ChessEngine;
using Chess.API;

class Evaluation {
    public const int Pawn = 100;
    public const int Knight = 320;
    public const int Bishop = 330;
    public const int Rook = 500;
    public const int Queen = 900;

    public static int Evaluate(Board board) {
        int eval = 0;

        eval += CalculateMaterialWorth(board, true) - CalculateMaterialWorth(board, false);

        return eval * (board.IsWhiteTurn ? 1 : -1);
    }   

    private static int CalculateMaterialWorth(Board board, bool isWhite) {
        int eval = 0;

        eval += (board.Type[PieceType.Pawn] & board.Color[isWhite]).Count() * Pawn;
        eval += (board.Type[PieceType.Knight] & board.Color[isWhite]).Count() * Knight;
        eval += (board.Type[PieceType.Bishop] & board.Color[isWhite]).Count() * Bishop;
        eval += (board.Type[PieceType.Rook] & board.Color[isWhite]).Count() * Rook;
        eval += (board.Type[PieceType.Queen] & board.Color[isWhite]).Count() * Queen;

        return eval;
    }
}