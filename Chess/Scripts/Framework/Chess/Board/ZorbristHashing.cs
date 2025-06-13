namespace Chess.ChessEngine;

using Chess.API;

class ZobristHashing {
    // Zobrist hashing is a technique used to generate a unique hash for a chess position
    // It uses random numbers to represent each piece on each square and combines them using XOR
    // This allows for fast comparison of positions and is used in transposition tables
    public static ulong CalculateZobristKey(Board board) {
        ulong key = 0;
        if (!board.IsWhiteTurn) key ^= blackToMoveKey; // XOR with the key for black to move
        for (int i = 0; i < 64; i++) {
            Piece piece = board.Square[i];
            if (!piece.IsNone) {
                int color = piece.Color;
                int pieceIndex = (color / 8 - 1) * 6 + (piece.Type - 1); // 0-11 for 12 pieces
                key ^= ZobristTable[i, pieceIndex];
            }
        }
        return key;
    }

    private static readonly ulong[,] ZobristTable = new ulong[64, 12]; // 64 squares, 12 piece types (6 colors * 2 types)
    private static readonly ulong blackToMoveKey = 0x1UL; // Unique key for black to move

    static ZobristHashing() {
        Random random = new Random();
        blackToMoveKey = (ulong) random.NextInt64(long.MaxValue);
        for (int i = 0; i < 64; i++) {
            for (int j = 0; j < 12; j++) {
                ZobristTable[i, j] = (ulong) random.NextInt64(long.MaxValue);
            }
        }
    }
}