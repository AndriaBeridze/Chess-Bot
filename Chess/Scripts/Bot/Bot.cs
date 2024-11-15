namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

class Bot {
    static Random rnd = new Random();
    const int positiveInfinity = 1000000;
    const int negativeInfinity = -1000000;
    static Move bestMove = Move.NullMove;

    public static Move Think(Board board) {
        bestMove = Move.NullMove;
        Search(board, 5);

        return bestMove;
    }

    public static int Search(Board board, int depth, bool firstCall = true, int alpha = negativeInfinity, int beta = positiveInfinity) {
        if (depth == 0) return Evaluation.Evaluate(board);

        List<Move> moves = MoveGenerator.GenerateMoves(board);
        if (moves.Count == 0) {
            if (BitboardHelper.IsInCheck(board, board.IsWhiteTurn)) {
                return negativeInfinity;
            }
            return 0;
        }

        foreach (Move move in moves) {
            board.MakeMove(move);
            int eval = -Search(board, depth - 1, false, -beta, -alpha);
            board.UnmakeMove(move);

            if (eval >= beta) {
                return beta;
            }

            if (alpha < eval) {
                alpha = eval;
                if (firstCall) {
                    bestMove = move;
                }
            }
        }

        return alpha;
    }
}