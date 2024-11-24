namespace Chess.ChessEngine;

using Chess.API;

class MoveHelper {
    // Step 1: Isolate the current row where the rook is located.
    // We'll use a mask to capture only the occupied squares in this row, excluding the rook itself.

    // Step 2: Subtract the rook's bitboard twice from the masked occupied squares.
    // This subtraction creates a "shifted" bit pattern, setting all bits between the rook
    // and the nearest occupied square to its left (or the edge of the board) to 1.

    // Formula Explanation: ((occupied & mask) - 2 * (rook)) & mask
    // - 'occupied & mask' gives the occupied squares in the rook's row.
    // - Subtracting '2 * rook' from this masked row shifts bits, creating a solid block of 1s from the rook to the left.
    // - Finally, '& mask' ensures that only bits within the current row are preserved.
    
    // Step 3: Repeat the process for the rook's column.
    public static Bitboard StraightMoves(Board board, int index) {
        Bitboard moveSet = new Bitboard(0);

        int row = BoardHelper.RowIndex(index);
        int col = BoardHelper.ColumnIndex(index);

        Bitboard occupied = board.Occupied;

        Bitboard left = ((occupied & Masks.Row[row]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.Row[row];
        Bitboard right = (((occupied & Masks.Row[row]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.Row[row];

        Bitboard up = ((occupied & Masks.Column[col]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.Column[col];
        Bitboard down = (((occupied & Masks.Column[col]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.Column[col];

        moveSet |= (left ^ right) & Masks.Row[row];
        moveSet |= (up ^ down) & Masks.Column[col];

        return moveSet;
    }

    // Same as the StraightMoves method, but we're now calculating the diagonal moves.
    public static Bitboard DiagonalMoves(Board board, int index) {
        Bitboard moveSet = new Bitboard(0);

        int row = BoardHelper.RowIndex(index);
        int col = BoardHelper.ColumnIndex(index);

        Bitboard occupied = board.Occupied;

        int diagonalIndex = row - col + 7;
        int antiDiagonalIndex = row + col;

        Bitboard topRight = ((occupied & Masks.Diagonal[diagonalIndex]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.Diagonal[diagonalIndex];
        Bitboard bottomLeft = (((occupied & Masks.Diagonal[diagonalIndex]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.Diagonal[diagonalIndex];

        Bitboard topLeft = ((occupied & Masks.AntiDiagonal[antiDiagonalIndex]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.AntiDiagonal[antiDiagonalIndex];
        Bitboard bottomRight = (((occupied & Masks.AntiDiagonal[antiDiagonalIndex]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.AntiDiagonal[antiDiagonalIndex];

        moveSet |= (topRight ^ bottomLeft) & Masks.Diagonal[diagonalIndex];
        moveSet |= (topLeft ^ bottomRight) & Masks.AntiDiagonal[antiDiagonalIndex];

        return moveSet;
    }

    public static Bitboard KnightMoves(int index) {
        Bitboard moveSet = Masks.KnightMoves;

        // Knight moves are the same from every square
        // We can keep one copy of it and shift it to the correct position
        if (index > 18) {   
            moveSet <<= index - 18;
        } else {
            moveSet >>= 18 - index;
        }

        // If knight is on the first half of the board, remove all the moves that go to the rightmost two columns
        // If knight is on the second half of the board, remove all the moves that go to the leftmost two columns
        if (index % 8 < 4) {
            moveSet &= ~(Masks.Column[6] | Masks.Column[7]);
        } else {
            moveSet &= ~(Masks.Column[0] | Masks.Column[1]);
        }

        return moveSet;
    }

    public static Bitboard KingMoves(int index) {
        Bitboard moveSet = Masks.KingMoves;

        // King moves are the same from every square
        // We can keep one copy of it and shift it to the correct position
        if (index > 9) {
            moveSet <<= index - 9;
        } else {
            moveSet >>= 9 - index;
        }

        // If king is on the first half of the board, remove all the moves that go to the rightmost column
        // If king is on the second half of the board, remove all the moves that go to the leftmost column
        if (index % 8 == 0) {
            moveSet &= ~Masks.Column[7];
        } else if (index % 8 == 7) {
            moveSet &= ~Masks.Column[0];
        }

        return moveSet;
    }

    // Key idea: Get all the squares that are attacked by the enemy pieces
    // Can be done by generating all the moves for the enemy pieces and combining them
    // Pseudo-legal moves need to be calculated only, as the king can't move to a square that is attacked by the enemy, even if the enemy piece is pinned
    public static Bitboard GetUnsafeSquares(Board board, bool color) {
        Bitboard result = Bitboard.Null;

        // Enemy pawns
        Bitboard pawns = board.Type[Piece.Pawn] & board.Color[!color];
        if (color) {
            result |= (pawns >> 7) & ~Masks.Column[0];
            result |= (pawns >> 9) & ~Masks.Column[7];
        } else {
            result |= (pawns << 7) & ~Masks.Column[7];
            result |= (pawns << 9) & ~Masks.Column[0];
        }

        // Enemy knights
        Bitboard knights = board.Type[Piece.Knight] & board.Color[!color];
        while (!knights.IsEmpty) {
            int index = knights.FirstBit;
        
            result |= KnightMoves(index);

            knights.ClearBit(index);
        }

        // Enemy sliding pieces
        Bitboard straight = (board.Type[Piece.Rook] | board.Type[Piece.Queen]) & board.Color[!color];
        while (!straight.IsEmpty) {
            int index = straight.FirstBit;

            Bitboard moveSet = StraightMoves(board, index);
            result |= moveSet;

            straight.ClearBit(index);
        }

        Bitboard diagonal = (board.Type[Piece.Bishop] | board.Type[Piece.Queen]) & board.Color[!color];
        while (!diagonal.IsEmpty) {
            int index = diagonal.FirstBit;

            Bitboard moveSet = DiagonalMoves(board, index);
            result |= moveSet;

            diagonal.ClearBit(index);
        }

        // Enemy king
        Bitboard kings = board.Type[Piece.King] & board.Color[!color];
        int kingIndex = kings.FirstBit;

        result |= KingMoves(kingIndex);

        return result;
    }

    public static bool IsInCheck(Board board, bool color) {
        Bitboard unsafeSquares = GetUnsafeSquares(board, color);
        return !(unsafeSquares & board.Type[Piece.King] & board.Color[color]).IsEmpty;
    }

    public static Bitboard GetEnPassant(Board board) {
        if (board.EnPassantSquare == -1) return new Bitboard(0);
        return BitboardHelper.GetBitAt(board.EnPassantSquare);
    }

    // Extract all moves for pawns
    // All pawns moves are generated all together, so considering pins and en passant is tricky
    public static List<Move> ExtractPawnMoves(Bitboard bitboard, int dir, bool promotion = false, bool enPassant = false, Dictionary<int, Bitboard>? pins = null, Board? board = null) {
        List<Move> moves = new List<Move>();
        while (!bitboard.IsEmpty) {
            int index = bitboard.FirstBit;
            int source = index - dir;

            if (pins != null && pins.ContainsKey(source)) {
                Bitboard pin = pins[source];

                if ((pin & BitboardHelper.GetBitAt(index)).IsEmpty) {
                    bitboard.ClearBit(index);
                    continue;
                }
            }

            if (promotion){
                moves.Add(new Move(index - dir, index, Move.QueenPromotion));
                moves.Add(new Move(index - dir, index, Move.BishopPromotion));
                moves.Add(new Move(index - dir, index, Move.KnightPromotion));
                moves.Add(new Move(index - dir, index, Move.RookPromotion));
            } else if (enPassant && board != null) {
                Move move = new Move(index - dir, index, Move.EnPassant);

                board.MakeMove(move);
                if (!IsInCheck(board, !board.IsWhiteTurn)) {
                    moves.Add(move);
                }
                board.UnmakeMove(move);
            }
            else {
                moves.Add(new Move(index - dir, index));
            }

            bitboard.ClearBit(index); 
        }

        return moves;
    }

    // Extract all moves for any other piece
    // All other pieces moves are generated one by one, so considering pins are not necessary
    public static List<Move> ExtractMoves(Bitboard bitboard, int index) {
        List<Move> moves = new List<Move>();
        while (!bitboard.IsEmpty) {
            int target = bitboard.FirstBit;
            moves.Add(new Move(index, target));
            
            bitboard.ClearBit(target);
        }

        return moves;
    }
}