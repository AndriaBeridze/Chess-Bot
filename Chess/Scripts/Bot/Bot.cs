namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

class Bot {
    const int positiveInfinity = 1000000000;
    const int negativeInfinity = -1000000000;
    const int checkmate = -1000000;
    static Move bestMove = Move.NullMove;

    static Dictionary<int, int> PieceValue = new Dictionary<int, int> {
        { Piece.Pawn, Evaluation.Pawn },
        { Piece.Knight, Evaluation.Knight },
        { Piece.Bishop, Evaluation.Bishop },
        { Piece.Rook, Evaluation.Rook },
        { Piece.Queen, Evaluation.Queen },
        { Piece.King, 1000000000 }
    };

    public static Move Think(Board board) {
        DateTime time = DateTime.Now;
        for (int i = 1; i <= 100; i++) {
            bestMove = Move.NullMove; // Reset the best move for each depth
            Search(board, i);
            if ((DateTime.Now - time).TotalSeconds > 1) break; // Limit the thinking time to 1 seconds
        }
        return bestMove;
    }

    // The search function is a minimax algorithm with alpha-beta pruning
    // It searches for the best move by recursively searching through the possible moves
    // The depth parameter controls the depth of the search
    // The alpha and beta parameters are used for pruning
    // The firstCall parameter is used to store the best move
    // Minimax algorithm: https://en.wikipedia.org/wiki/Minimax
    // Alpha-beta pruning: https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
    public static int Search(Board board, int depth, int alpha = negativeInfinity, int beta = positiveInfinity, bool firstCall = true) {
        if (depth == 0) return SearchCapture(board, alpha, beta);

        List<Move> moves = MoveGenerator.GenerateMoves(board); 
        Order(ref moves, board); // Order the moves to predict the best moves first
        if (moves.Count == 0) {
            // No moves can be made, either checkmate or stalemate
            if (MoveHelper.IsInCheck(board, board.IsWhiteTurn)) return checkmate - depth; // Checkmate
            return 0; // Stalemate
        }

        foreach (Move move in moves) {
            board.MakeMove(move);
            int eval = -Search(board, depth - 1, -beta, -alpha, false);
            board.UnmakeMove(move);

            if (eval >= beta) return beta; // Prune the branch, opponent will not allow this move
            if (alpha < eval) {
                alpha = eval;
                if (firstCall) {
                    // Update the best move if it's the first call
                    bestMove = move;
                }
            }
        }

        return alpha;
    }

    // After the depth is reached, the search function will call the SearchCapture function
    // The SearchCapture evaluates every possible position where a capture can be made
    // It is used to better judge the position, because evaluation function does not take captures into account
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

    // Orders the moves based on prediction score
    public static void Order(ref List<Move> moves, Board board) {
        moves.Sort((a, b) => {
            return Score(b, board) - Score(a, board);
        });
    }

    // Calculate the score of the move
    // This is used to predict the best moves, so the bot can make faster decisions
    public static int Score(Move move, Board board) {
        int score = 0;
        int movedPiece = board.Square[move.Source].Type;
        int capturedPiece = board.Square[move.Target].Type;

        // The better the piece, the higher the score
        if (capturedPiece != Piece.None) score += 10 * PieceValue[capturedPiece] - PieceValue[movedPiece];

        // If the move is a promotion, add the value of the promoted piece
        if (move.IsPromotion) score += PieceValue[move.PromotingTo];

        // If piece is under attack, subtract the value of the piece
        if (MoveHelper.GetUnsafeSquares(board, board.IsWhiteTurn).Contains(move.Target)) score -= PieceValue[movedPiece];

        return score;
    }
}