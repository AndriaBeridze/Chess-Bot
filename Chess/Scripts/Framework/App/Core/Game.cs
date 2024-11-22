namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.ChessEngine;
using Chess.Bot;

class Game {
    private Player whitePlayer;
    private Player blackPlayer;

    private static Board board = new Board("");

    private static BoardUI boardUI = new BoardUI();
    private CoordUI coordUI;
    private static PositionUI positionUI = new PositionUI(board);
    private PlayerUI playerUI;
    private GameStatus gameStatus;

    private static OpeningBook openingBook = new OpeningBook();

    private static Player currentPlayer = new HumanPlayer(true);
    private static bool canBeChecked = true;

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

        currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;
    }

    public void Update() {
        if (!canBeChecked) goto Skip;

        string status = Arbiter.Status(board);
        if (status != "") {
            Color color = Color.White;

            if (status == "Checkmate") color = Theme.CheckmateTextColor;
            if (status == "Stalemate") color = Theme.StalemateTextColor;
            if (status == "Draw") color = Theme.DrawTextColor;

            gameStatus = new GameStatus(status, color);

            return;
        }

        Skip:

        // Displaying bot moves
        currentPlayer = board.IsWhiteTurn ? whitePlayer : blackPlayer;
        if (currentPlayer.IsBot && canBeChecked) {
            canBeChecked = false;

            Task.Run(() => {
                GetBotMove();
            });
        }

        positionUI.Update(board, boardUI, highlightMoves : currentPlayer.IsHuman);

        if (board.MovesMade.Count > 0 && board.MovesMade[board.MovesMade.Count - 1].IsPromotion) {
            Move move = board.MovesMade[board.MovesMade.Count - 1];
            int index = positionUI.Pieces.FindIndex(piece => piece.Coord == new Coord(move.Target));
            int color = board.IsWhiteTurn ? PieceType.Black : PieceType.White;
            if (index == -1) return;
            switch (move.Flag) {
                case Move.QueenPromotion:
                    positionUI.Pieces[index] = new PieceUI(new Piece(color, PieceType.Queen), new Coord(move.Target));
                    break;
                case Move.RookPromotion:
                    positionUI.Pieces[index] = new PieceUI(new Piece(color, PieceType.Rook), new Coord(move.Target));
                    break;
                case Move.BishopPromotion:
                    positionUI.Pieces[index] = new PieceUI(new Piece(color, PieceType.Bishop), new Coord(move.Target));
                    break;
                case Move.KnightPromotion:
                    positionUI.Pieces[index] = new PieceUI(new Piece(color, PieceType.Knight), new Coord(move.Target));
                    break;
            }   
        }
    }

    private static void GetBotMove() {
        Move move = openingBook.GetMove(board.MovesMade);
        if (move.IsNull) move = currentPlayer.Search(board);

        Task.Run(() => {
            AnimateMove(move);
        });
    }

    private static void AnimateMove(Move move) {
        positionUI.AnimateMove(move, board);
        board.MakeMove(move, record : true);
        boardUI.SetLastMove(move);

        Thread.Sleep(100);

        canBeChecked = true;
    }

    public void Render() {
        boardUI.Render();
        coordUI.Render();
        positionUI.Render();
        playerUI.Render();
        gameStatus.Render();
    }
}