namespace Chess.ChessEngine;

class BitboardHelper {
    public static Bitboard GetBitAt(int index) => new Bitboard(1UL << index);

    public static int GetIndex(Bitboard bitboard) {
        ulong value = bitboard.Value;
        int index = 0;
        while (value > 0) {
            value >>= 1;
            index++;
        }
        return index - 1;
    }

    public static Bitboard GetFile(int index) {
        ulong file = 0x0101010101010101;
        return new Bitboard(file << index);
    }

    public static Bitboard GetRank(int index) {
        ulong rank = 0xFF;
        return new Bitboard(rank << (index * 8));
    }

    public static Bitboard GetDiagonal(int index) {
        ulong diagonal = 0x8040201008040201;
        return new Bitboard(diagonal << index);
    }

    public static Bitboard GetAntiDiagonal(int index) {
        ulong antiDiagonal = 0x0102040810204080;
        return new Bitboard(antiDiagonal << index);
    }
}