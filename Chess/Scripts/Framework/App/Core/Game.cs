namespace Chess.App;

using Chess.API;

class Game {
    public Player WhitePlayer;
    public Player BlackPlayer;

    public Game(Player whitePlayer, Player blackPlayer) {
        WhitePlayer = whitePlayer;
        BlackPlayer = blackPlayer;

        GameStats();
    }

    void GameStats() {
        ConsoleHelper.WriteColoredText("Game Stats:", ConsoleColor.Yellow);

        ConsoleHelper.WriteColoredText("Player 1: ", ConsoleColor.Blue, false);
        ConsoleHelper.WriteColoredText(WhitePlayer.ToString(), ConsoleColor.White);

        ConsoleHelper.WriteColoredText("Player 2: ", ConsoleColor.Red, false);
        ConsoleHelper.WriteColoredText(BlackPlayer.ToString(), ConsoleColor.White);
    }

    public void Render() {

    }
}