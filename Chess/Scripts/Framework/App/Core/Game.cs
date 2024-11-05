namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;

class Game {
    private Player whitePlayer;
    private Player blackPlayer;

    public BoardUI boardUI;
    public CoordUI coordUI;
    public PositionUI positionUI;
    public PlayerUI playerUI;

    public Board board;

    public Game(Player whitePlayer, Player blackPlayer, string fen, bool isWhitePerspective) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;

        // Does board need to be rotated to show it from black's perspective?
        Theme.IsWhitePerspective = isWhitePerspective;

        board = new Board(fen);

        boardUI = new BoardUI();
        coordUI = new CoordUI();
        positionUI = new PositionUI(board);
        playerUI = new PlayerUI(whitePlayer.PlayerType, blackPlayer.PlayerType);
    }

    public void Render() {
        boardUI.Render();
        coordUI.Render();
        positionUI.Render();
        playerUI.Render();
    }

    public void Update() {
        Player currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;
        if (currentPlayer.IsBot) {
            
        }
        positionUI.Update(board, boardUI);
    }
}