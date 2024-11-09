namespace Chess.ChessEngine;

using Chess.API;

class FenUtility {
    public static string startingFen = "RNBQKBNR/PPPPPPPP/8/8/8/8/pppppppp/rnbqkbnr w KQkq - 0 1"; // Default starting position

    private static Dictionary<char, int> pieceTypeByChar = new() {
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
                    board.Square[squareIndex] = new Piece(PieceType.None);
                    squareIndex++;
                }
            } else {
                int type = pieceTypeByChar[char.ToLower(c)];
                bool isWhite = char.IsUpper(c);

                board.Bitboard[type].SetBit(squareIndex);
                board.Bitboard[isWhite ? PieceType.White : PieceType.Black].SetBit(squareIndex);
                
                Piece piece = new Piece(type, isWhite ? PieceType.White : PieceType.Black);

                board.Square[squareIndex] = piece;
                squareIndex++;

            }
        }

        board.IsWhiteTurn = parts[1] == "w";

        board.CastlingRights |= parts[2].Contains('K') ? 0b1000 : 0;
        board.CastlingRights |= parts[2].Contains('Q') ? 0b0100 : 0;
        board.CastlingRights |= parts[2].Contains('k') ? 0b0010 : 0;
        board.CastlingRights |= parts[2].Contains('q') ? 0b0001 : 0;

        if (parts[3] != "-") board.EnPassantSquare = BoardHelper.SquareIndexFromName(parts[3]);

        board.HalfMoveClock = int.Parse(parts[4]);

        board.MoveCount = int.Parse(parts[5]);
    }
}