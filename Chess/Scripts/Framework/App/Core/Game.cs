namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;
using System.Runtime.CompilerServices;

class Game {
    private Player whitePlayer;
    private Player blackPlayer;

    public BoardUI boardUI;
    public CoordUI coordUI;
    public PositionUI positionUI;
    public PlayerUI playerUI;
    public GameStatus gameStatus;

    public Board board;

    private bool isBotCheckAllowed = true;

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
    }

    public async void Update() {
        if (Arbiter.IsCheckmate(board)) {
            gameStatus.Text = "Checkmate";
            gameStatus.Color = Theme.CheckmateTextColor;
            return; 
        } else if (Arbiter.IsStalemate(board)) {
            gameStatus.Text = "Stalemate";
            gameStatus.Color = Theme.StalemateTextColor;
            return;
        } else if (Arbiter.IsDraw(board)) {
            gameStatus.Text = "Draw";
            gameStatus.Color = Theme.DrawTextColor;
            return;
        }

        positionUI.Update(board, boardUI);

        // Displaying bot moves
        Player currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;
        if (currentPlayer.IsBot && isBotCheckAllowed) {
            isBotCheckAllowed = false;
            Move bestMove = currentPlayer.Search(board);

            boardUI.SetLastMove(bestMove);
            await positionUI.AnimateMove(bestMove, board).ContinueWith(_ => {
                isBotCheckAllowed = true;

                board.MakeMove(bestMove);
            });
        }
    }

    public void Render() {
        boardUI.Render();
        coordUI.Render();
        positionUI.Render();
        playerUI.Render();
        gameStatus.Render();
    }
}