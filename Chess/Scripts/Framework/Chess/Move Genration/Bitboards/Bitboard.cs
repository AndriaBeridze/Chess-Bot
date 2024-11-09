namespace Chess.ChessEngine;

class Bitboard {
    public ulong Value;

    public Bitboard(ulong value) {
        this.Value = value;
    }

    public void SetBit(int index) => Value |= 1UL << index;
    public void ClearBit(int index) => Value &= ~(1UL << index);

    // For example 1236 is 10011010100 in binary 
    // The result of the function will be 2, because rightmost bit that is 1 is at index 0
    public int FirstBit => (int) Math.Log2(Value & (ulong) - (long) Value);
    
    public bool IsEmpty => this == Null;

    public static Bitboard operator |(Bitboard a, Bitboard b) => new Bitboard(a.Value | b.Value);
    public static Bitboard operator &(Bitboard a, Bitboard b) => new Bitboard(a.Value & b.Value);
    public static Bitboard operator ^(Bitboard a, Bitboard b) => new Bitboard(a.Value ^ b.Value);
    public static Bitboard operator ~(Bitboard a) => new Bitboard(~a.Value);
    public static Bitboard operator <<(Bitboard a, int shift) => new Bitboard(a.Value << shift);
    public static Bitboard operator >>(Bitboard a, int shift) => new Bitboard(a.Value >> shift);

    public static Bitboard operator +(Bitboard a, Bitboard b) => new Bitboard(a.Value + b.Value);  
    public static Bitboard operator -(Bitboard a, Bitboard b) => new Bitboard(a.Value - b.Value);

    public static bool operator ==(Bitboard a, Bitboard b) => a.Value == b.Value;
    public static bool operator !=(Bitboard a, Bitboard b) => a.Value != b.Value;
    public override bool Equals(object? obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
    
    public static Bitboard Null => new Bitboard(0);

    public Bitboard Reverse() {
        ulong value = this.Value;

        value = (value & 0x5555555555555555) << 1 | (value >> 1) & 0x5555555555555555;
        value = (value & 0x3333333333333333) << 2 | (value >> 2) & 0x3333333333333333;
        value = (value & 0x0f0f0f0f0f0f0f0f) << 4 | (value >> 4) & 0x0f0f0f0f0f0f0f0f;
        value = (value & 0x00ff00ff00ff00ff) << 8 | (value >> 8) & 0x00ff00ff00ff00ff;
        value = (value & 0x0000ffff0000ffff) << 16 | (value >> 16) & 0x0000ffff0000ffff;
        value = (value & 0x00000000ffffffff) << 32 | (value >> 32) & 0x00000000ffffffff;

        return new Bitboard(value);
    }

    public string Binary => Convert.ToString((long) Value, 2).PadLeft(64, '0');
}

