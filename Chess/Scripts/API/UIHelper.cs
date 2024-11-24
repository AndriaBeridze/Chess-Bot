namespace Chess.API;

using Raylib_cs;

class UIHelper {
    // Finding square's x position on screen by its column index
    public static int GetScreenX(int columnIndex) {
        return Theme.ScreenWidth / 2 - (Theme.FromWhitesView ? (4 - columnIndex) : (columnIndex - 3)) * Theme.SquareSideLength;
    }

    // Finding square's y position on screen by its row index
    public static int GetScreenY(int rowIndex) {
        return Theme.ScreenHeight / 2 - (Theme.FromWhitesView ? (rowIndex - 3) : (4 - rowIndex)) * Theme.SquareSideLength;
    }

    public static int GetScreenX(Coord coord) => GetScreenX(coord.ColumnIndex);
    public static int GetScreenY(Coord coord) => GetScreenY(coord.RowIndex);

    public static Font LoadFont(int fontSize) {
        string fontPath = "Chess/Resources/Fonts/Nunito-Medium.ttf";
        return Raylib.LoadFontEx(fontPath, fontSize, null, 0);
    }

    public static string GetPieceName(Piece piece) {
        if (piece.IsKing) return "King";
        if (piece.IsQueen) return "Queen";
        if (piece.IsRook) return "Rook";
        if (piece.IsBishop) return "Bishop";
        if (piece.IsKnight) return "Knight";
        if (piece.IsPawn) return "Pawn";
        return "";
    }

    public static string GetPieceColor(Piece piece) {
        return piece.IsWhite ? "White" : "Black";
    }

    // Get an image url corresponding to the piece
    public static string GetImageNameByPiece(Piece piece) {
        return GetPieceColor(piece).ToLower() + GetPieceName(piece).ToLower() + ".png";
    }
}