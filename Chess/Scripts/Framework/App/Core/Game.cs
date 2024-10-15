namespace Chess.App;

class Game {
    public Player WhitePlayer;
    public Player BlackPlayer;

    public Game(Player whitePlayer, Player blackPlayer) {
        WhitePlayer = whitePlayer;
        BlackPlayer = blackPlayer;

        GameStats();
    }

    void GameStats() {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Game Stats:");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write($"Player 1: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(WhitePlayer);
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"Player 2: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(BlackPlayer);
        Console.ResetColor();
    }
}