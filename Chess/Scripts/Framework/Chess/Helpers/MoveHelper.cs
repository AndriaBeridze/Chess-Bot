namespace Chess.ChessEngine;

using Chess.API;

class MoveHelper {
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
                if (!BitboardHelper.IsInCheck(board, !board.IsWhiteTurn)) {
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