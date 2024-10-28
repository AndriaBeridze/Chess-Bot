namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;

class Game {
    private Player whitePlayer;
    private Player blackPlayer;

    private BoardUI boardUI;
    private CoordUI coordUI;
    private PositionUI positionUI;
    private PlayerUI playerUI;

    private Board board;

    public Game(Player whitePlayer, Player blackPlayer, string fen, bool isWhitePerspective) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;

        Theme.IsWhitePerspective = isWhitePerspective;

        board = new Board(fen);

        boardUI = new BoardUI();
        coordUI = new CoordUI();
        positionUI = new PositionUI(board);
        playerUI = new PlayerUI(whitePlayer.PlayerType, blackPlayer.PlayerType);

        GameStats();
    }

    void GameStats() {
        UIHelper.WriteColoredText("Game Stats:", ConsoleColor.Yellow);

        UIHelper.WriteColoredText("Player 1: ", ConsoleColor.Blue, false);
        UIHelper.WriteColoredText(whitePlayer.ToString(), ConsoleColor.White);

        UIHelper.WriteColoredText("Player 2: ", ConsoleColor.Red, false);
        UIHelper.WriteColoredText(blackPlayer.ToString(), ConsoleColor.White);
    }

    public void Render() {
        boardUI.Render();
        coordUI.Render();
        positionUI.Render();
        playerUI.Render();
    }
}