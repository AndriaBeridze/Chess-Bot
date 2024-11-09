namespace Chess.ChessEngine;

using Chess.API;

class Board {
    public Piece[] Square = new Piece[64];
    public bool IsWhiteTurn = true;
    public int CastlingRights = 0b0000;
    public int EnPassantSquare = -1;
    public int HalfMoveClock = 0;
    public int MoveCount = 1;

    
    public Dictionary<int, Bitboard> Bitboard = new Dictionary<int, Bitboard> {
        { PieceType.Pawn, new Bitboard(0) },
        { PieceType.Knight, new Bitboard(0) },
        { PieceType.Bishop, new Bitboard(0) },
        { PieceType.Rook, new Bitboard(0) },
        { PieceType.Queen, new Bitboard(0) },
        { PieceType.King, new Bitboard(0) },
        { PieceType.White, new Bitboard(0) },
        { PieceType.Black, new Bitboard(0) },
    };

    public Bitboard Occupied => Bitboard[PieceType.White] | Bitboard[PieceType.Black];
    public Bitboard Empty => ~Occupied;

    public Board(string fen) {
        FenUtility.LoadFen(fen, this);
    }

    public void SwitchTurn() {
        IsWhiteTurn = !IsWhiteTurn;
    }
    
    public void MakeMove(Move move) {
        MoveUtility.MakeMove(this, move);
        SwitchTurn();
    }
}