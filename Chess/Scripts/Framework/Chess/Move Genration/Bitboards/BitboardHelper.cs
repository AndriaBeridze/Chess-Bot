using Chess.API;

namespace Chess.ChessEngine;

class BitboardHelper {
    public static Bitboard GetBitAt(int index) => new Bitboard(1UL << index);

    public static List<Move> ExtractPawnMoves(Bitboard bitboard, int dir, bool promotion = false) {
        List<Move> moves = new List<Move>();
        while (!bitboard.IsEmpty) {
            int index = bitboard.FirstBit;
            if (!promotion) moves.Add(new Move(index - dir, index));
            else {
                // Flags are being added to the move to indicate promotion
                moves.Add(new Move(index - dir, index, Move.QueenPromotion));
                moves.Add(new Move(index - dir, index, Move.BishopPromotion));
                moves.Add(new Move(index - dir, index, Move.KnightPromotion));
                moves.Add(new Move(index - dir, index, Move.RookPromotion));
            }

            bitboard.ClearBit(index); 
        }

        return moves;
    }

    public static List<Move> ExtractMoves(Bitboard bitboard, int index) {
        List<Move> moves = new List<Move>();
        while (!bitboard.IsEmpty) {
            int target = bitboard.FirstBit;
            moves.Add(new Move(index, target));
            
            bitboard.ClearBit(target);
        }

        return moves;
    }

    public static Bitboard GetEnPassant(Board board) {
        if (board.EnPassantSquare == -1) return new Bitboard(0);
        return GetBitAt(board.EnPassantSquare);
    }
}