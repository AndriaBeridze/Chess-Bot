namespace Chess.ChessEngine;

using Chess.API;

class BoardHelper {
    public const string rowNames = "abcdefgh";
    public const string columnNames = "12345678";

    public static int ColumnIndex(int squareIndex) {
        return squareIndex & 0b00000111;
    }

    public static int RowIndex(int squareIndex) {
        return squareIndex >> 0b00000011;
    }

    public static int DiagonalIndex(int squareIndex) {
        return RowIndex(squareIndex) - ColumnIndex(squareIndex) + 7;
    }

    public static int AntiDiagonalIndex(int squareIndex) {
        return RowIndex(squareIndex) + ColumnIndex(squareIndex);
    }

    public static int IndexFromCoord(int rowIndex, int columnIndex) {
        return columnIndex * 8 + rowIndex;
    }

    public static int IndexFromCoord(Coord coord) {
        return IndexFromCoord(coord.RowIndex, coord.ColumnIndex);
    }

    public static Coord CoordFromIndex(int squareIndex) {
        return new Coord(RowIndex(squareIndex), ColumnIndex(squareIndex));
    }

    public static bool LightSquare(int rowIndex, int columnIndex) {
        return (rowIndex + columnIndex) % 2 != 0;
    }

    public static bool LightSquare(int squareIndex) {
        return LightSquare(RowIndex(squareIndex), ColumnIndex(squareIndex));
    }

    public static string SquareNameFromCoordinate(int rowIndex, int columnIndex) {
        return rowNames[rowIndex] + "" + (columnIndex + 1);
    }

    public static string SquareNameFromIndex(int squareIndex) {
        return SquareNameFromCoordinate(CoordFromIndex(squareIndex));
    }

    public static string SquareNameFromCoordinate(Coord coord) {
        return SquareNameFromCoordinate(coord.RowIndex, coord.ColumnIndex);
    }

    public static int SquareIndexFromName(string name) {
        char rowName = name[0];
        char columnName = name[1];
        int rowIndex = rowNames.IndexOf(rowName);
        int columnIndex = columnNames.IndexOf(columnName);
        return IndexFromCoord(rowIndex, columnIndex);
    }

    public static bool IsValidCoordinate(int x, int y) => x >= 0 && x < 8 && y >= 0 && y < 8;
}