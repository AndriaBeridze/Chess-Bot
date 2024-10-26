namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;

class Game {
    public Player WhitePlayer;
    public Player BlackPlayer;
    
    public BoardUI BoardUI;
    public CoordUI CoordUI;
    public PlayerUI PlayerUI;

    public Board Board;

    public Game(Player whitePlayer, Player blackPlayer, string fen, bool isWhitePerspective) {
        WhitePlayer = whitePlayer;
        BlackPlayer = blackPlayer;

        Theme.IsWhitePerspective = isWhitePerspective;

        BoardUI = new BoardUI();
        CoordUI = new CoordUI();
        PlayerUI = new PlayerUI(whitePlayer.PlayerType, blackPlayer.PlayerType);

        Board = new Board(fen);

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
        PlayerUI.Render();
    }
}