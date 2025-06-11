namespace Chess.ChessEngine;

using Chess.API;

class MoveGenerator {
    private static Bitboard attackRays = new Bitboard(0xFFFFFFFFFFFFFFFF);
    private static List<int> attackerIndexes = new List<int>();
    private static Dictionary<int, Bitboard> pins = new Dictionary<int, Bitboard>();

    public static List<Move> GenerateMoves(Board board, int? squareIndex = null) {
        GenerateAttackRays(board);
        GetPinnedPieces(board);

        // If double check, only king can move
        if (attackerIndexes.Count == 2) {
            attackRays = Bitboard.Null;
        }

        // Generates all possible moves for the current player
        List<Move> moves = [
            .. PawnMoves(board),
            .. KnightMoves(board),
            .. SlidingPieceMoves(board),
            .. KingMoves(board)
        ];

        // If specific square is provided, filter moves to only include moves from that square
        if (squareIndex != null)  moves = moves.Where(move => move.Source == squareIndex).ToList();

        return moves;
    }

    public static List<Move> GenerateCaptureMoves(Board board) {
        List<Move> moves = GenerateMoves(board);

        return moves.Where(move => board.Square[move.Target].Type != Piece.None).ToList();
    }

    public static List<Move> PawnMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard pawns = board.Type[Piece.Pawn] & board.Color[board.IsWhiteTurn];
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
            rightCapture = (pawns << 9) & board.Color[false] & ~Masks.Column[0] & ~Masks.Row[7];
            leftCapture = (pawns << 7) & board.Color[false] & ~Masks.Column[7] & ~Masks.Row[7];
            regularPromotion = (pawns << 8) & empty & Masks.Row[7];
            rightCapturePromotion = (pawns << 9) & board.Color[false] & ~Masks.Column[0] & Masks.Row[7];
            leftCapturePromotion = (pawns << 7) & board.Color[false] & ~Masks.Column[7] & Masks.Row[7];
        } else {
            singlePush = (pawns >> 8) & empty & ~Masks.Row[0];
            doublePush = (pawns >> 16) & empty & (empty >> 8) & Masks.Row[4];
            rightCapture = (pawns >> 9) & board.Color[true] & ~Masks.Column[7] & ~Masks.Row[0];
            leftCapture = (pawns >> 7) & board.Color[true] & ~Masks.Column[0] & ~Masks.Row[0];
            regularPromotion = (pawns >> 8) & empty & Masks.Row[0];
            rightCapturePromotion = (pawns >> 9) & board.Color[true] & ~Masks.Column[7] & Masks.Row[0];
            leftCapturePromotion = (pawns >> 7) & board.Color[true] & ~Masks.Column[0] & Masks.Row[0];
        }

        singlePush &= attackRays;
        doublePush &= attackRays;
        rightCapture &= attackRays;
        leftCapture &= attackRays;
        regularPromotion &= attackRays;
        rightCapturePromotion &= attackRays;
        leftCapturePromotion &= attackRays;

        // Extract moves from bitboards
        // Check rightmost bit, add it to the list, and then clear it
        moves.AddRange(MoveHelper.ExtractPawnMoves(singlePush, board.IsWhiteTurn ? 8 : -8, pins: pins));
        moves.AddRange(MoveHelper.ExtractPawnMoves(doublePush, board.IsWhiteTurn ? 16 : -16, pins: pins));
        moves.AddRange(MoveHelper.ExtractPawnMoves(rightCapture, board.IsWhiteTurn ? 9 : -9, pins: pins));
        moves.AddRange(MoveHelper.ExtractPawnMoves(leftCapture, board.IsWhiteTurn ? 7 : -7, pins: pins));
        moves.AddRange(MoveHelper.ExtractPawnMoves(regularPromotion, board.IsWhiteTurn ? 8 : -8, promotion: true, pins: pins));
        moves.AddRange(MoveHelper.ExtractPawnMoves(rightCapturePromotion, board.IsWhiteTurn ? 9 : -9, promotion: true, pins: pins));
        moves.AddRange(MoveHelper.ExtractPawnMoves(leftCapturePromotion, board.IsWhiteTurn ? 7 : -7, promotion: true, pins: pins));    

        moves.AddRange(EnPassantMoves(board, board.IsWhiteTurn ? Piece.White : Piece.Black));

        return moves;
    }

    public static List<Move> KnightMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard knights = board.Type[Piece.Knight] & board.Color[board.IsWhiteTurn];

        while (!knights.IsEmpty) {
            int index = knights.FirstBit;

            Bitboard moveSet = Masks.KnightAttacks[index];
            moveSet &= board.Empty | board.Color[!board.IsWhiteTurn]; // Remove all the moves that go to the squares occupied by the same color pieces to avoid same color capturing
            moveSet &= attackRays;
            if (pins.ContainsKey(index)) moveSet &= pins[index];

            moves.AddRange(MoveHelper.ExtractMoves(moveSet, index));

            knights.ClearBit(index);
        }

        return moves;
    }

    // Sliding pieces include rooks, bishops, and queens
    // They can move in any direction until they hit a piece or the edge of the board
    public static List<Move> SlidingPieceMoves(Board board) {
        List<Move> moves = new List<Move>();

        // Get all the rooks and queens for the horizontal and vertical moves
        Bitboard straight = (board.Type[Piece.Rook] | board.Type[Piece.Queen]) & board.Color[board.IsWhiteTurn];
        // Get all the bishops and queens for the diagonal moves
        Bitboard diagonal = (board.Type[Piece.Bishop] | board.Type[Piece.Queen]) & board.Color[board.IsWhiteTurn];

        // Straight moves first
        while (!straight.IsEmpty) {
            int index = straight.FirstBit;

            Bitboard result = MoveHelper.StraightMoves(board, index);
            result &= board.Color[!board.IsWhiteTurn] | board.Empty; // Result above still considers friendly pieces as capturable, so we need to remove them
            result &= attackRays;
            if (pins.ContainsKey(index)) result &= pins[index];

            moves.AddRange(MoveHelper.ExtractMoves(result, index));

            straight.ClearBit(index);
        }

        // Same idea as above, but for the diagonal moves   
        while (!diagonal.IsEmpty) {
            int index = diagonal.FirstBit;

            Bitboard result = MoveHelper.DiagonalMoves(board, index);
            result &= board.Color[!board.IsWhiteTurn] | board.Empty;
            result &= attackRays;
            if (pins.ContainsKey(index)) result &= pins[index];

            moves.AddRange(MoveHelper.ExtractMoves(result, index));

            diagonal.ClearBit(index);
        }

        return moves;
    }

    public static List<Move> KingMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard kings = board.Type[Piece.King] & board.Color[board.IsWhiteTurn];

        int index = kings.FirstBit;
        Bitboard moveSet = Masks.KingAttacks[index];

        // Remove all the moves that go to the squares occupied by the same color pieces to avoid same color capturing
        moveSet &= board.Empty | board.Color[!board.IsWhiteTurn];

        // Get all the unsafe squares for the king
        board.Type[Piece.King].ClearBit(index);
        board.Color[board.IsWhiteTurn].ClearBit(index);
        
        moveSet &= ~MoveHelper.GetUnsafeSquares(board, board.IsWhiteTurn);

        board.Type[Piece.King].SetBit(index);
        board.Color[board.IsWhiteTurn].SetBit(index);

        moves.AddRange(MoveHelper.ExtractMoves(moveSet, index));
        moves.AddRange(CastlingMoves(board, index));

        kings.ClearBit(index);

        return moves;
    }

    public static List<Move> CastlingMoves(Board board, int kingIndex) {
        List<Move> moves = new List<Move>();
        Bitboard unsafeBitboard = MoveHelper.GetUnsafeSquares(board, board.IsWhiteTurn);

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

        Bitboard enPassantSquare = MoveHelper.GetEnPassant(board);
        Bitboard left, right;
        Bitboard pawns = board.Type[Piece.Pawn] & board.Color[board.IsWhiteTurn];

        if (board.IsWhiteTurn) {
            right = (pawns << 9) & ~Masks.Column[0] & enPassantSquare;
            left = (pawns << 7) & ~Masks.Column[7] & enPassantSquare;
        } else {
            right = (pawns >> 9) & ~Masks.Column[7] & enPassantSquare;
            left = (pawns >> 7) & ~Masks.Column[0] & enPassantSquare;
        } 

        moves.AddRange(MoveHelper.ExtractPawnMoves(right, board.IsWhiteTurn ? 9 : -9, enPassant: true, board: board));
        moves.AddRange(MoveHelper.ExtractPawnMoves(left, board.IsWhiteTurn ? 7 : -7, enPassant: true, board: board));

        return moves;
    }

    // Generate attack rays for the current player
    // If queen on d5 is attacking the king on d8, the attack ray is d5-d8
    //     - All the bits corresponding to the squares between d5 and d8 are set to 1
    // Key idea:
    //     - Treat friendly king as an every piece combined and generate attack rays
    //     - Check if the attack rays intersect with any of the enemy pieces
    //     - If the above condition is met, the piece is attacking the king
    //     - Get the ray by bitwise AND the move mask from king and from the enemy piece
    //     - Update the attack rays
    public static void GenerateAttackRays(Board board) {
        attackRays = new Bitboard(0xFFFFFFFFFFFFFFFF);
        Bitboard temp = new Bitboard(0x0000000000000000);
        attackerIndexes = new List<int>();

        int kingIndex = (board.Type[Piece.King] & board.Color[board.IsWhiteTurn]).FirstBit;

        Bitboard pawns = board.Type[Piece.Pawn] & board.Color[!board.IsWhiteTurn];
        Bitboard knights = board.Type[Piece.Knight] & board.Color[!board.IsWhiteTurn];
        Bitboard bishops = board.Type[Piece.Bishop] & board.Color[!board.IsWhiteTurn];
        Bitboard rooks = board.Type[Piece.Rook] & board.Color[!board.IsWhiteTurn];
        Bitboard queens = board.Type[Piece.Queen] & board.Color[!board.IsWhiteTurn];
        
        // Pawns 
        if (board.IsWhiteTurn) {
            if (!((BitboardHelper.GetBitAt(kingIndex) << 7) & pawns & ~Masks.Column[7]).IsEmpty) {
                temp |= BitboardHelper.GetBitAt(kingIndex) << 7;
                attackerIndexes.Add(kingIndex + 7);
            }
            if (!((BitboardHelper.GetBitAt(kingIndex) << 9) & pawns & ~Masks.Column[0]).IsEmpty) {
                temp |= BitboardHelper.GetBitAt(kingIndex) << 9;
                attackerIndexes.Add(kingIndex + 9);
            }
        } else {
            if (!((BitboardHelper.GetBitAt(kingIndex) >> 7) & pawns & ~Masks.Column[0]).IsEmpty) {
                temp |= BitboardHelper.GetBitAt(kingIndex) >> 7;
                attackerIndexes.Add(kingIndex - 7);
            }
            if (!((BitboardHelper.GetBitAt(kingIndex) >> 9) & pawns & ~Masks.Column[7]).IsEmpty) {
                temp |= BitboardHelper.GetBitAt(kingIndex) >> 9;
                attackerIndexes.Add(kingIndex - 9);
            }
        }

        // Knights
        Bitboard knightMoves = Masks.KnightAttacks[kingIndex];
        if (!(knightMoves & knights).IsEmpty) {
            temp |= knightMoves & knights;
            attackerIndexes.Add((knightMoves & knights).FirstBit);
        }

        // Straight moves
        Bitboard straightAttackers = rooks | queens;
        Bitboard straightMoves = MoveHelper.StraightMoves(board, kingIndex);

        while (!straightAttackers.IsEmpty) {
            int index = straightAttackers.FirstBit;
            Bitboard straightAttack = MoveHelper.StraightMoves(board, index);

            if (!(straightAttack & BitboardHelper.GetBitAt(kingIndex)).IsEmpty) {
                temp |= straightAttack & straightMoves;
                temp |= BitboardHelper.GetBitAt(index);

                attackerIndexes.Add(index);
            }

            straightAttackers.ClearBit(index);
        }

        // Diagonal moves
        Bitboard diagonalAttackers = bishops | queens;
        Bitboard diagonalMoves = MoveHelper.DiagonalMoves(board, kingIndex);

        while (!diagonalAttackers.IsEmpty) {
            int index = diagonalAttackers.FirstBit;
            Bitboard diagonalAttack = MoveHelper.DiagonalMoves(board, index);

            if (!(diagonalAttack & BitboardHelper.GetBitAt(kingIndex)).IsEmpty) {
                temp |= diagonalAttack & diagonalMoves;
                temp |= BitboardHelper.GetBitAt(index);

                attackerIndexes.Add(index);
            }

            diagonalAttackers.ClearBit(index);
        }

        // Update the attack rays
        if (temp == Bitboard.Null) return;
        attackRays &= temp;
    }


    // Get all the pinned pieces
    // Key idea:
    //     - Treat king as a sliding piece and generate attack rays
    //     - Check if king rays intersect with any of the enemy sliding pieces and king is not in check
    //         - If the above condition is met, the piece is pinned
    //     - Generate the pin mask for the pinned piece
    //     - Add the pinned piece to the pins dictionary to access it later
    public static void GetPinnedPieces(Board board) {
        pins.Clear();

        int kingIndex = (board.Type[Piece.King] & board.Color[board.IsWhiteTurn]).FirstBit;

        Bitboard rooks = board.Type[Piece.Rook] & board.Color[!board.IsWhiteTurn];
        Bitboard bishops = board.Type[Piece.Bishop] & board.Color[!board.IsWhiteTurn];
        Bitboard queens = board.Type[Piece.Queen] & board.Color[!board.IsWhiteTurn];

        Bitboard straightAttackers = rooks | queens;
        Bitboard diagonalAttackers = bishops | queens;

        Bitboard straightMoves = MoveHelper.StraightMoves(board, kingIndex);
        Bitboard diagonalMoves = MoveHelper.DiagonalMoves(board, kingIndex);

        // Step 1: Treat king as a sliding piece and generate attack rays
        // Step 2: Check if king rays intersect with any of the enemy sliding pieces and king is not in check
        //         If the above condition is met, the piece is pinned
        // Step 4: Generate the pin mask for the pinned piece
        // Step 5: Add the pinned piece to the pins dictionary to access it later
        while (!straightAttackers.IsEmpty) {
            int index = straightAttackers.FirstBit;

            if (kingIndex % 8 == index % 8 || kingIndex / 8 == index / 8) {
                Bitboard straightAttack = MoveHelper.StraightMoves(board, index);

                if (!(straightAttack & straightMoves & board.Color[board.IsWhiteTurn]).IsEmpty && (straightAttack & BitboardHelper.GetBitAt(kingIndex)).IsEmpty) {
                    int pinnedPieceIndex = (straightAttack & straightMoves & board.Color[board.IsWhiteTurn]).FirstBit;
                    
                    pins.Add(pinnedPieceIndex, (straightAttack | straightMoves | BitboardHelper.GetBitAt(index)) & (MoveHelper.StraightMoves(board, pinnedPieceIndex) | BitboardHelper.GetBitAt(pinnedPieceIndex)));
                }
            }

            straightAttackers.ClearBit(index);
        }

        while (!diagonalAttackers.IsEmpty) {
            int index = diagonalAttackers.FirstBit;

            if (kingIndex % 8 - kingIndex / 8 == index % 8 - index / 8 || kingIndex % 8 + kingIndex / 8 == index % 8 + index / 8) {
                Bitboard diagonalAttack = MoveHelper.DiagonalMoves(board, index);

                if (!(diagonalAttack & diagonalMoves & board.Color[board.IsWhiteTurn]).IsEmpty && (diagonalAttack & BitboardHelper.GetBitAt(kingIndex)).IsEmpty) {
                    int pinnedPieceIndex = (diagonalAttack & diagonalMoves & board.Color[board.IsWhiteTurn]).FirstBit;

                    pins.Add(pinnedPieceIndex, (diagonalAttack | diagonalMoves | BitboardHelper.GetBitAt(index)) & (MoveHelper.DiagonalMoves(board, pinnedPieceIndex) | BitboardHelper.GetBitAt(pinnedPieceIndex)));
                }
            }

            diagonalAttackers.ClearBit(index);
        }

        return;
    }
}
