namespace Chess.App;

using Chess.API;
using Chess.ChessEngine;
using Chess.Bot;
using Chess.Utility;
using Raylib_cs;

class Game {
    private Player whitePlayer;
    private Player blackPlayer;

    private TimerUI whiteTimerUI;
    private TimerUI blackTimerUI;

    private Board board;

    private BoardUI boardUI;
    private CoordUI coordUI;
    private PositionUI positionUI;
    private PlayerUI playerUI;
    private GameStatusUI gameStatusUI;
    private Buttons buttons;

    private OpeningBook openingBook;

    private Player currentPlayer;

    private bool statusCheck = true;
    private bool gameChanged = false;
    private bool gameOver = false;

    private Task BotTask;
    private Task AnimationTask;

    public Game(Player whitePlayer, Player blackPlayer, string fen, bool fromWhitesView) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;

        // Setting the view of the board, usually from the human player's perspective
        Settings.FromWhitesView = fromWhitesView;

        whiteTimerUI = new TimerUI(new Timer(Settings.TimeLimit), !fromWhitesView);
        blackTimerUI = new TimerUI(new Timer(Settings.TimeLimit), fromWhitesView);

        board = new Board(fen);

        if (board.IsWhiteTurn) blackTimerUI.Stop();
        else whiteTimerUI.Stop();

        boardUI = new BoardUI();
        coordUI = new CoordUI();
        positionUI = new PositionUI(board);
        playerUI = new PlayerUI(whitePlayer.PlayerType, blackPlayer.PlayerType);
        gameStatusUI = new GameStatusUI("");
        buttons = new Buttons();

        openingBook = new OpeningBook();

        currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;

        BotTask = Task.Run(() => { });
        AnimationTask = Task.Run(() => { });
    }

    public void Update() {
        // There was a bug with checking the status of the game when the bot was thinking, so it is not checked during the bot's turn
        if (!statusCheck) goto Update; 

        // If the game has ended, stop tendering and display the game status
        string status = Arbiter.Status(board, whiteTimerUI.Timer, blackTimerUI.Timer);
        if (status != "") {
            Color color = Color.White;

            if (status == "Checkmate") color = Theme.CheckmateTextColor;
            if (status == "Stalemate") color = Theme.StalemateTextColor;
            if (status == "Draw") color = Theme.DrawTextColor;
            
            gameStatusUI = new GameStatusUI(status, color);
            gameOver = true;

            goto HandleUI;
        }

        Update:

        // Displaying bot moves
        currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;
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

        positionUI.Update(board, boardUI, highlightMoves : statusCheck && !gameOver, ref whiteTimerUI, ref blackTimerUI);
        positionUI.AnimatePromotion(board); // There was an issue with changing the piece UI during a thread sleep, so it is checked separately
        
        whiteTimerUI.Update();
        blackTimerUI.Update();

        int buttonUpdate = buttons.Update();
        if (buttonUpdate != -1) {
            HandleButtonPress(buttonUpdate);
        }
    }

    private void GetBotMove() {
        Move move = openingBook.GetMove(board.MovesMade); // Check if there is a matching opening move
        if (move.IsNull) move = currentPlayer.Search(board); // If not, search for a move

        // Piece animation is also done in a separate thread
        // There is no need to do this, since animation is very short and doesn't affect the UI interface
        // But it is done to keep the code consistent
        AnimationTask = Task.Run(() => {
            AnimateMove(move);
        });
    }

    private void AnimateMove(Move move) {
        if (Arbiter.Status(board, whiteTimerUI.Timer, blackTimerUI.Timer) != "" || gameChanged) {
            statusCheck = true;
            gameChanged = false;
            return;
        }
        
        positionUI.AnimateMove(move, board);
        board.MakeMove(move, record : true); // Record the move since it is being recorded in the UI
        boardUI.SetLastMove(move);

        Thread.Sleep(100); // Short delay to make animations more pleasant
        if (board.IsWhiteTurn) {
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
                whitePlayer = new HumanPlayer(true);
                blackPlayer = new BotPlayer(false);
                Settings.FromWhitesView = true;
                break;
            case 1: // Play as Black
                whitePlayer = new BotPlayer(true);
                blackPlayer = new HumanPlayer(false);
                Settings.FromWhitesView = false;
                break;
            case 2: // AI vs AI
                whitePlayer = new BotPlayer(true);
                blackPlayer = new BotPlayer(false);
                Settings.FromWhitesView = true;
                break;
        }

        // Resetting the game
        whiteTimerUI = new TimerUI(new Timer(Settings.TimeLimit), !Settings.FromWhitesView);
        blackTimerUI = new TimerUI(new Timer(Settings.TimeLimit), Settings.FromWhitesView);

        board = new Board("");

        if (board.IsWhiteTurn) blackTimerUI.Stop();
        else whiteTimerUI.Stop();

        boardUI = new BoardUI();
        coordUI = new CoordUI();
        positionUI = new PositionUI(board);
        playerUI = new PlayerUI(whitePlayer.PlayerType, blackPlayer.PlayerType);
        gameStatusUI = new GameStatusUI("");
        buttons = new Buttons();

        openingBook = new OpeningBook();

        currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;

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