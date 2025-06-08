namespace Chess.Core;

using Chess.UI;
using Chess.API;
using Chess.Bot;
using Chess.Utility;
using Raylib_cs;

class Game {
    private App.Player whitePlayer;
    private App.Player blackPlayer;

    private ChessEngine.Board chessBoard;

    private Timer whiteTimer;
    private Timer blackTimer;

    private Board board;
    private Coords coord;
    private Position position;
    private Player player;
    private Status gameStatus;
    private Menu buttons;

    private OpeningBook openingBook;

    private App.Player currentPlayer;

    private bool statusCheck = true;
    private bool gameChanged = false;
    private bool gameOver = false;

    private Task BotTask;
    private Task AnimationTask;

    private CancellationTokenSource botTokenSource;
    private CancellationTokenSource animationTokenSource;

    public Game(App.Player whitePlayer, App.Player blackPlayer, string fen, bool fromWhitesView) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;

        Settings.FromWhitesView = fromWhitesView;

        whiteTimer = new Timer(Settings.TimeLimit, !fromWhitesView);
        blackTimer = new Timer(Settings.TimeLimit, fromWhitesView);

        chessBoard = new ChessEngine.Board(fen);

        if (chessBoard.IsWhiteTurn) blackTimer.Stop();
        else whiteTimer.Stop();

        board = new Board();
        coord = new Coords();
        position = new Position(chessBoard);
        player = new Player(whitePlayer.PlayerType, blackPlayer.PlayerType);
        gameStatus = new Status("");
        buttons = new Menu();

        openingBook = new OpeningBook();

        currentPlayer = chessBoard.IsWhiteTurn ? whitePlayer : blackPlayer;

        botTokenSource = new CancellationTokenSource();
        animationTokenSource = new CancellationTokenSource();

        BotTask = Task.CompletedTask;
        AnimationTask = Task.CompletedTask;
    }

    public void Update() {
        if (!statusCheck) goto Update;

        string status = Arbiter.Status(chessBoard, whiteTimer.Time, blackTimer.Time);
        if (status != "") {
            Color color = Color.White;

            if (status == "Checkmate") color = Theme.CheckmateTextColor;
            if (status == "Stalemate") color = Theme.StalemateTextColor;
            if (status == "Draw") color = Theme.DrawTextColor;

            gameStatus = new Status(status, color);
            gameOver = true;

            goto Handle;
        }

        Update:

        currentPlayer = chessBoard.IsWhiteTurn ? whitePlayer : blackPlayer;
        if (currentPlayer.IsBot && statusCheck) {
            statusCheck = false;
            botTokenSource = new CancellationTokenSource();
            var token = botTokenSource.Token;
            BotTask = Task.Run(() => GetBotMove(token), token);
        }

        Handle:

        if (gameOver) {
            whiteTimer.Stop();
            blackTimer.Stop();
        }

        position.Update(chessBoard, board, highlightMoves: statusCheck && !gameOver, ref whiteTimer, ref blackTimer);
        position.AnimatePromotion(chessBoard);

        whiteTimer.Update();
        blackTimer.Update();

        int buttonUpdate = buttons.Update();
        if (buttonUpdate != -1) {
            HandleButtonPress(buttonUpdate);
        }
    }

    private void GetBotMove(CancellationToken token) {
        try {
            if (token.IsCancellationRequested) return;

            Move move = openingBook.GetMove(chessBoard.MovesMade);
            if (move.IsNull) move = currentPlayer.Search(chessBoard); // You can pass the token here if supported

            animationTokenSource = new CancellationTokenSource();
            var animToken = animationTokenSource.Token;
            AnimationTask = Task.Run(() => AnimateMove(move, animToken), animToken);
        } catch (OperationCanceledException) {
            // Canceled
        }
    }

    private void AnimateMove(Move move, CancellationToken token) {
        if (token.IsCancellationRequested) return;

        if (Arbiter.Status(chessBoard, whiteTimer.Time, blackTimer.Time) != "" || gameChanged) {
            statusCheck = true;
            gameChanged = false;
            return;
        }

        position.AnimateMove(move, chessBoard);
        if (token.IsCancellationRequested) return;

        chessBoard.MakeMove(move, record: true);
        board.SetLastMove(move);

        Thread.Sleep(100);
        if (token.IsCancellationRequested) return;

        if (chessBoard.IsWhiteTurn) {
            whiteTimer.Start();
            blackTimer.Stop();
        } else {
            blackTimer.Start();
            whiteTimer.Stop();
        }

        statusCheck = true;
    }

    public void HandleButtonPress(int buttonUpdate) {
        // Cancel any running bot/animation tasks
        botTokenSource.Cancel();
        animationTokenSource.Cancel();

        try {
            Task.WaitAll(new[] { BotTask, AnimationTask }, 200); // Wait briefly (optional)
        } catch (AggregateException) {
            // Ignore task cancellations
        }

        // Reinitialize players based on button selection
        switch (buttonUpdate) {
            case 0:
                whitePlayer = new App.HumanPlayer(true);
                blackPlayer = new App.BotPlayer(false);
                Settings.FromWhitesView = true;
                break;
            case 1:
                whitePlayer = new App.BotPlayer(true);
                blackPlayer = new App.HumanPlayer(false);
                Settings.FromWhitesView = false;
                break;
            case 2:
                whitePlayer = new App.BotPlayer(true);
                blackPlayer = new App.BotPlayer(false);
                Settings.FromWhitesView = true;
                break;
        }

        // Reset game state
        whiteTimer = new Timer(Settings.TimeLimit, !Settings.FromWhitesView);
        blackTimer = new Timer(Settings.TimeLimit, Settings.FromWhitesView);

        chessBoard = new ChessEngine.Board("");

        if (chessBoard.IsWhiteTurn) blackTimer.Stop();
        else whiteTimer.Stop();

        board = new Board();
        coord = new Coords();
        position = new Position(chessBoard);
        player = new Player(whitePlayer.PlayerType, blackPlayer.PlayerType);
        gameStatus = new Status("");
        buttons = new Menu();

        openingBook = new OpeningBook();

        currentPlayer = chessBoard.IsWhiteTurn ? whitePlayer : blackPlayer;

        botTokenSource = new CancellationTokenSource();
        animationTokenSource = new CancellationTokenSource();

        statusCheck = true;
        gameChanged = true;
        gameOver = false;
    }

    public void Render() {
        buttons.Render();
        board.Render();
        coord.Render();
        position.Render();
        player.Render();
        gameStatus.Render();
        whiteTimer.Render();
        blackTimer.Render();
    }
}
