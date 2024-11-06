using Chess.API;

namespace Chess.ChessEngine;

class BitboardHelper {
    public static void SetBit(ref ulong bitboard, int index) {
        bitboard |= 1UL << index;
    }

    public static void ClearBit(ref ulong bitboard, int index) {
        bitboard &= ~(1UL << index);
    }

    // For example 1236 is 10011010100 in binary 
    // The result of the function will be 2, because rightmost bit that is 1 is at index 0
    // Function is effective because computers keep negative numbers in two's complement form
    public static int GetFirstBit(ulong bitboard) {
        return (int) Math.Log2(bitboard & (ulong) - (long) bitboard);
    }

    public static List<Move> ExtractPawnMoves(ulong bitboard, int dir, bool promotion = false) {
        List<Move> moves = new List<Move>();
        while (bitboard != 0) {
            int index = GetFirstBit(bitboard);
            if (!promotion) moves.Add(new Move(index - dir, index));
            else {
                // Flags are being added to the move to indicate promotion
                moves.Add(new Move(index - dir, index, Move.QueenPromotion));
                moves.Add(new Move(index - dir, index, Move.BishopPromotion));
                moves.Add(new Move(index - dir, index, Move.KnightPromotion));
                moves.Add(new Move(index - dir, index, Move.RookPromotion));
            }

            ClearBit(ref bitboard, index);
        }

        return moves;
    }

    public static List<Move> ExtractKnightMoves(ulong bitboard, int index) {
        List<Move> moves = new List<Move>();
        while (bitboard != 0) {
            int target = GetFirstBit(bitboard);
            moves.Add(new Move(index, target));
            
            ClearBit(ref bitboard, target);
        }

        return moves;
    }
}