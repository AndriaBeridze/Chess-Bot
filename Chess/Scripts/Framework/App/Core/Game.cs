namespace Chess.App;

using Chess.API;

class Game {
    public Player WhitePlayer;
    public Player BlackPlayer;
    
    public BoardUI BoardUI;
    public CoordUI CoordUI;

    public Game(Player whitePlayer, Player blackPlayer, bool isWhitePerspective) {
        WhitePlayer = whitePlayer;
        BlackPlayer = blackPlayer;

        Theme.IsWhitePerspective = isWhitePerspective;

        BoardUI = new BoardUI();
        CoordUI = new CoordUI();
        
        GameStats();
    }

    void GameStats() {
        UIHelper.WriteColoredText("Game Stats:", ConsoleColor.Yellow);

        UIHelper.WriteColoredText("Player 1: ", ConsoleColor.Blue, false);
        UIHelper.WriteColoredText(WhitePlayer.ToString(), ConsoleColor.White);

        UIHelper.WriteColoredText("Player 2: ", ConsoleColor.Red, false);
        UIHelper.WriteColoredText(BlackPlayer.ToString(), ConsoleColor.White);
    }

    public void Render() {
        BoardUI.Render();
        CoordUI.Render();
    }
}