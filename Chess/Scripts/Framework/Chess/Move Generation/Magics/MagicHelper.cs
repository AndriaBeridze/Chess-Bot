namespace Chess.ChessEngine;

using Chess.API;

static class MagicHelper {
    public static Bitboard[] CreateAllBlockerBitboards(ulong movementMask) {
        // Create a list of the indices of the bits that are set in the movement mask
        List<int> moveSquareIndices = new();
        for (int i = 0; i < 64; i++) {
            if (((movementMask >> i) & 1) == 1) {
                moveSquareIndices.Add(i);
            }
        }

        // Calculate total number of different bitboards (one for each possible arrangement of pieces)
        int numPatterns = 1 << moveSquareIndices.Count; // 2^n
        Bitboard[] blockerBitboards = new Bitboard[numPatterns];

        // Create all bitboards
        for (int patternIndex = 0; patternIndex < numPatterns; patternIndex++) {
            blockerBitboards[patternIndex] = Bitboard.Null;
            for (int bitIndex = 0; bitIndex < moveSquareIndices.Count; bitIndex++) {
                int bit = (patternIndex >> bitIndex) & 1;
                blockerBitboards[patternIndex] |= new Bitboard((ulong) bit) << moveSquareIndices[bitIndex];
            }
        }

        return blockerBitboards;
    }


    public static Bitboard CreateMovementMask(int squareIndex, bool ortho) {
        Bitboard mask = Bitboard.Null;
        Coord[] directions = ortho ? [Coord.North, Coord.South, Coord.East, Coord.West] : 
                                     [Coord.NorthEast, Coord.NorthWest, Coord.SouthEast, Coord.SouthWest];
        Coord startCoord = new Coord(squareIndex);

        foreach (Coord dir in directions) {
            for (int dst = 1; dst < 8; dst++) {
                Coord coord = startCoord + dir * dst;
                Coord nextCoord = startCoord + dir * (dst + 1);

                if (nextCoord.IsValidSquare) {
                    mask.SetBit(coord.SquareIndex);
                }
                else { break; }
            }
        }

        return mask;
    }

    public static Bitboard LegalMoveBitboardFromBlockers(int startSquare, Bitboard blockerBitboard, bool ortho) {
        Bitboard bitboard = Bitboard.Null;

        Coord[] directions = ortho ? [Coord.North, Coord.South, Coord.East, Coord.West] : 
                                     [Coord.NorthEast, Coord.NorthWest, Coord.SouthEast, Coord.SouthWest];
        Coord startCoord = new Coord(startSquare);

        foreach (Coord dir in directions) {
            for (int dst = 1; dst < 8; dst++) {
                Coord coord = startCoord + dir * dst;

                if (coord.IsValidSquare) {
                    bitboard.SetBit(coord.SquareIndex);
                    if (blockerBitboard.Contains(coord.SquareIndex)) {
                        // If there is a blocker, we stop adding squares in this direction
                        // and we do not add the blocker square itself to the bitboard
                        break;
                    }
                }
                else { break; }
            }
        }

        return bitboard;
    }
}