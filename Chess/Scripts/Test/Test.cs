namespace Chess.Test;

using Chess.ChessEngine;
using Chess.API;

class Test {
    public static void TestPosition(string fen) {
        Board board = new Board(fen);

        for (int i = 1; i <= 10; i++) {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int numPositions = CheckNumPosition(board, i);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine($"Depth {i}: {numPositions} positions, Time: {elapsedMs} seconds");
        }
    }

    private static int CheckNumPosition(Board board, int depth) {
        if (depth == 0) return 1;

        List<Move> moves = MoveGenerator.LegalMoves(board);
        // Console.WriteLine(moves.Count);
        int sum = 0;

        foreach(Move move in moves) {
            board.MakeMove(move);

            sum += CheckNumPosition(board, depth - 1);
            
            board.UnmakeMove(move);
        }

        return sum;
    }
}