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

    private TimerUI whiteTimerUI;
    private TimerUI blackTimerUI;

    private Board boardUI;
    private Coords coordUI;
    private Position positionUI;
    private Player playerUI;
    private Status gameStatusUI;
    private Menu buttons;

    private OpeningBook openingBook;

    private App.Player currentPlayer;

    private bool statusCheck = true;
    private bool gameChanged = false;
    private bool gameOver = false;

    private Task BotTask;
    private Task AnimationTask;

    public Game(App.Player whitePlayer, App.Player blackPlayer, string fen, bool fromWhitesView) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;

        // Setting the view of the board, usually from the human player's perspective
        Settings.FromWhitesView = fromWhitesView;

        whiteTimerUI = new TimerUI(Settings.TimeLimit, !fromWhitesView);
        blackTimerUI = new TimerUI(Settings.TimeLimit, fromWhitesView);

        chessBoard = new ChessEngine.Board(fen);

        if (chessBoard.IsWhiteTurn) blackTimerUI.Stop();
        else whiteTimerUI.Stop();

        boardUI = new Board();
        coordUI = new Coords();
        positionUI = new Position(chessBoard);
        playerUI = new Player(whitePlayer.PlayerType, blackPlayer.PlayerType);
        gameStatusUI = new Status("");
        buttons = new Menu();

        openingBook = new OpeningBook();

        currentPlayer = chessBoard.IsWhiteTurn ? whitePlayer : blackPlayer;

        BotTask = Task.Run(() => { });
        AnimationTask = Task.Run(() => { });
    }

    public void Update() {
        // There was a bug with checking the status of the game when the bot was thinking, so it is not checked during the bot's turn
        if (!statusCheck) goto Update; 

        // If the game has ended, stop tendering and display the game status
        string status = Arbiter.Status(chessBoard, whiteTimerUI.Time, blackTimerUI.Time);
        if (status != "") {
            Color color = Color.White;

            if (status == "Checkmate") color = Theme.CheckmateTextColor;
            if (status == "Stalemate") color = Theme.StalemateTextColor;
            if (status == "Draw") color = Theme.DrawTextColor;
            
            gameStatusUI = new Status(status, color);
            gameOver = true;

            goto HandleUI;
        }

        Update:

        // Displaying bot moves
        currentPlayer = chessBoard.IsWhiteTurn ? whitePlayer : blackPlayer;
        if (currentPlayer.IsBot && statusCheck) {
            statusCheck = false;
            // Running the bot move in a separate thread, so the game doesn't freeze, and the user can interact with the UI
            BotTask = Task.Run(GetBotMove);
        }

        HandleUI:

        if (gameOver) {
            whiteTimerUI.Stop();
            blackTimerUI.Stop();
        }

        positionUI.Update(chessBoard, boardUI, highlightMoves : statusCheck && !gameOver, ref whiteTimerUI, ref blackTimerUI);
        positionUI.AnimatePromotion(chessBoard); // There was an issue with changing the piece UI during a thread sleep, so it is checked separately
        
        whiteTimerUI.Update();
        blackTimerUI.Update();

        int buttonUpdate = buttons.Update();
        if (buttonUpdate != -1) {
            HandleButtonPress(buttonUpdate);
        }
    }

    private void GetBotMove() {
        Move move = openingBook.GetMove(chessBoard.MovesMade); // Check if there is a matching opening move
        if (move.IsNull) move = currentPlayer.Search(chessBoard); // If not, search for a move

        // Piece animation is also done in a separate thread
        // There is no need to do this, since animation is very short and doesn't affect the UI interface
        // But it is done to keep the code consistent
        AnimationTask = Task.Run(() => {
            AnimateMove(move);
        });
    }

    private void AnimateMove(Move move) {
        if (Arbiter.Status(chessBoard, whiteTimerUI.Time, blackTimerUI.Time) != "" || gameChanged) {
            statusCheck = true;
            gameChanged = false;
            return;
        }
        
        positionUI.AnimateMove(move, chessBoard);
        chessBoard.MakeMove(move, record : true); // Record the move since it is being recorded in the UI
        boardUI.SetLastMove(move);

        Thread.Sleep(100); // Short delay to make animations more pleasant
        if (chessBoard.IsWhiteTurn) {
            whiteTimerUI.Start();
            blackTimerUI.Stop();
        } else {
            blackTimerUI.Start();
            whiteTimerUI.Stop();
        }

        statusCheck = true;
    }

    public void HandleButtonPress(int buttonUpdate) {
        if (!BotTask.IsCompleted) BotTask.Wait();
        if (!AnimationTask.IsCompleted) AnimationTask.Wait();
        
        switch (buttonUpdate) {
            case 0: // Play as White
                whitePlayer = new App.HumanPlayer(true);
                blackPlayer = new App.BotPlayer(false);
                Settings.FromWhitesView = true;
                break;
            case 1: // Play as Black
                whitePlayer = new App.BotPlayer(true);
                blackPlayer = new App.HumanPlayer(false);
                Settings.FromWhitesView = false;
                break;
            case 2: // AI vs AI
                whitePlayer = new App.BotPlayer(true);
                blackPlayer = new App.BotPlayer(false);
                Settings.FromWhitesView = true;
                break;
        }

        // Resetting the game
        whiteTimerUI = new TimerUI(Settings.TimeLimit, !Settings.FromWhitesView);
        blackTimerUI = new TimerUI(Settings.TimeLimit, Settings.FromWhitesView);

        chessBoard = new ChessEngine.Board("");

        if (chessBoard.IsWhiteTurn) blackTimerUI.Stop();
        else whiteTimerUI.Stop();

        boardUI = new Board();
        coordUI = new Coords();
        positionUI = new Position(chessBoard);
        playerUI = new Player(whitePlayer.PlayerType, blackPlayer.PlayerType);
        gameStatusUI = new Status("");
        buttons = new Menu();

        openingBook = new OpeningBook();

        currentPlayer = chessBoard.IsWhiteTurn ? whitePlayer : blackPlayer;

        statusCheck = true;
        gameChanged = true;
        gameOver = false;
    }

    public void Render() {
        buttons.Render();
        boardUI.Render();
        coordUI.Render();
        positionUI.Render();
        playerUI.Render();
        gameStatusUI.Render();
        whiteTimerUI.Render();
        blackTimerUI.Render();
    }
}