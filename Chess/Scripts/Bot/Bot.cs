namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

class Bot {
    static Random rnd = new Random();
    const int positiveInfinity = 1000000;
    const int negativeInfinity = -1000000;
    static Move bestMove = Move.NullMove;

    static Dictionary<int, int> PieceValue = new Dictionary<int, int> {
        { PieceType.Pawn, Evaluation.Pawn },
        { PieceType.Knight, Evaluation.Knight },
        { PieceType.Bishop, Evaluation.Bishop },
        { PieceType.Rook, Evaluation.Rook },
        { PieceType.Queen, Evaluation.Queen },
        { PieceType.King, 1000000000 }
    };

    public static Move Think(Board board) {
        Search(board, 4);

        return bestMove;
    }

    public static int Search(Board board, int depth, bool firstCall = true, int alpha = negativeInfinity, int beta = positiveInfinity) {
        if (depth == 0) return SearchCapture(board, alpha, beta);

        List<Move> moves = MoveGenerator.GenerateMoves(board);
        Order(ref moves, board);
        if (moves.Count == 0) {
            if (BitboardHelper.IsInCheck(board, board.IsWhiteTurn)) {
                return negativeInfinity / 2 - depth;
            }
            return 0;
        }

        foreach (Move move in moves) {
            board.MakeMove(move);
            int eval = -Search(board, depth - 1, false, -beta, -alpha);
            board.UnmakeMove(move);

            if (eval >= beta) return beta;

            if (alpha < eval) {
                alpha = eval;
                if (firstCall) {
                    bestMove = move;
                }
            }
        }

        return alpha;
    }

    public static int SearchCapture(Board board, int alpha, int beta) {
        int eval = Evaluation.Evaluate(board);
        if (eval >= beta) return beta;

        alpha = Math.Max(alpha, eval);

        List<Move> moves = MoveGenerator.GenerateCaptureMoves(board);
        Order(ref moves, board);

        foreach (Move move in moves) {
            board.MakeMove(move);
            eval = -SearchCapture(board, -beta, -alpha);
            board.UnmakeMove(move);

            if (eval >= beta) return beta;

            alpha = Math.Max(alpha, eval);
        }

        return alpha;
    }

    public static void Order(ref List<Move> moves, Board board) {
        moves.Sort((a, b) => {
            return Score(b, board) - Score(a, board);
        });
    }

    public static int Score(Move move, Board board) {
        int score = 0;
        int movedPiece = board.Square[move.Source].Type;
        int capturedPiece = board.Square[move.Target].Type;

        if (capturedPiece != PieceType.None) {
            score += 10 * PieceValue[capturedPiece] - PieceValue[movedPiece];
        }

        if (move.Flag == Move.QueenPromotion) {
            score += PieceValue[PieceType.Queen];
        } else if (move.Flag == Move.RookPromotion) {
            score += PieceValue[PieceType.Rook];
        } else if (move.Flag == Move.BishopPromotion) {
            score += PieceValue[PieceType.Bishop];
        } else if (move.Flag == Move.KnightPromotion) {
            score += PieceValue[PieceType.Knight];
        }

        if (!(BitboardHelper.GetUnsafeSquares(board, board.IsWhiteTurn) & BitboardHelper.GetBitAt(move.Target)).IsEmpty) {
            score -= PieceValue[movedPiece];
        }

        return score;
    }
}