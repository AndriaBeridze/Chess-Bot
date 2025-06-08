namespace Chess.UI;

using Chess.API;
using Chess.Utility;
using Raylib_cs;

class Board {
    private Rectangle desk; // Board frame
    private Square[] squares;
    private Move lastMove = Move.NullMove; // For highlighting the last move

    public Board() {
        int deskX = UIHelper.GetScreenX(Settings.FromWhitesView ? 0 : 7) - Settings.BorderSize;
        int deskY = UIHelper.GetScreenY(Settings.FromWhitesView ? 7 : 0) - Settings.BorderSize;
        int deskSideLength = 8 * Settings.SquareSideLength + 2 * Settings.BorderSize;

        desk = new Rectangle(deskX, deskY, deskSideLength, deskSideLength);

        squares = new Square[64];
        for (int i = 0; i < 64; i++) {
            squares[i] = new Square(new Coord(i));
        }
        
        lastMove = Move.NullMove; // When the game is started, there is no last move
        Clear();
    }

    public void SetColor(int index, Color color) {
        squares[index].SetColor(color);
    }

    public void HighlightSquare(int index) {
        squares[index].SetColor(new Coord(index).IsLightColor ? Theme.SelectedLight : Theme.SelectedDark);
    }

    public void HighlightValidMoves(List<Move> moves) {
        foreach (Move move in moves) {
            squares[move.Target].SetColor(move.TargetCoord.IsLightColor ? Theme.LegalLight : Theme.LegalDark);
        }
    }

    // To highlight the last move on the board
    public void SetLastMove(Move move) {
        lastMove = move;
        Clear();
    }

    public bool IsValidToMove(int index) {
        // Check if the square is highlighted as a legal move
        return squares[index].Color.Equals(Theme.LegalLight) || squares[index].Color.Equals(Theme.LegalDark);
    }

    // Clearing the board from legal move and check highlights
    public void Clear() {
        for (int i = 0; i < 64; i++) {
            squares[i] = new Square(new Coord(i));
        }
        if (!lastMove.IsNull) {
            HighlightSquare(lastMove.Source);
            HighlightSquare(lastMove.Target);
        }
    }

    public void Render() {
        Raylib.DrawRectangleRec(desk, Theme.DeskBackCol);
        for (int i = 0; i < 64; i++) {
            squares[i].Render();
        }
    }
}