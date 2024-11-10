namespace Chess.Testing;

using Chess.ChessEngine;
using Chess.API;

class Tester {
    private static string[] fenTests = [
        "RNBQKBNR/PPPPPPPP/8/8/8/8/pppppppp/rnbqkbnr w KQkq - 0 1",
        "RNBQK2R/PPP1NnPP/8/2B5/8/2p5/pp1Pbppp/rnbq1k1r w KQ - 1 8"
    ];

    private static int[][] numPositions = [
        [20, 400, 8902, 197281, 4865609],
        [44, 1486, 62379, 2103487]
    ];

    public static void Test() {
        Console.ForegroundColor = ConsoleColor.Gray;
        for (int i = 0; i < fenTests.Length; i++) {
            PrintColoredText($"Position: ", ConsoleColor.White);
            PrintColoredText($"{ fenTests[i] }\n", ConsoleColor.Blue);
            PrintColoredText($"Testing...\n\n", ConsoleColor.Yellow);

            int[][] count = TestPosition(fenTests[i], numPositions[i].Length);

            Console.WriteLine(" Depth | Position Count | Expected Count | Time Elapsed (ms) | Passed/Failed ");
            Console.WriteLine("-------+----------------+----------------+-------------------+---------------");
            for (int j = 0; j < numPositions[i].Length; j++) {
                PrintInteger(j + 1, 5, ConsoleColor.Gray);
                PrintInteger(count[j][0], 14, ConsoleColor.Blue);
                PrintInteger(numPositions[i][j], 14, ConsoleColor.Blue);
                PrintInteger(count[j][1], 17, ConsoleColor.Yellow);
                
                if (count[j][0] == numPositions[i][j]) {
                    PrintColoredText("        Passed", ConsoleColor.Green);
                } else {
                    PrintColoredText("        Failed", ConsoleColor.Red);
                }

                if (j == numPositions[i].Length - 1){
                    Console.WriteLine("\n");
                } else {
                    Console.WriteLine("\n-------+----------------+----------------+-------------------+---------------");
                }
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.White;
    }


    public static int[][] TestPosition(string fen, int maxDepth) {
        Board board = new Board(fen);   

        int[][] ans = new int[maxDepth][];
        for (int i = 0; i < maxDepth; i++) {
            ans[i] = new int[2];
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            ans[i][0] = CountNumPosition(board, i + 1);
            stopwatch.Stop();
            ans[i][1] = (int) stopwatch.ElapsedMilliseconds;
        }

        return ans;
    }

    private static int CountNumPosition(Board board, int depth) {
        if (depth == 0) return 1;

        List<Move> moves = MoveGenerator.LegalMoves(board);
        int sum = 0;

        foreach(Move move in moves) {
            board.MakeMove(move);

            sum += CountNumPosition(board, depth - 1);
            
            board.UnmakeMove(move);
        }

        return sum;
    }

    private static int CountDigits(int num) {
        if (num == 0) return 1;

        int ans = 0;
        while(num != 0) {
            ans++;
            num /= 10;
        }

        return ans;
    }

    private static void PrintInteger(int num, int spaceCount, ConsoleColor color) {
        int numDigits = CountDigits(num);
        
        Console.Write(" ");
        for (int i = 0; i < spaceCount - numDigits; i++) {
            Console.Write(" ");
        }
        PrintColoredText($"{ num }", color);
        Console.Write(" |");
    }

    private static void PrintColoredText(string text, ConsoleColor color) {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}