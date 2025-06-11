namespace Chess.ChessEngine;

static class Magic {
    public static Bitboard[] RookMask;
    public static Bitboard[] BishopMask;

    public static readonly Bitboard[][] RookAttacks;
	public static readonly Bitboard[][] BishopAttacks;

    public static Bitboard GetSliderAttacks(int square, Bitboard blockers, bool ortho) {
        return ortho ? GetRookAttacks(square, blockers) : GetBishopAttacks(square, blockers);
    }

    public static Bitboard GetRookAttacks(int square, Bitboard blockers) {
        ulong key = ((blockers & RookMask[square]).Value * MagicNumbers.RookMagics[square]) >> MagicNumbers.RookShifts[square];
        return RookAttacks[square][key];
    }

    public static Bitboard GetBishopAttacks(int square, Bitboard blockers) {
        ulong key = ((blockers & BishopMask[square]).Value * MagicNumbers.BishopMagics[square]) >> MagicNumbers.BishopShifts[square];
        return BishopAttacks[square][key];
    }

    static Magic() {
        RookMask = new Bitboard[64];
        BishopMask = new Bitboard[64];

        for (int squareIndex = 0; squareIndex < 64; squareIndex++) {
            RookMask[squareIndex] = MagicHelper.CreateMovementMask(squareIndex, true);
            BishopMask[squareIndex] = MagicHelper.CreateMovementMask(squareIndex, false);
        }

        RookAttacks = new Bitboard[64][];
        BishopAttacks = new Bitboard[64][];

        for (int i = 0; i < 64; i++){
            RookAttacks[i] = CreateTable(i, true, new Bitboard(MagicNumbers.RookMagics[i]), MagicNumbers.RookShifts[i]);
            BishopAttacks[i] = CreateTable(i, false, new Bitboard(MagicNumbers.BishopMagics[i]), MagicNumbers.BishopShifts[i]);
        }

        Bitboard[] CreateTable(int square, bool rook, Bitboard magic, int leftShift) {
            int numBits = 64 - leftShift;
            int lookupSize = 1 << numBits;
            Bitboard[] table = new Bitboard[lookupSize];

            Bitboard movementMask = MagicHelper.CreateMovementMask(square, rook);
            Bitboard[] blockerPatterns = MagicHelper.CreateAllBlockerBitboards(movementMask.Value);

            foreach (Bitboard pattern in blockerPatterns) {
                ulong index = (pattern.Value * magic.Value) >> leftShift;
                Bitboard moves = MagicHelper.LegalMoveBitboardFromBlockers(square, pattern, rook);
                table[index] = moves;
            }

            return table;
        }
    }
}