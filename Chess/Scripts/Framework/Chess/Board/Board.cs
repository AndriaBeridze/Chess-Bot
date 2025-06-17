namespace Chess.ChessEngine;

using Chess.API;

class Board {
    public Piece[] Square = new Piece[64];
    public bool IsWhiteTurn = true;
    public int CastlingRights = 0b0000;
    public int EnPassantSquare = -1;
    public int HalfMoveClock = 0;
    public int MoveCount = 1;

    public List<ulong> PastZobristKeys = new List<ulong>();
    public List<Move> MovesMade = new List<Move>();

    // Bitboard for pieces
    public Dictionary<int, Bitboard> Type = new Dictionary<int, Bitboard> {
        { Piece.Pawn, new Bitboard(0) },
        { Piece.Knight, new Bitboard(0) },
        { Piece.Bishop, new Bitboard(0) },
        { Piece.Rook, new Bitboard(0) },
        { Piece.Queen, new Bitboard(0) },
        { Piece.King, new Bitboard(0) },
    };

    // Bitboard for colors
    public Dictionary<bool, Bitboard> Color = new Dictionary<bool, Bitboard> {
        { true, new Bitboard(0) }, // White
        { false, new Bitboard(0) }, // Black
    };

    public Bitboard Occupied => Color[true] | Color[false];
    public Bitboard Empty => ~Occupied;

    public ulong ZobristKey;

    public Move LastMove => MovesMade.Count > 0 ? MovesMade[^1] : Move.NullMove;

    public Board(string fen) {
        FenUtility.LoadFen(fen, this);
        MovesMade = new List<Move>(); // Clearing the list of moves after resetting the game
        
        ZobristKey = ZobristHashing.CalculateZobristKey(this);
        PastZobristKeys = new List<ulong> { ZobristKey }; // Initialize with the current key
    }

    public void SwitchTurn() => IsWhiteTurn = !IsWhiteTurn;
    
    public void MakeMove(Move move, bool record = false) {
        MoveUtility.MakeMove(this, move);
        SwitchTurn();
        if (IsWhiteTurn) MoveCount++;

        ZobristKey = ZobristHashing.CalculateZobristKey(this);
        PastZobristKeys.Add(ZobristKey);

        if (record) MovesMade.Add(move);
    }

    public void UnmakeMove(Move move) {
        if (IsWhiteTurn) MoveCount--;
        SwitchTurn();
        MoveUtility.UnmakeMove(this, move);

        PastZobristKeys.RemoveAt(PastZobristKeys.Count - 1);
        ZobristKey = ZobristHashing.CalculateZobristKey(this);
    }

    public int CountZobristKeys(ulong zobristKey) {
        int count = 0;
        foreach (ulong key in PastZobristKeys) {
            if (key == zobristKey) count++;
        }
        return count;
    }
}