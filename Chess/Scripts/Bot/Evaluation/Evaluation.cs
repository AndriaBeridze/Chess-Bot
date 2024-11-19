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
        eval += CalculateBonus(board, true) - CalculateBonus(board, false);

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

    private static int CalculateBonus(Board board, bool isWhite) {
        int bonus = 0;

        for (int i = 0; i < 64; i++) {
            if (board.Square[i].IsWhite == isWhite) {
                switch (board.Square[i].Type) {
                    case PieceType.Pawn:
                        bonus += Bonus.PawnBonusMap[isWhite ? i : 63 - i];
                        break;
                    case PieceType.Knight:
                        bonus += Bonus.KnightBonusMap[i];
                        break;
                    case PieceType.Bishop:
                        bonus += Bonus.BishopBonusMap[i];
                        break;
                    case PieceType.Rook:
                        bonus += Bonus.RookBonusMap[i];
                        break;
                    case PieceType.Queen:
                        bonus += Bonus.QueenBonusMap[i];
                        break;
                    case PieceType.King:
                        bonus += Bonus.KingBonusMap[isWhite ? i : 63 - i];
                        break;
                }
            }
        }

        return bonus;
    }
}