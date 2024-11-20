namespace Chess.ChessEngine;

using Chess.API;

class Board {
    public Piece[] Square = new Piece[64];
    public bool IsWhiteTurn = true;
    public int CastlingRights = 0b0000;
    public int EnPassantSquare = -1;
    public int HalfMoveClock = 0;
    public int MoveCount = 1;

    public List<Move> MovesMade = new List<Move>();

    // Bitboard for pieces
    public Dictionary<int, Bitboard> Type = new Dictionary<int, Bitboard> {
        { PieceType.Pawn, new Bitboard(0) },
        { PieceType.Knight, new Bitboard(0) },
        { PieceType.Bishop, new Bitboard(0) },
        { PieceType.Rook, new Bitboard(0) },
        { PieceType.Queen, new Bitboard(0) },
        { PieceType.King, new Bitboard(0) },
    };

    // Bitboard for colors
    public Dictionary<bool, Bitboard> Color = new Dictionary<bool, Bitboard> {
        { true, new Bitboard(0) }, // White
        { false, new Bitboard(0) }, // Black
    };

    public Bitboard Occupied => Color[true] | Color[false];
    public Bitboard Empty => ~Occupied;

    public Board(string fen) {
        FenUtility.LoadFen(fen, this);
    }

    public void SwitchTurn() {
        IsWhiteTurn = !IsWhiteTurn;
    }
    
    public void MakeMove(Move move, bool record = false) {
        MoveUtility.MakeMove(this, move);
        SwitchTurn();
        if (IsWhiteTurn) MoveCount++;

        if (record) MovesMade.Add(move);
    }

    public void UnmakeMove(Move move) {
        if (IsWhiteTurn) MoveCount--;
        SwitchTurn();
        MoveUtility.UnmakeMove(this, move);
    }
}