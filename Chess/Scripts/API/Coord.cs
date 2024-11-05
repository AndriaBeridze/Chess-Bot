namespace Chess.API;

class Coord {
    public int RowIndex;
    public int ColumnIndex;

    // (0, 0) -> 0
    // (7, 7) -> 63
    public int SquareIndex => RowIndex * 8 + ColumnIndex;

    public bool IsValidSquare => ColumnIndex >= 0 && ColumnIndex < 8 && RowIndex >= 0 && RowIndex < 8; //If inside the bounds
    public bool IsLightColor => (ColumnIndex + RowIndex) % 2 != 0;

    public Coord(int rowIndex, int columnIndex) {
        this.RowIndex = rowIndex;
        this.ColumnIndex = columnIndex;
    }

    //  0 -> (0, 0)
    // 63 -> (7, 7)
    public Coord(int squareIndex) {
        RowIndex = squareIndex >> 3;
        ColumnIndex = squareIndex & 7;
    }

    // A1 -> (0, 0)
    // H8 -> (7, 7)
    public static Coord FromString(string coord) {
        return new Coord(coord[0] - 'a', coord[1] - '1');
    }

    // (0, 0) -> A1
    // (7, 7) -> H8
    public override string ToString() {
        return $"{(char)('a' + ColumnIndex)}{RowIndex + 1}";
    }

    public static bool operator ==(Coord a, Coord b) => a.RowIndex == b.RowIndex && a.ColumnIndex == b.ColumnIndex;
    public static bool operator !=(Coord a, Coord b) => a.RowIndex != b.RowIndex || a.ColumnIndex != b.ColumnIndex;
    public override bool Equals(object? obj) => obj is Coord coord && this == coord;
    public override int GetHashCode() => SquareIndex;
    
    // Knight move can be represented as (1, 2); adding this to the current position gives the new position.
    public static Coord operator +(Coord a, Coord b) => new(a.RowIndex + b.RowIndex, a.ColumnIndex + b.ColumnIndex);
    public static Coord operator -(Coord a, Coord b) => new(a.RowIndex - b.RowIndex, a.ColumnIndex - b.ColumnIndex);
    public static Coord operator *(Coord a, int b) => new(a.RowIndex * b, a.ColumnIndex * b);
    public static Coord operator *(int b, Coord a) => a * b;  

}