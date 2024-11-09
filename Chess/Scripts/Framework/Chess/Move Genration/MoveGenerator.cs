namespace Chess.ChessEngine;

using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Chess.API;

class MoveGenerator {
    public static List<Move> GenerateMoves(Board board, int? squareIndex = null) {
        // Generates all possible moves for the current player
        List<Move> moves = [
            .. PawnMoves(board),
            .. KnightMoves(board),
            .. SlidingPieceMoves(board),
            .. KingMoves(board)
        ];

        // If specific square is provided, filter moves to only include moves from that square
        if (squareIndex != -1)  moves = moves.Where(move => move.Source == squareIndex).ToList();

        return moves;
    }

    public static List<Move> PawnMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard pawns = board.Bitboard[PieceType.Pawn] & board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black];
        Bitboard empty = board.Empty;

        Bitboard singlePush, doublePush, rightCapture, leftCapture;
        Bitboard regularPromotion, rightCapturePromotion, leftCapturePromotion;

        // Single push: Move one square forward and check if the corresponding squares are empty
        // Double push: Move two squares forward and check if the corresponding squares are empty, squares in between are empty, and the pawn is in the starting position
        // Right capture: Move one square diagonally right and check if the corresponding square is occupied by an enemy piece, and the pawn is not on the rightmost column
        // Left capture: Move one square diagonally left and check if the corresponding square is occupied by an enemy piece, and the pawn is not on the leftmost column
        // Regular promotion: Move to the last row and promote to a any piece except a king
        if (board.IsWhiteTurn) {
            singlePush = (pawns << 8) & empty & ~Masks.Row[7];
            doublePush = (pawns << 16) & empty & (empty << 8) & Masks.Row[3];
            rightCapture = (pawns << 9) & board.Bitboard[PieceType.Black] & ~Masks.Column[0] & ~Masks.Row[7];
            leftCapture = (pawns << 7) & board.Bitboard[PieceType.Black] & ~Masks.Column[7] & ~Masks.Row[7];
            regularPromotion = (pawns << 8) & empty & Masks.Row[7];
            rightCapturePromotion = (pawns << 9) & board.Bitboard[PieceType.Black] & ~Masks.Column[0] & Masks.Row[7];
            leftCapturePromotion = (pawns << 7) & board.Bitboard[PieceType.Black] & ~Masks.Column[7] & Masks.Row[7];
        } else {
            singlePush = (pawns >> 8) & empty & ~Masks.Row[0];
            doublePush = (pawns >> 16) & empty & (empty >> 8) & Masks.Row[4];
            rightCapture = (pawns >> 9) & board.Bitboard[PieceType.White] & ~Masks.Column[7] & ~Masks.Row[0];
            leftCapture = (pawns >> 7) & board.Bitboard[PieceType.White] & ~Masks.Column[0] & ~Masks.Row[0];
            regularPromotion = (pawns >> 8) & empty & Masks.Row[0];
            rightCapturePromotion = (pawns >> 9) & board.Bitboard[PieceType.White] & ~Masks.Column[7] & Masks.Row[0];
            leftCapturePromotion = (pawns >> 7) & board.Bitboard[PieceType.White] & ~Masks.Column[0] & Masks.Row[0];
        }

        // Extract moves from bitboards
        // Check rightmost bit, add it to the list, and then clear it
        moves.AddRange(BitboardHelper.ExtractPawnMoves(singlePush, board.IsWhiteTurn ? 8 : -8));
        moves.AddRange(BitboardHelper.ExtractPawnMoves(doublePush, board.IsWhiteTurn ? 16 : -16));
        moves.AddRange(BitboardHelper.ExtractPawnMoves(rightCapture, board.IsWhiteTurn ? 9 : -9));
        moves.AddRange(BitboardHelper.ExtractPawnMoves(leftCapture, board.IsWhiteTurn ? 7 : -7));
        moves.AddRange(BitboardHelper.ExtractPawnMoves(regularPromotion, board.IsWhiteTurn ? 8 : -8, true));
        moves.AddRange(BitboardHelper.ExtractPawnMoves(rightCapturePromotion, board.IsWhiteTurn ? 9 : -9, true));
        moves.AddRange(BitboardHelper.ExtractPawnMoves(leftCapturePromotion, board.IsWhiteTurn ? 7 : -7, true));    

        moves.AddRange(EnPassantMoves(board, board.IsWhiteTurn ? PieceType.White : PieceType.Black));

        return moves;
    }

    public static List<Move> KnightMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard knights = board.Bitboard[PieceType.Knight] & board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black];

        while (!knights.IsEmpty) {
            int index = knights.FirstBit;
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

            // Remove all the moves that go to the squares occupied by the same color pieces to avoid same color capturing
            moveSet &= board.Empty | board.Bitboard[board.IsWhiteTurn ? PieceType.Black : PieceType.White];
            
            moves.AddRange(BitboardHelper.ExtractMoves(moveSet, index));

            knights.ClearBit(index);
        }

        return moves;
    }

    // Sliding pieces include rooks, bishops, and queens
    // They can move in any direction until they hit a piece or the edge of the board
    public static List<Move> SlidingPieceMoves(Board board) {
        List<Move> moves = new List<Move>();

        // Get all the rooks and queens for the horizontal and vertical moves
        Bitboard straight = (board.Bitboard[PieceType.Rook] | board.Bitboard[PieceType.Queen]) & board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black];
        // Get all the bishops and queens for the diagonal moves
        Bitboard diagonal = (board.Bitboard[PieceType.Bishop] | board.Bitboard[PieceType.Queen]) & board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black];

        // Straight moves first
        while (!straight.IsEmpty) {
            Bitboard result = Bitboard.Null;

            int index = straight.FirstBit;
            int row = BoardHelper.RowIndex(index);
            int col = BoardHelper.ColumnIndex(index);

            Bitboard occupied = board.Occupied;

            // Step 1: Isolate the current row where the rook is located.
            // We'll use a mask to capture only the occupied squares in this row, excluding the rook itself.

            // Step 2: Subtract the rook's bitboard twice from the masked occupied squares.
            // This subtraction creates a "shifted" bit pattern, setting all bits between the rook
            // and the nearest occupied square to its left (or the edge of the board) to 1.

            // Formula Explanation: ((occupied & mask) - 2 * (rook)) & mask
            // - 'occupied & mask' gives the occupied squares in the rook's row.
            // - Subtracting '2 * rook' from this masked row shifts bits, creating a solid block of 1s from the rook to the left.
            // - Finally, '& mask' ensures that only bits within the current row are preserved.
            Bitboard left = ((occupied & Masks.Row[row]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.Row[row];
            Bitboard right = (((occupied & Masks.Row[row]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.Row[row];


            // Same idea as above, but for the column
            Bitboard bottom = ((occupied & Masks.Column[col]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.Column[col];
            Bitboard top = (((occupied & Masks.Column[col]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.Column[col];

            // combine the moves from the left and right, and top and bottom by xor-ing them and getting rid of unwanted duplicates
            result |= (left ^ right) & Masks.Row[row];
            result |= (top ^ bottom) & Masks.Column[col];

            // Result above still considers friendly pieces as capturable, so we need to remove them
            result &= board.Bitboard[board.IsWhiteTurn ? PieceType.Black : PieceType.White] | board.Empty;

            moves.AddRange(BitboardHelper.ExtractMoves(result, index));

            straight.ClearBit(index);
        }

        // Same idea as above, but for the diagonal moves   
        while (!diagonal.IsEmpty) {
            Bitboard result = Bitboard.Null;

            int index = diagonal.FirstBit;
            int row = BoardHelper.RowIndex(index);
            int col = BoardHelper.ColumnIndex(index);

            Bitboard occupied = board.Occupied;

            int diagonalIndex = row - col + 7; // Diagonals have an unique property that the difference between row and column is always the same
            int antiDiagonalIndex = row + col; // Anti-diagonals have an unique property that the sum of row and column is always the same

            Bitboard topRight = ((occupied & Masks.Diagonal[diagonalIndex]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.Diagonal[diagonalIndex];
            Bitboard bottomLeft = (((occupied & Masks.Diagonal[diagonalIndex]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.Diagonal[diagonalIndex];

            Bitboard topLeft = ((occupied & Masks.AntiDiagonal[antiDiagonalIndex]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.AntiDiagonal[antiDiagonalIndex];
            Bitboard bottomRight = (((occupied & Masks.AntiDiagonal[antiDiagonalIndex]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.AntiDiagonal[antiDiagonalIndex];

            result |= (topRight ^ bottomLeft) & Masks.Diagonal[diagonalIndex];
            result |= (topLeft ^ bottomRight) & Masks.AntiDiagonal[antiDiagonalIndex];

            result &= board.Bitboard[board.IsWhiteTurn ? PieceType.Black : PieceType.White] | board.Empty;

            moves.AddRange(BitboardHelper.ExtractMoves(result, index));

            diagonal.ClearBit(index);
        }

        return moves;
    }

    public static List<Move> KingMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard kings = board.Bitboard[PieceType.King] & board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black];

        int index = kings.FirstBit;
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

        // Remove all the moves that go to the squares occupied by the same color pieces to avoid same color capturing
        moveSet &= board.Empty | board.Bitboard[board.IsWhiteTurn ? PieceType.Black : PieceType.White];

        // Get all the unsafe squares for the king
        Board boardWithoutKing = board;
        boardWithoutKing.Bitboard[PieceType.King].ClearBit(index);
        boardWithoutKing.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].ClearBit(index);
        
        moveSet &= ~GetUnsafeSquares(boardWithoutKing, board.IsWhiteTurn ? PieceType.White : PieceType.Black);

        boardWithoutKing.Bitboard[PieceType.King].SetBit(index);
        boardWithoutKing.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black].SetBit(index);

        moves.AddRange(BitboardHelper.ExtractMoves(moveSet, index));
        moves.AddRange(CastlingMoves(board, index));

        kings.ClearBit(index);

        return moves;
    }

    // Key idea: Get all the squares that are attacked by the enemy pieces
    // Can be done by generating all the moves for the enemy pieces and combining them
    // Pseudo-legal moves need to be calculated only, as the king can't move to a square that is attacked by the enemy, even if the enemy piece is pinned
    public static Bitboard GetUnsafeSquares(Board board, int type) {
        Bitboard result = Bitboard.Null;

        // Enemy pawns
        Bitboard pawns = board.Bitboard[PieceType.Pawn] & board.Bitboard[24 - type];
        if (type == PieceType.White) {
            result |= (pawns >> 7) & ~Masks.Column[0];
            result |= (pawns >> 9) & ~Masks.Column[7];
        } else {
            result |= (pawns << 7) & ~Masks.Column[7];
            result |= (pawns << 9) & ~Masks.Column[0];
        }

        // Enemy knights
        Bitboard knights = board.Bitboard[PieceType.Knight] & board.Bitboard[24 - type];
        while (!knights.IsEmpty) {
            int index = knights.FirstBit;
            Bitboard moveSet = Masks.KnightMoves;

            if (index > 18) {   
                moveSet <<= index - 18;
            } else {
                moveSet >>= 18 - index;
            }

            if (index % 8 < 4) {
                moveSet &= ~(Masks.Column[6] | Masks.Column[7]);
            } else {
                moveSet &= ~(Masks.Column[0] | Masks.Column[1]);
            }

            result |= moveSet;

            knights.ClearBit(index);
        }

        // Enemy sliding pieces
        Bitboard straight = (board.Bitboard[PieceType.Rook] | board.Bitboard[PieceType.Queen]) & board.Bitboard[24 - type];
        while (!straight.IsEmpty) {
            Bitboard moveSet = Bitboard.Null;

            int index = straight.FirstBit;
            int row = BoardHelper.RowIndex(index);
            int col = BoardHelper.ColumnIndex(index);

            Bitboard occupied = board.Occupied;

            Bitboard left = ((occupied & Masks.Row[row]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.Row[row];
            Bitboard right = (((occupied & Masks.Row[row]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.Row[row];

            Bitboard bottom = ((occupied & Masks.Column[col]) - (BitboardHelper.GetBitAt(index) << 1)) & Masks.Column[col];
            Bitboard top = (((occupied & Masks.Column[col]).Reverse() - (BitboardHelper.GetBitAt(index).Reverse() << 1)).Reverse()) & Masks.Column[col];

            moveSet |= (left ^ right) & Masks.Row[row];
            moveSet |= (top ^ bottom) & Masks.Column[col];

            result |= moveSet;

            straight.ClearBit(index);
        }

        Bitboard diagonal = (board.Bitboard[PieceType.Bishop] | board.Bitboard[PieceType.Queen]) & board.Bitboard[24 - type];
        while (!diagonal.IsEmpty) {
            Bitboard moveSet = Bitboard.Null;

            int index = diagonal.FirstBit;
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

            result |= moveSet;

            diagonal.ClearBit(index);
        }

        // Enemy king
        Bitboard kings = board.Bitboard[PieceType.King] & board.Bitboard[24 - type];

        int kingIndex = kings.FirstBit;
        Bitboard kingMoveSet = Masks.KingMoves;

        if (kingIndex > 9) {   
            kingMoveSet <<= kingIndex - 9;
        } else {
            kingMoveSet >>= 9 - kingIndex;
        }

        if (kingIndex % 8 == 0) {
            kingMoveSet &= ~Masks.Column[7];
        } else if (kingIndex % 8 == 7) {
            kingMoveSet &= ~Masks.Column[0];
        }

        result |= kingMoveSet;

        return result;
    }

    public static List<Move> CastlingMoves(Board board, int kingIndex) {
        List<Move> moves = new List<Move>();
        Bitboard unsafeBitboard = GetUnsafeSquares(board, board.IsWhiteTurn ? PieceType.White : PieceType.Black);

        // If king is in check, player can't castle
        if ((unsafeBitboard & BitboardHelper.GetBitAt(kingIndex)) != Bitboard.Null) return moves;
        if (board.IsWhiteTurn) {
            if ((board.CastlingRights & 0b1000) != 0) {
                // White king side castling
                if ((board.Occupied & new Bitboard(0b01100000)) == Bitboard.Null && (unsafeBitboard & new Bitboard(0b01100000)) == Bitboard.Null) {
                    moves.Add(new Move(4, 6, Move.Castling));
                }
            }
            if ((board.CastlingRights & 0b0100) != 0) {
                // White queen side castling
                if ((board.Occupied & new Bitboard(0b00001110)) == Bitboard.Null && (unsafeBitboard & new Bitboard(0b00001100)) == Bitboard.Null) {
                    moves.Add(new Move(4, 2, Move.Castling));
                }
            }
        } else {
            if ((board.CastlingRights & 0b0010) != 0) {
                // Black king side castling
                if ((board.Occupied & new Bitboard(0x6000000000000000)) == Bitboard.Null && (unsafeBitboard & new Bitboard(0x6000000000000000)) == Bitboard.Null) {
                    moves.Add(new Move(60, 62, Move.Castling));
                }
            }
            if ((board.CastlingRights & 0b0001) != 0) {
                // Black queen side castling
                if ((board.Occupied & new Bitboard(0x0E00000000000000)) == Bitboard.Null && (unsafeBitboard & new Bitboard(0x0C00000000000000)) == Bitboard.Null) {
                    moves.Add(new Move(60, 58, Move.Castling));
                }
            }
        }

        return moves;
    }

    public static List<Move> EnPassantMoves(Board board, int type) {
        List<Move> moves = new List<Move>();

        Bitboard enPassantSquare = BitboardHelper.GetEnPassant(board);
        Bitboard left, right;
        Bitboard whitePawns = board.Bitboard[PieceType.Pawn] & board.Bitboard[PieceType.White];
        Bitboard blackPawns = board.Bitboard[PieceType.Pawn] & board.Bitboard[PieceType.Black];

        if (board.IsWhiteTurn) {
            right = (whitePawns << 9) & ~Masks.Column[0] & enPassantSquare;
            left = (whitePawns << 7) & ~Masks.Column[7] & enPassantSquare;
        } else {
            right = (blackPawns >> 9) & ~Masks.Column[7] & enPassantSquare;
            left = (blackPawns >> 7) & ~Masks.Column[0] & enPassantSquare;
        } 

        Console.WriteLine(enPassantSquare.Binary);
        Console.WriteLine(right.Binary);

        moves.AddRange(BitboardHelper.ExtractPawnMoves(right, board.IsWhiteTurn ? 9 : -9));
        moves.AddRange(BitboardHelper.ExtractPawnMoves(left, board.IsWhiteTurn ? 7 : -7));

        return moves;
    }
}
