namespace Chess.API;

class Coord {
    private int columnIndex;
    private int rowIndex;

    public Coord(int columnIndex, int rowIndex) {
        this.columnIndex = columnIndex;
        this.rowIndex = rowIndex;
    }

    public Coord(int squareIndex) {
        columnIndex = squareIndex & 7;
        rowIndex = squareIndex >> 3;
    }

    public static Coord FromString(string coord) {
        return new Coord(coord[0] - 'a', coord[1] - '1');
    }

    public int ColumnIndex => columnIndex;
    public int RowIndex => rowIndex;
    public int SquareIndex => rowIndex * 8 + columnIndex;

    public bool IsValidSquare => columnIndex >= 0 && columnIndex < 8 && rowIndex >= 0 && rowIndex < 8;
    public bool IsLightColor => (columnIndex + rowIndex) % 2 != 0;

    public override string ToString() {
        return $"{(char)('a' + columnIndex)}{rowIndex + 1}";
    }

    public static bool operator ==(Coord a, Coord b) => a.columnIndex == b.columnIndex && a.rowIndex == b.rowIndex;
    public static bool operator !=(Coord a, Coord b) => a.columnIndex != b.columnIndex || a.rowIndex != b.rowIndex;
    public override bool Equals(object? obj) => obj is Coord coord && this == coord;
    public override int GetHashCode() => SquareIndex;

    public static Coord operator +(Coord a, Coord b) => new(a.columnIndex + b.columnIndex, a.rowIndex + b.rowIndex);
    public static Coord operator -(Coord a, Coord b) => new(a.columnIndex - b.columnIndex, a.rowIndex - b.rowIndex);
    public static Coord operator *(Coord a, int b) => new(a.columnIndex * b, a.rowIndex * b);
    public static Coord operator *(int b, Coord a) => a * b;  

}