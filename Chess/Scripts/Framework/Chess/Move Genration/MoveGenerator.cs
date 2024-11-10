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
        if (squareIndex != null)  moves = moves.Where(move => move.Source == squareIndex).ToList();

        return moves;
    }

    public static List<Move> PawnMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard pawns = board.Type[PieceType.Pawn] & board.Color[board.IsWhiteTurn];
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

        // Extract moves from bitboards
        // Check rightmost bit, add it to the list, and then clear it
        moves.AddRange(MoveHelper.ExtractPawnMoves(singlePush, board.IsWhiteTurn ? 8 : -8));
        moves.AddRange(MoveHelper.ExtractPawnMoves(doublePush, board.IsWhiteTurn ? 16 : -16));
        moves.AddRange(MoveHelper.ExtractPawnMoves(rightCapture, board.IsWhiteTurn ? 9 : -9));
        moves.AddRange(MoveHelper.ExtractPawnMoves(leftCapture, board.IsWhiteTurn ? 7 : -7));
        moves.AddRange(MoveHelper.ExtractPawnMoves(regularPromotion, board.IsWhiteTurn ? 8 : -8, promotion: true));
        moves.AddRange(MoveHelper.ExtractPawnMoves(rightCapturePromotion, board.IsWhiteTurn ? 9 : -9, promotion: true));
        moves.AddRange(MoveHelper.ExtractPawnMoves(leftCapturePromotion, board.IsWhiteTurn ? 7 : -7, promotion: true));    

        moves.AddRange(EnPassantMoves(board, board.IsWhiteTurn ? PieceType.White : PieceType.Black));

        return moves;
    }

    public static List<Move> KnightMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard knights = board.Type[PieceType.Knight] & board.Color[board.IsWhiteTurn];

        while (!knights.IsEmpty) {
            int index = knights.FirstBit;

            Bitboard moveSet = BitboardHelper.KnightMoves(index);
            moveSet &= board.Empty | board.Color[!board.IsWhiteTurn]; // Remove all the moves that go to the squares occupied by the same color pieces to avoid same color capturing
            
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
        Bitboard straight = (board.Type[PieceType.Rook] | board.Type[PieceType.Queen]) & board.Color[board.IsWhiteTurn];
        // Get all the bishops and queens for the diagonal moves
        Bitboard diagonal = (board.Type[PieceType.Bishop] | board.Type[PieceType.Queen]) & board.Color[board.IsWhiteTurn];

        // Straight moves first
        while (!straight.IsEmpty) {
            int index = straight.FirstBit;

            Bitboard result = BitboardHelper.StraightMoves(board, index);
            result &= board.Color[!board.IsWhiteTurn] | board.Empty; // Result above still considers friendly pieces as capturable, so we need to remove them

            moves.AddRange(MoveHelper.ExtractMoves(result, index));

            straight.ClearBit(index);
        }

        // Same idea as above, but for the diagonal moves   
        while (!diagonal.IsEmpty) {
            int index = diagonal.FirstBit;

            Bitboard result = BitboardHelper.DiagonalMoves(board, index);
            result &= board.Color[!board.IsWhiteTurn] | board.Empty;

            moves.AddRange(MoveHelper.ExtractMoves(result, index));

            diagonal.ClearBit(index);
        }

        return moves;
    }

    public static List<Move> KingMoves(Board board) {
        List<Move> moves = new List<Move>();

        Bitboard kings = board.Type[PieceType.King] & board.Color[board.IsWhiteTurn];

        int index = kings.FirstBit;
        Bitboard moveSet = BitboardHelper.KingMoves(index);

        // Remove all the moves that go to the squares occupied by the same color pieces to avoid same color capturing
        moveSet &= board.Empty | board.Color[!board.IsWhiteTurn];

        // Get all the unsafe squares for the king
        board.Type[PieceType.King].ClearBit(index);
        board.Color[board.IsWhiteTurn].ClearBit(index);
        
        moveSet &= ~BitboardHelper.GetUnsafeSquares(board, board.IsWhiteTurn);

        board.Type[PieceType.King].SetBit(index);
        board.Color[board.IsWhiteTurn].SetBit(index);

        moves.AddRange(MoveHelper.ExtractMoves(moveSet, index));
        moves.AddRange(CastlingMoves(board, index));

        kings.ClearBit(index);

        return moves;
    }

    public static List<Move> CastlingMoves(Board board, int kingIndex) {
        List<Move> moves = new List<Move>();
        Bitboard unsafeBitboard = BitboardHelper.GetUnsafeSquares(board, board.IsWhiteTurn);

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
        Bitboard pawns = board.Type[PieceType.Pawn] & board.Color[board.IsWhiteTurn];

        if (board.IsWhiteTurn) {
            right = (pawns << 9) & ~Masks.Column[0] & enPassantSquare;
            left = (pawns << 7) & ~Masks.Column[7] & enPassantSquare;
        } else {
            right = (pawns >> 9) & ~Masks.Column[7] & enPassantSquare;
            left = (pawns >> 7) & ~Masks.Column[0] & enPassantSquare;
        } 

        moves.AddRange(MoveHelper.ExtractPawnMoves(right, board.IsWhiteTurn ? 9 : -9, enPassant: true));
        moves.AddRange(MoveHelper.ExtractPawnMoves(left, board.IsWhiteTurn ? 7 : -7, enPassant: true));

        return moves;
    }

    // Temporary function to get legal moves before implementing move validation
    public static List<Move> LegalMoves(Board board, int? index = null) {
        List<Move> moves = GenerateMoves(board, index);
        List<Move> legalMoves = new List<Move>();

        foreach (Move move in moves) {
            board.MakeMove(move);

            if (!BitboardHelper.IsInCheck(board, !board.IsWhiteTurn)) legalMoves.Add(move);

            board.UnmakeMove(move);
        }

        return legalMoves;
    }
}
