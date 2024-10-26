namespace Chess.API;

class Piece {
    private bool? isWhite;
    private PieceType type;

    public bool IsWhite => isWhite != null && isWhite.Value;
    public bool IsBlack => isWhite != null && !isWhite.Value;

    public bool IsNone => type == PieceType.None;
    public bool IsPawn => type == PieceType.Pawn;
    public bool IsRook => type == PieceType.Rook;
    public bool IsKnight => type == PieceType.Knight;
    public bool IsBishop => type == PieceType.Bishop;
    public bool IsQueen => type == PieceType.Queen;
    public bool IsKing => type == PieceType.King;

    public Piece(bool? isWhite, PieceType type) {
        this.isWhite = isWhite;
        this.type = type;
    }

    public override string ToString() {
        if (IsNone) return "None";
        return $"{(isWhite.HasValue && isWhite.Value ? "White" : "Black")} {type}";
    }

    public static bool operator ==(Piece a, Piece b) => a.IsWhite == b.IsWhite && a.type == b.type;
    public static bool operator !=(Piece a, Piece b) => !(a == b);
    public bool Equals(Piece other) => this == other;
    public override bool Equals(object? obj) => obj is Piece other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(isWhite, type);
}