namespace Chess.ChessEngine;

using Chess.API;

class Board {
    public Piece[] Square = new Piece[64];
    public bool IsWhiteTurn = true;
    public int CastlingRights = 0b0000;
    public int EnPassantSquare = -1;
    public int HalfMoveClock = 0;
    public int MoveCount = 1;

    
    public Dictionary<int, ulong> Bitboard = new Dictionary<int, ulong> {
        { PieceType.Pawn, 0 },
        { PieceType.Knight, 0 },
        { PieceType.Bishop, 0 },
        { PieceType.Rook, 0 },
        { PieceType.Queen, 0 },
        { PieceType.King, 0 },
        { PieceType.White, 0 },
        { PieceType.Black, 0 },
    };

    public ulong Occupied => Bitboard[PieceType.White] | Bitboard[PieceType.Black];
    public ulong Empty => ~Occupied;

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