using Chess.ChessEngine;

namespace Chess.API;

class Move {
    // Move is hashed into a 16-bit integer
    // Structure is following: FFFFTTTTTTSSSSSS
    // F - move flag, T - target square, S - source square
    private ushort value;

    // Flags
    public const ushort NoFlag = 0b0000;
    public const ushort QueenPromotion = 0b0001;
    public const ushort RookPromotion = 0b0010;
    public const ushort BishopPromotion = 0b0011;
    public const ushort KnightPromotion = 0b0100;
    public const ushort EnPassant = 0b0101;
    public const ushort Castling = 0b0110;

    public Move(int source, int target, ushort flag = 0b0000) {
        value = (ushort) (flag << 12 | target << 6 | source);
    }

    public Move(ushort value) {
        this.value = value;
    }

    public Move(Coord sourceCoord, Coord targetCoord, ushort flag = 0b0000) {
        value = (ushort) (flag << 12 | targetCoord.SquareIndex << 6 | sourceCoord.SquareIndex);
    }

    public int Source => value & 0b00111111;
    public int Target => (value >> 6) & 0b000111111;
    public ushort Flag => (ushort) (value >> 12);

    public Coord SourceCoord => new Coord(Source);
    public Coord TargetCoord => new Coord(Target);

    public bool IsNull => value == 0;
    public bool IsPromotion => Flag == QueenPromotion || Flag == RookPromotion || Flag == BishopPromotion || Flag == KnightPromotion;
    public bool IsEnPassant => Flag == EnPassant;
    public bool IsCastling => Flag == Castling;

    public int PromotingTo => Flag switch {
        QueenPromotion => Piece.Queen,
        RookPromotion => Piece.Rook,
        BishopPromotion => Piece.Bishop,
        KnightPromotion => Piece.Knight,
        _ => Piece.None
    };

    public static Move NullMove => new Move(0, 0, 0);  

    public static bool operator ==(Move a, Move b) => a.value == b.value;
    public static bool operator !=(Move a, Move b) => a.value != b.value;
    public override int GetHashCode() => value;
    public override bool Equals(object ? obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        Move move = (Move) obj;
        return value == move.value;
    }  
}