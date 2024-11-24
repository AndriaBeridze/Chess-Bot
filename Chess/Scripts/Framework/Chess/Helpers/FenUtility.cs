namespace Chess.ChessEngine;

using Chess.API;

class FenUtility {
    // FEN (Forsyth-Edwards Notation) is a standard notation for describing a particular board position of a chess game.
    // It is split into six parts, separated by spaces:
    // 1. Piece placement (from white's perspective). Each piece is identified by a single letter (uppercase for white, lowercase for black).
    //    - Pieces are identified by the following letters: K (king), Q (queen), R (rook), B (bishop), N (knight), P (pawn).
    //    - Each rank is described, starting with rank 8 and ending with rank 1; within each rank, the contents of each square are described from file a to file h.
    //    - Empty squares are noted using digits 1-8 (the number of empty squares).
    // 2. Active color. "w" means white moves next, "b" means black moves next.
    // 3. Castling availability. Each letter indicates whether a particular move is legal:
    //    - K (white king side castling), Q (white queen side castling), k (black king side castling), q (black queen side castling).
    //    - If there is no castling availability for either side, a hyphen (-) is used.
    // 4. En passant target square in algebraic notation. If there is no en passant target square, a hyphen (-) is used.
    // 5. Halfmove clock: The number of halfmoves since the last pawn advance or capture.
    // 6. Fullmove number: The number of the full move. It starts at 1, and is incremented after black
    public static string startingFen = "RNBQKBNR/PPPPPPPP/8/8/8/8/pppppppp/rnbqkbnr w KQkq - 0 1"; // Default starting position

    private static Dictionary<char, int> PieceByChar = new() {
        { 'p', Piece.Pawn },
        { 'n', Piece.Knight },
        { 'b', Piece.Bishop },
        { 'r', Piece.Rook },
        { 'q', Piece.Queen },
        { 'k', Piece.King }
    };

    public static void LoadFen(string fen, Board board) {
        if (string.IsNullOrEmpty(fen)) fen = startingFen;
        string[] parts = fen.Split(' ');

        int squareIndex = 0;

        // Parse piece placement
        foreach (char c in parts[0]) {
            if (c == '/') continue;
            if (char.IsDigit(c)) {
                int emptySquares = c - '0';
                for (int i = 0; i < emptySquares; i++) {
                    board.Square[squareIndex] = new Piece(Piece.None);
                    squareIndex++;
                }
            } else {
                int type = PieceByChar[char.ToLower(c)];
                bool isWhite = char.IsUpper(c);

                board.Type[type].SetBit(squareIndex);
                board.Color[isWhite].SetBit(squareIndex);
                
                Piece piece = new Piece(type, isWhite ? Piece.White : Piece.Black);

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