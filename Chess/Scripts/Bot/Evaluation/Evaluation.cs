namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

class Evaluation {
    // The values of the pieces
    // Pawn = 1 Pawn
    // Knight = 3 Pawns
    // Bishop = 3 Pawns
    // Rook = 5 Pawns
    // Queen = 9 Pawns
    // Some grandmasters suggest that bishop is slightly better than a knight, so we can adjust the values
    public const int Pawn = 100;
    public const int Knight = 300;
    public const int Bishop = 330;
    public const int Rook = 500;
    public const int Queen = 900;

    // The evaluation consists of two parts:
    // 1. Material worth: The value of the pieces on the board - controls the pieces that are under attack
    // 2. Bonus: The value of the position of the pieces on the board - helps with the strategic placement of the pieces
    public static int Evaluate(Board board) {
        int eval = 0;

        eval += CalculateMaterialWorth(board, true) - CalculateMaterialWorth(board, false);
        eval += CalculateBonus(board, true) - CalculateBonus(board, false);

        // Since we are evaluating the board from the white's perspective, we need to negate the value if it's black
        return eval * (board.IsWhiteTurn ? 1 : -1); 
    }   

    private static int CalculateMaterialWorth(Board board, bool isWhite) {
        int eval = 0;

        eval += (board.Type[Piece.Pawn] & board.Color[isWhite]).Count() * Pawn;
        eval += (board.Type[Piece.Knight] & board.Color[isWhite]).Count() * Knight;
        eval += (board.Type[Piece.Bishop] & board.Color[isWhite]).Count() * Bishop;
        eval += (board.Type[Piece.Rook] & board.Color[isWhite]).Count() * Rook;
        eval += (board.Type[Piece.Queen] & board.Color[isWhite]).Count() * Queen;

        return eval;
    }

    private static int CalculateBonus(Board board, bool isWhite) {
        int bonus = 0;

        for (int i = 0; i < 64; i++) {
            if (board.Square[i].IsWhite == isWhite && board.Square[i].Type != Piece.None) {
                int index = isWhite ? i : 63 - i;
                bonus += Bonus.BonusMap[board.Square[i].Type][index];
            }
        }

        return bonus;
    }
}