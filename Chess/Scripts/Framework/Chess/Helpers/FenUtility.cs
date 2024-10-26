namespace Chess.ChessEngine;

using Chess.API;

class FenUtility {
    public static string startingFen = "RNBQKBNR/PPPPPPPP/8/8/8/8/pppppppp/rnbqkbnr w KQkq - 0 1"; // Default starting position

    private static Dictionary<char, PieceType> pieceTypeByChar = new() {
        { 'p', PieceType.Pawn },
        { 'n', PieceType.Knight },
        { 'b', PieceType.Bishop },
        { 'r', PieceType.Rook },
        { 'q', PieceType.Queen },
        { 'k', PieceType.King }
    };

    public static void LoadFen(string fen, Board board) {
        if (string.IsNullOrEmpty(fen)) fen = startingFen;
        string[] parts = fen.Split(' ');

        int squareIndex = 0;

        foreach (char c in parts[0]) {
            if (c == '/') continue;
            if (char.IsDigit(c)) {
                int emptySquares = c - '0';
                for (int i = 0; i < emptySquares; i++) {
                    board.Square[i] = new Piece(false, PieceType.None);
                    squareIndex++;
                }
            } else {
                PieceType type = pieceTypeByChar[char.ToLower(c)];
                bool isWhite = char.IsUpper(c);
                
                Piece piece = new Piece(isWhite, type);

                board.Square[squareIndex] = piece;
                squareIndex++;
            }
        }

        board.IsWhiteTurn = parts[1] == "w";

        board.WhiteShortCastle = parts[2].Contains('K');
        board.WhiteLongCastle = parts[2].Contains('Q');
        board.BlackShortCastle = parts[2].Contains('k');
        board.BlackLongCastle = parts[2].Contains('q');

        if (parts[3] != "-") board.enPassantSquare = BoardHelper.SquareIndexFromName(parts[3]);

        board.HalfMoveClock = int.Parse(parts[4]);

        board.MoveCount = int.Parse(parts[5]);
    }
}