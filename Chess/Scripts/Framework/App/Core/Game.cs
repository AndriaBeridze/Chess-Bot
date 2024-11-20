namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;
using System.Runtime.CompilerServices;
using Chess.Bot;

class Game {
    private Player whitePlayer;
    private Player blackPlayer;

    private BoardUI boardUI;
    private CoordUI coordUI;
    private PositionUI positionUI;
    private PlayerUI playerUI;
    private GameStatus gameStatus;

    private Board board;
 
    private OpeningBook openingBook;

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

        gameStatus = new GameStatus("");

        openingBook = new OpeningBook();
    }

    public void Update() {
        string status = Arbiter.Status(board);
        if (status != "") {
            Color color = Color.White;

            if (status == "Checkmate") color = Theme.CheckmateTextColor;
            if (status == "Stalemate") color = Theme.StalemateTextColor;
            if (status == "Draw") color = Theme.DrawTextColor;

            gameStatus = new GameStatus(status, color);

            return;
        }

        // Displaying bot moves
        Player currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;
        if (currentPlayer.IsBot) {
            Move move = openingBook.GetMove(board.MovesMade);
            if (move.IsNull) move = currentPlayer.Search(board);

            positionUI.AnimateMove(move, board);
            board.MakeMove(move, record : true);
            boardUI.SetLastMove(move);
        }

        positionUI.Update(board, boardUI);
    }

    public void Render() {
        boardUI.Render();
        coordUI.Render();
        positionUI.Render();
        playerUI.Render();
        gameStatus.Render();
    }
}