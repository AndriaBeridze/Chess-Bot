namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;
using Chess.Bot;

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

    private OpeningBook openingBook;

    private Player currentPlayer;
    private bool statusCheck = true;

    public Game(Player whitePlayer, Player blackPlayer, string fen, bool fromWhitesView) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;

        // Setting the view of the board, usually from the human player's perspective
        Theme.FromWhitesView = fromWhitesView;

        whiteTimerUI = new TimerUI(new Timer(Theme.TimeLimit), !fromWhitesView);
        blackTimerUI = new TimerUI(new Timer(Theme.TimeLimit), fromWhitesView);

        board = new Board(fen);

        if (board.IsWhiteTurn) blackTimerUI.Stop();
        else whiteTimerUI.Stop();

        boardUI = new BoardUI();
        coordUI = new CoordUI();
        positionUI = new PositionUI(board);
        playerUI = new PlayerUI(whitePlayer.PlayerType, blackPlayer.PlayerType);
        gameStatusUI = new GameStatusUI("");

        openingBook = new OpeningBook();

        currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;
    }

    public void Update() {
        // There was a bug with checking the status of the game when the bot was thinking, so it is not checked during the bot's turn
        if (!statusCheck) goto Update; 

        string status = Arbiter.Status(board, whiteTimerUI.Timer, blackTimerUI.Timer);
        if (status != "") {
            Color color = Color.White;

            if (status == "Checkmate") color = Theme.CheckmateTextColor;
            if (status == "Stalemate") color = Theme.StalemateTextColor;
            if (status == "Draw") color = Theme.DrawTextColor;
            
            gameStatusUI = new GameStatusUI(status, color);

            return;
        }

        Update:

        // Displaying bot moves
        currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;
        if (currentPlayer.IsBot && statusCheck) {
            statusCheck = false;
            // Running the bot move in a separate thread, so the game doesn't freeze, and the user can interact with the UI
            Task.Run(GetBotMove);
        }

        positionUI.Update(board, boardUI, highlightMoves : statusCheck, ref whiteTimerUI, ref blackTimerUI);
        positionUI.AnimatePromotion(board); // There was an issue with changing the piece UI during a thread sleep, so it is checked separately
        
        whiteTimerUI.Update();
        blackTimerUI.Update();
    }

    private void GetBotMove() {
        Move move = openingBook.GetMove(board.MovesMade); // Check if there is a matching opening move
        if (move.IsNull) move = currentPlayer.Search(board); // If not, search for a move

        // Piece animation is also done in a separate thread
        // There is no need to do this, since animation is very short and doesn't affect the UI interface
        // But it is done to keep the code consistent
        Task.Run(() => {
            AnimateMove(move);
        });
    }

    private void AnimateMove(Move move) {
        if (Arbiter.Status(board, whiteTimerUI.Timer, blackTimerUI.Timer) != "") {
            statusCheck = true;
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

    public void Render() {
        boardUI.Render();
        coordUI.Render();
        positionUI.Render();
        playerUI.Render();
        gameStatusUI.Render();
        whiteTimerUI.Render();
        blackTimerUI.Render();
    }
}