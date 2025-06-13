namespace Chess.Bot;

using System.Net;
using System.Runtime.CompilerServices;
using Chess.API;
using Chess.ChessEngine;

class Bot {
    const int positiveInfinity = 1000000000;
    const int negativeInfinity = -1000000000;
    const int checkmate = -1000000;

    static Dictionary<int, int> PieceValue = new Dictionary<int, int> {
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
    public static Move Think(Board board, double timeLeft) {
        overallBestMove = Move.NullMove;
        searchCancelled = false;
        timeLimitSeconds = GetThinkTime(board, timeLeft);

        var timerThread = new Thread(() => {
            Thread.Sleep((int)(timeLimitSeconds * 1000));
            searchCancelled = true;
        });
        timerThread.IsBackground = true;
        timerThread.Start();

        int depthSearched = 0, eval = 0;
        TranspositionTable.Clear();
        for (int depth = 1; depth <= int.MaxValue; depth++) {
            if (searchCancelled) break;
            currentBestMove = Move.NullMove;

            depthSearched = depth;
            eval = Search(board, depth);

            if (!searchCancelled) {
                overallBestMove = currentBestMove;
            }
        }

        return overallBestMove;
    }

    public static int Search(Board board, int depth, int alpha = negativeInfinity, int beta = positiveInfinity, bool firstCall = true) {
        if (searchCancelled) return 0; // Abort if time exceeded

        int hashF = HashFlag.ALPHA;

        int? val = TranspositionTable.ProbeHash(board, depth, alpha, beta);
        if (val.HasValue) {
            if (firstCall) {
                Move move = TranspositionTable.GetEntry(board.ZobristKey).move;
                if (!move.IsNull) {
                    currentBestMove = move;
                }
            }
            return val.Value;
        }

        if (depth == 0) return SearchCapture(board, alpha, beta);

        List<Move> moves = MoveGenerator.GenerateMoves(board); 
        Order(ref moves, board);
        if (moves.Count == 0) {
            if (MoveHelper.IsInCheck(board, board.IsWhiteTurn)) {
                return checkmate - depth;
            }
            return 0;
        }

        Move bestMoveThisPosition = Move.NullMove;
        foreach (Move move in moves) {
            board.MakeMove(move);
            int eval = -Search(board, depth - 1, -beta, -alpha, false);
            board.UnmakeMove(move);

            if (searchCancelled) return 0;

            if (eval >= beta) {
                TranspositionTable.RecordHash(board, depth, beta, HashFlag.BETA, move);
                return beta;
            }

            if (eval > alpha) {
                hashF = HashFlag.EXACT;
                alpha = eval;
                bestMoveThisPosition = move;

                if (firstCall) {
                    currentBestMove = move;
                }
            }
        }

        TranspositionTable.RecordHash(board, depth, alpha, hashF, bestMoveThisPosition);
        return alpha;
    }

    public static int SearchCapture(Board board, int alpha, int beta) {
        if (searchCancelled) return 0; // Abort if time exceeded

        int eval = Evaluation.Evaluate(board);
        if (eval >= beta) return beta;
        alpha = Math.Max(alpha, eval);

        List<Move> moves = MoveGenerator.GenerateCaptureMoves(board);
        Order(ref moves, board);

        foreach (Move move in moves) {
            board.MakeMove(move);
            eval = -SearchCapture(board, -beta, -alpha);
            board.UnmakeMove(move);

            if (searchCancelled) return 0; // Abort mid-search

            if (eval >= beta) return beta;
            alpha = Math.Max(alpha, eval);
        }

        return alpha;
    }

    public static void Order(ref List<Move> moves, Board board) {
        moves.Sort((a, b) => Score(b, board) - Score(a, board));
    }

    public static int Score(Move move, Board board) {
        int score = 0;
        int movedPiece = board.Square[move.Source].Type;
        int capturedPiece = board.Square[move.Target].Type;

        if (capturedPiece != Piece.None)
            score += 10 * PieceValue[capturedPiece] - PieceValue[movedPiece];

        if (move.IsPromotion)
            score += PieceValue[move.PromotingTo];

        if (MoveHelper.GetUnsafeSquares(board, board.IsWhiteTurn).Contains(move.Target))
            score -= PieceValue[movedPiece];

        return score;
    }

    public static double GetThinkTime(Board board, double timeLeft) {
        // Safety bounds
        const double minTime = 0.05;  // Don't go below this per move
        const double maxTime = 3.0;   // Never spend more than this per move

        int pieceCount = 0;
        foreach (var square in board.Square) {
            if (square.Type != Piece.None) pieceCount++;
        }

        // Estimate phase of the game
        bool isOpening = pieceCount > 24;
        bool isEndgame = pieceCount < 12;

        // Estimate moves remaining
        int estimatedMovesRemaining = isEndgame ? 20 : (isOpening ? 40 : 30);

        // Divide time budget with some flexibility
        double baseTime = timeLeft / estimatedMovesRemaining;

        // Allow more time per move if a lot of time is left
        double flexibilityMultiplier = timeLeft > 60 ? 1.5 : (timeLeft < 10 ? 0.5 : 1.0);

        double thinkTime = baseTime * flexibilityMultiplier;

        // Clamp it within safe bounds=
        thinkTime = Math.Clamp(thinkTime / 10, minTime, maxTime);

        return thinkTime;
    }
}
