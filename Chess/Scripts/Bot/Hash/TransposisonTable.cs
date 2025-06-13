namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

static class TranspositionTable {
    private static readonly int TableSize = 1 << 20; // ~ 1 million entries
    private static HashEntry[] table = new HashEntry[TableSize];

    public static HashEntry GetEntry(ulong zobristKey) =>
        table[zobristKey % (ulong)TableSize];

    public static void StoreEntry(ulong zobristKey, HashEntry entry) =>
        table[zobristKey % (ulong)TableSize] = entry;

    public static bool TryProbe(ulong zobristKey, out HashEntry entry) {
        int index = (int)(zobristKey % (ulong)TableSize);
        entry = table[index];
        return entry.key == zobristKey;
    }

    public static void RecordHash(Board board, int depth, int val, int flag, Move move) {
        ulong key = board.ZobristKey;

        HashEntry entry = new() {
            key = key,
            value = val,
            flag = flag,
            depth = depth,
            move = move
        };

        StoreEntry(key, entry);
    }

    public static int? ProbeHash(Board board, int depth, int alpha, int beta) {
        if (TryProbe(board.ZobristKey, out HashEntry entry)) {
            if (entry.depth >= depth) {
                if (entry.flag == HashFlag.EXACT) {
                    return entry.value;
                } else if (entry.flag == HashFlag.ALPHA && entry.value <= alpha) {
                    return alpha;
                } else if (entry.flag == HashFlag.BETA && entry.value >= beta) {
                    return beta;
                }
            }
        }
        return null;
    }

    public static void Clear() {
        for (int i = 0; i < TableSize; i++) {
            table[i] = new HashEntry { key = 0, depth = -1, flag = -1, value = 0, move = Move.NullMove };
        }
    }
}

struct HashEntry {
    public ulong key;        // Zobrist key
    public int depth;        // Search depth at which it was stored
    public int flag;         // HashFlag.EXACT / ALPHA / BETA
    public int value;        // Evaluation score
    public Move move;
}

class HashFlag {
    public const int EXACT = 0;
    public const int ALPHA = 1;
    public const int BETA = 2;
}
