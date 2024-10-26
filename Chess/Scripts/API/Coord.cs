namespace Chess.API;

class Coord {
    public int RowIndex;
    public int ColumnIndex;

    public int SquareIndex => RowIndex * 8 + ColumnIndex;

    public bool IsValidSquare => ColumnIndex >= 0 && ColumnIndex < 8 && RowIndex >= 0 && RowIndex < 8;
    public bool IsLightColor => (ColumnIndex + RowIndex) % 2 != 0;

    public Coord(int rowIndex, int columnIndex) {
        this.RowIndex = rowIndex;
        this.ColumnIndex = columnIndex;
    }

    public Coord(int squareIndex) {
        RowIndex = squareIndex >> 3;
        ColumnIndex = squareIndex & 7;
    }

    public static Coord FromString(string coord) {
        return new Coord(coord[0] - 'a', coord[1] - '1');
    }

    public override string ToString() {
        return $"{(char)('a' + ColumnIndex)}{RowIndex + 1}";
    }

    public static bool operator ==(Coord a, Coord b) => a.ColumnIndex == b.ColumnIndex && a.RowIndex == b.RowIndex;
    public static bool operator !=(Coord a, Coord b) => a.ColumnIndex != b.ColumnIndex || a.RowIndex != b.RowIndex;
    public override bool Equals(object? obj) => obj is Coord coord && this == coord;
    public override int GetHashCode() => SquareIndex;

    public static Coord operator +(Coord a, Coord b) => new(a.ColumnIndex + b.ColumnIndex, a.RowIndex + b.RowIndex);
    public static Coord operator -(Coord a, Coord b) => new(a.ColumnIndex - b.ColumnIndex, a.RowIndex - b.RowIndex);
    public static Coord operator *(Coord a, int b) => new(a.ColumnIndex * b, a.RowIndex * b);
    public static Coord operator *(int b, Coord a) => a * b;  

}