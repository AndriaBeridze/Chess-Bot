namespace Chess.API;

class Piece {
    // Piece is hashed into a 5 5 bit integer
    // Structure is following: CCTTT
    // C - color, T - type
    private int value;

    public const int None = 0;
    public const int Pawn = 1;
    public const int Knight = 2;
    public const int Bishop = 3;
    public const int Rook = 4;
    public const int Queen = 5;
    public const int King = 6;

    public const int White = 8;
    public const int Black = 16;

    public int Color => value & 0b00011000;
    public int Type => value & 0b00000111;
    public int FriendlyColor => Color;
    public int EnemyColor => FriendlyColor ^ 0b00011000;

    public bool IsWhite => !IsNone && Color == White;
    public bool IsBlack => !IsNone && Color == Black;

    public bool IsNone => Type == None;
    public bool IsPawn => Type == Pawn;
    public bool IsKnight => Type == Knight;
    public bool IsBishop => Type == Bishop;
    public bool IsRook => Type == Rook;
    public bool IsQueen => Type == Queen;
    public bool IsKing => Type == King;
    
    // Sliding piece movements can be combined and calculated in one go
    public bool IsSlidingPiece => IsRook || IsBishop || IsQueen;

    public override string ToString()
    {
        if (IsNone) return ".";
        string name = Type switch {
            Pawn => "P",
            Knight => "N",
            Bishop => "B",
            Rook => "R",
            Queen => "Q",
            King => "K",
            _ => "?"
        };
        if (IsBlack) name = name.ToLower();
        
        return name;
    }

    public Piece(int color, int type = 0) {
        value = color | type;
    }
}