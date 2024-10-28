namespace Chess.API;

using Raylib_cs;

class UIHelper {
    public static void WriteColoredText(string text, ConsoleColor color, bool newLine = true) {
        Console.ForegroundColor = color;
        Console.Write(text + (newLine ? Environment.NewLine : ""));
    }

    public static int GetScreenX(int x) {
        return Theme.ScreenWidth / 2 - (Theme.IsWhitePerspective ? (4 - x) : (x - 3)) * Theme.SquareSideLength;
    }

    public static int GetScreenY(int y) {
        return Theme.ScreenHeight / 2 - (Theme.IsWhitePerspective ? (y - 3) : (4 - y)) * Theme.SquareSideLength;
    }

    public static int GetScreenX(Coord coord) {
        return GetScreenX(coord.ColumnIndex);
    }

    public static int GetScreenY(Coord coord) {
        return GetScreenY(coord.RowIndex);
    }

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

    public static string GetImageNameByPiece(Piece piece) {
        return GetPieceColor(piece).ToLower() + GetPieceName(piece).ToLower() + ".png";
    }
}