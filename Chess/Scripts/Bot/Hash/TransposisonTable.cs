namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

static class TranspositionTable {
    private static readonly int TableSize = 1 << 20; // Size of table: ~1 million entries
    private static readonly HashEntry[] table = new HashEntry[TableSize];

    // Retrieve entry from the table using Zobrist key
    public static HashEntry GetEntry(ulong zobristKey) =>
        table[zobristKey % (ulong)TableSize];

    // Store entry in the table at Zobrist-indexed location
    public static void StoreEntry(ulong zobristKey, HashEntry entry) =>
        table[zobristKey % (ulong)TableSize] = entry;

    // Try to retrieve an entry and check if it matches the given Zobrist key
    public static bool TryProbe(ulong zobristKey, out HashEntry entry) {
        int index = (int)(zobristKey % (ulong)TableSize);
        entry = table[index];
        return entry.key == zobristKey;
    }

    // Record a new hash entry into the table
    public static void RecordHash(Board board, int depth, int value, int flag, Move move) {
        ulong key = board.ZobristKey;

        HashEntry entry = new() {
            key = key,
            depth = depth,
            value = value,
            flag = flag,
            move = move
        };

        StoreEntry(key, entry);
    }

    // Probe for a previously stored evaluation and use it if valid
    public static int? ProbeHash(Board board, int depth, int alpha, int beta) {
        if (TryProbe(board.ZobristKey, out HashEntry entry)) {
            if (entry.depth >= depth) {
                return entry.flag switch {
                    HashFlag.EXACT => entry.value,
                    HashFlag.ALPHA when entry.value <= alpha => alpha,
                    HashFlag.BETA when entry.value >= beta => beta,
                    _ => null
                };
            }
        }
        return null;
    }

    // Clear the transposition table
    public static void Clear() {
        for (int i = 0; i < TableSize; i++) {
            table[i] = new HashEntry {
                key = 0,
                depth = -1,
                flag = -1,
                value = 0,
                move = Move.NullMove
            };
        }
    }
}

// Represents a stored evaluation in the transposition table
struct HashEntry {
    public ulong key;    // Zobrist key for board position
    public int depth;    // Depth at which the position was evaluated
    public int flag;     // Type of bound: EXACT, ALPHA, or BETA
    public int value;    // Evaluation score
    public Move move;    // Best move from this position
}

// Constants representing the type of stored evaluation
static class HashFlag {
    public const int EXACT = 0;
    public const int ALPHA = 1;
    public const int BETA = 2;
}
