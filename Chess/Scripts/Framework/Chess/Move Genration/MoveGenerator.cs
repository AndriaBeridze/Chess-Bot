namespace Chess.ChessEngine;

using System.ComponentModel.DataAnnotations;
using Chess.API;

class MoveGenerator {
    public static List<Move> GenerateMoves(Board board, int? squareIndex = null) {
        // Generates all possible moves for the current player
        List<Move> moves = [
            .. GeneratePawnMoves(board),
            .. GenerateKnightMoves(board),
        ];

        // If specific square is provided, filter moves to only include moves from that square
        if (squareIndex != -1)  moves = moves.Where(move => move.Source == squareIndex).ToList();

        return moves;
    }

    public static List<Move> GeneratePawnMoves(Board board) {
        List<Move> moves = new List<Move>();

        ulong pawns = board.Bitboard[PieceType.Pawn] & board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black];
        ulong empty = board.Empty;

        ulong singlePush, doublePush, rightCapture, leftCapture;
        ulong regularPromotion, rightCapturePromotion, leftCapturePromotion;

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

        return moves;
    }

    public static List<Move> GenerateKnightMoves(Board board) {
        List<Move> moves = new List<Move>();

        ulong knights = board.Bitboard[PieceType.Knight] & board.Bitboard[board.IsWhiteTurn ? PieceType.White : PieceType.Black];

        while (knights != 0) {
            int index = BitboardHelper.GetFirstBit(knights);
            ulong moveSet = Masks.KnightMoves;

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
            
            moves.AddRange(BitboardHelper.ExtractKnightMoves(moveSet, index));

            BitboardHelper.ClearBit(ref knights, index);
        }

        return moves;
    }
}