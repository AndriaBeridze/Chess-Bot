namespace Chess.Bot;

using Chess.API;

class Bonus {
    // Bonus maps are used to "encourage" the bot to make certain moves
    // Good practice for the opening and endgame
    // The values are very general and can be adjusted (especially during the phase changes)
    private static int[] pawnBonusMap = {
         0,  0,   0,   0,   0,   0,  0, 0,
         5, 10,  10, -20, -20,  10, 10,  5,
         5, -5, -10,   0,   0, -10, -5,  5,
         0,  0,   0,  20,  20,   0,  0,  0,
         5,  5,  10,  25,  25,  10,  5,  5,
        10, 10,  20,  30,  30,  20, 10, 10,
        50, 50,  50,  50,  50,  50, 50, 50,
         0,  0,   0,   0,   0,   0,  0,  0
    };

    private static int[] knightBonusMap = {
        -50, -40, -30, -30, -30, -30, -40, -50,
        -40, -20,   0,   5,   5,   0, -20, -40,
        -30,   5,  10,  15,  15,  10,   5, -30,
        -30,   0,  15,  20,  20,  15,   0, -30,
        -30,   5,  15,  20,  20,  15,   5, -30,
        -30,   0,  10,  15,  15,  10,   0, -30,
        -40, -20,   0,   0,   0,   0, -20, -40,
        -50, -40, -30, -30, -30, -30, -40, -50
    };

    private static int[] bishopBonusMap = {
        -20, -10, -10, -10, -10, -10, -10, -20,
        -10,   5,   0,   0,   0,   0,   5, -10,
        -10,  10,  10,  10,  10,  10,  10, -10,
        -10,   0,  10,  10,  10,  10,   0, -10,
        -10,   5,   5,  10,  10,   5,   5, -10,
        -10,   0,   5,  10,  10,   5,   0, -10,
        -10,   0,   0,   0,   0,   0,   0, -10,
        -20, -10, -10, -10, -10, -10, -10, -20
    };

    private static int[] rookBonusMap = {
         0,  0,  0,  5,  5,  0,  0,  0,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
         5, 10, 10, 10, 10, 10, 10,  5,
         0,  0,  0,  0,  0,  0,  0,  0
    };

    private static int[] queenBonusMap = {
        -20, -10, -10, -5, -5, -10, -10, -20,
        -10,   0,   0,  0,  0,   0,   0, -10,
        -10,   5,   5,  5,  5,   5,   0, -10,
          0,   0,   5,  5,  5,   5,   0,  -5,
         -5,   0,   5,  5,  5,   5,   0,  -5,
        -10,   0,   5,  5,  5,   5,   0, -10,
        -10,   0,   0,  0,  0,   0,   0, -10,
        -20, -10, -10, -5, -5, -10, -10, -20
    };

    private static int[] kingBonusMap = {
         20,  30,  10,   0,   0,  10,  30,  20,
         20,  20,   0,   0,   0,   0,  20,  20,
        -10, -20, -20, -20, -20, -20, -20, -10,
        -20, -30, -30, -40, -40, -30, -30, -20,
        -30, -40, -40, -50, -50, -40, -40, -30,
        -30, -40, -40, -50, -50, -40, -40, -30,
        -30, -40, -40, -50, -50, -40, -40, -30,
        -30, -40, -40, -50, -50, -40, -40, -30
    };

    public static Dictionary<int, int[]> BonusMap = new Dictionary<int, int[]> {
        { Piece.Pawn, pawnBonusMap },
        { Piece.Knight, knightBonusMap },
        { Piece.Bishop, bishopBonusMap },
        { Piece.Rook, rookBonusMap },
        { Piece.Queen, queenBonusMap },
        { Piece.King, kingBonusMap }
    };
}