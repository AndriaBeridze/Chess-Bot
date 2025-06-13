namespace Chess.Bot;

using System.Net;
using System.Runtime.CompilerServices;
using Chess.API;
using Chess.ChessEngine;

class Bot {
    const int positiveInfinity = 1000000000;
    const int negativeInfinity = -1000000000;
    const int checkmate = -1000000;

    // Piece values used for move ordering (not evaluation)
    static readonly Dictionary<int, int> PieceValue = new() {
        { Piece.Pawn, Evaluation.Pawn },
        { Piece.Knight, Evaluation.Knight },
        { Piece.Bishop, Evaluation.Bishop },
        { Piece.Rook, Evaluation.Rook },
        { Piece.Queen, Evaluation.Queen },
        { Piece.King, positiveInfinity }
    };

    static double timeLimitSeconds;
    static bool searchCancelled = false;

    public static Move currentBestMove = Move.NullMove;
    public static Move overallBestMove = Move.NullMove;

    // Entry point: searches for best move within a time budget
    public static Move Think(Board board, double timeLeft) {
        overallBestMove = Move.NullMove;
        searchCancelled = false;
        timeLimitSeconds = GetThinkTime(board, timeLeft);

        // Starts a background thread that cancels the search after time limit
        var timerThread = new Thread(() => {
            Thread.Sleep((int)(timeLimitSeconds * 1000));
            searchCancelled = true;
        }) {
            IsBackground = true
        };
        timerThread.Start();

        int depthSearched = 0, eval = 0;
        TranspositionTable.Clear();

        // Iterative deepening loop
        for (int depth = 1; depth <= int.MaxValue; depth++) {
            if (searchCancelled) break;

            currentBestMove = Move.NullMove;
            depthSearched = depth;
            eval = Search(board, depth);

            if (!currentBestMove.IsNull) {
                overallBestMove = currentBestMove;
            }
        }

        return overallBestMove;
    }

    // Main minimax search with alpha-beta pruning
    public static int Search(Board board, int depth, int alpha = negativeInfinity, int beta = positiveInfinity, bool firstCall = true) {
        if (searchCancelled) return 0;

        int hashF = HashFlag.ALPHA;

        // Probe transposition table for prior result
        int? val = TranspositionTable.ProbeHash(board, depth, alpha, beta);
        if (val.HasValue) {
            if (firstCall) {
                Move move = TranspositionTable.GetEntry(board.ZobristKey).move;
                if (!move.IsNull) currentBestMove = move;
            }
            return val.Value;
        }

        if (depth == 0) return QuiescenceSearch(board, alpha, beta); // Quiescence search

        List<Move> moves = MoveGenerator.GenerateMoves(board);
        Order(ref moves, board); // Improve pruning efficiency

        if (moves.Count == 0) {
            if (MoveHelper.IsInCheck(board, board.IsWhiteTurn)) {
                return checkmate - depth; // Mate found
            }
            return 0; // Stalemate
        }

        Move bestMoveThisPosition = Move.NullMove;

        foreach (Move move in moves) {
            board.MakeMove(move);
            int eval = -Search(board, depth - 1, -beta, -alpha, false);
            board.UnmakeMove(move);

            if (searchCancelled) return 0;

            if (eval >= beta) {
                // Move too good, opponent will not allow it
                TranspositionTable.RecordHash(board, depth, beta, HashFlag.BETA, move);
                return beta;
            }

            if (eval > alpha) {
                // Best score so far
                hashF = HashFlag.EXACT;
                alpha = eval;
                bestMoveThisPosition = move;

                if (firstCall) currentBestMove = move;
            }
        }

        TranspositionTable.RecordHash(board, depth, alpha, hashF, bestMoveThisPosition);
        return alpha;
    }

    // Quiescence search to avoid horizon effect on captures
    public static int QuiescenceSearch(Board board, int alpha, int beta) {
        if (searchCancelled) return 0;

        int eval = Evaluation.Evaluate(board);
        if (eval >= beta) return beta;
        alpha = Math.Max(alpha, eval);

        List<Move> moves = MoveGenerator.GenerateCaptureMoves(board);
        Order(ref moves, board);

        foreach (Move move in moves) {
            board.MakeMove(move);
            eval = -QuiescenceSearch(board, -beta, -alpha);
            board.UnmakeMove(move);

            if (searchCancelled) return 0;
            if (eval >= beta) return beta;

            alpha = Math.Max(alpha, eval);
        }

        return alpha;
    }

    // Orders moves to improve alpha-beta efficiency
    public static void Order(ref List<Move> moves, Board board) {
        moves.Sort((a, b) => Score(b, board) - Score(a, board));
    }

    // Heuristic move scoring for ordering
    public static int Score(Move move, Board board) {
        if (move == overallBestMove) return positiveInfinity; // Prioritize best move found in previous iterations

        int score = 0;

        int movedPiece = board.Square[move.Source].Type;
        int capturedPiece = board.Square[move.Target].Type;

        // MVV-LVA: prioritize high-value captures with low-value attackers
        if (capturedPiece != Piece.None)
            score += 10 * PieceValue[capturedPiece] - PieceValue[movedPiece];

        // Promote earlier
        if (move.IsPromotion)
            score += PieceValue[move.PromotingTo];

        // Discourage moving into danger
        if (MoveHelper.GetUnsafeSquares(board, board.IsWhiteTurn).Contains(move.Target))
            score -= PieceValue[movedPiece];

        return score;
    }

    // Dynamic time allocation based on game phase and time left
    public static double GetThinkTime(Board board, double timeLeft) {
        const double minTime = 0.05;  // Minimum think time per move
        const double maxTime = 3.0;   // Maximum think time per move

        int pieceCount = 0;
        foreach (var square in board.Square) {
            if (square.Type != Piece.None) pieceCount++;
        }

        bool isOpening = pieceCount > 24;
        bool isEndgame = pieceCount < 12;
        int estimatedMovesRemaining = isEndgame ? 20 : (isOpening ? 40 : 30);

        double baseTime = timeLeft / estimatedMovesRemaining;

        double flexibilityMultiplier = timeLeft > 60 ? 1.5 : (timeLeft < 10 ? 0.5 : 1.0);

        double thinkTime = baseTime * flexibilityMultiplier;

        // Divide by 10 to preserve a buffer for future moves
        thinkTime = Math.Clamp(thinkTime / 10, minTime, maxTime);

        return thinkTime;
    }
}
