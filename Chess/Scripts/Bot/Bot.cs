namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

class Bot {
    static Random rnd = new Random();
    static int positiveInfinity = 1000000;
    static int negativeInfinity = -1000000;
    static Move bestMove = Move.NullMove;

    public static Move Think(Board board) {
        bestMove = Move.NullMove;
        Search(board, 3);

        return bestMove;
    }

    public static int Search(Board board, int depth, bool firstCall = true) {
        if (depth == 0) return Evaluation.Evaluate(board);

        List<Move> moves = MoveGenerator.GenerateMoves(board);
        if (moves.Count == 0) {
            if (BitboardHelper.IsInCheck(board, board.IsWhiteTurn)) {
                return negativeInfinity;
            }
            return 0;
        }

        int bestEval = negativeInfinity;
        foreach (Move move in moves) {
            board.MakeMove(move);

            int eval = -Search(board, depth - 1, false);
            if (eval > bestEval) {
                bestEval = eval;
                if (firstCall) {
                    bestMove = move;
                }
            }

            board.UnmakeMove(move);
        }

        return bestEval;
    }
}