namespace Chess.UI;

using Chess.API;
using Chess.Utility;
using Raylib_cs;

class UIHelper {
    // Finding square's x position on screen by its column index
    public static int GetScreenX(int columnIndex) {
        return Settings.ScreenWidth / 2 - (Settings.FromWhitesView ? (4 - columnIndex) : (columnIndex - 3)) * Settings.SquareSideLength;
    }

    // Finding square's y position on screen by its row index
    public static int GetScreenY(int rowIndex) {
        return Settings.ScreenHeight / 2 - (Settings.FromWhitesView ? (rowIndex - 3) : (4 - rowIndex)) * Settings.SquareSideLength;
    }

    public static int GetScreenX(Coord coord) => GetScreenX(coord.ColumnIndex);
    public static int GetScreenY(Coord coord) => GetScreenY(coord.RowIndex);

    public static Font LoadFont(int fontSize) {
        string fontPath = "Chess/Resources/Fonts/Nunito-Medium.ttf";
        return Raylib.LoadFontEx(fontPath, fontSize, null, 0);
    }

    public static string GetPieceName(API.Piece piece) {
        if (piece.IsKing) return "King";
        if (piece.IsQueen) return "Queen";
        if (piece.IsRook) return "Rook";
        if (piece.IsBishop) return "Bishop";
        if (piece.IsKnight) return "Knight";
        if (piece.IsPawn) return "Pawn";
        return "";
    }

    public static string GetPieceColor(API.Piece piece) {
        return piece.IsWhite ? "White" : "Black";
    }

    // Get an image url corresponding to the piece
    public static string GetImageNameByPiece(API.Piece piece) {
        return GetPieceColor(piece) + "/" + GetPieceName(piece) + ".png";
    }
}