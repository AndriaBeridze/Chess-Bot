namespace Chess.API;

class Piece {
    private PieceType piece;

    public int Type => piece.Type;
    public int Color => piece.Color;

    public bool IsWhite => Color == PieceType.White;
    public bool IsBlack => Color == PieceType.Black;

    public bool IsNone => Type == PieceType.None;
    public bool IsPawn => Type == PieceType.Pawn;
    public bool IsRook => Type == PieceType.Rook;
    public bool IsKnight => Type == PieceType.Knight;
    public bool IsBishop => Type == PieceType.Bishop;
    public bool IsQueen => Type == PieceType.Queen;
    public bool IsKing => Type == PieceType.King;
    
    // Sliding piece movements can be combined and calculated in one go
    public bool IsSlidingPiece => IsRook || IsBishop || IsQueen; 

    public Piece(int color, int type = 0) {
        piece = new PieceType(color, type);
    }

    public override string ToString() {
        if (IsNone) return "None";
        return $"{(IsWhite ? "White" : "Black")} {Type}";
    }

    public static bool operator ==(Piece a, Piece b) => a.IsWhite == b.IsWhite && a.Type == b.Type;
    public static bool operator !=(Piece a, Piece b) => !(a == b);
    public bool Equals(Piece other) => this == other;
    public override bool Equals(object? obj) => obj is Piece other && Equals(other);
    public override int GetHashCode() => piece.GetHashCode();
}