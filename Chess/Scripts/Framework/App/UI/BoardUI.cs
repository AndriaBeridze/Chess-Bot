namespace Chess.App;

using Chess.API;
using Raylib_cs;

class BoardUI {
    private Rectangle desk; // Board frame
    private SquareUI[] squares;
    private Move lastMove = Move.NullMove; // For highlighting the last move

    public BoardUI() {
        int deskX = UIHelper.GetScreenX(Theme.FromWhitesView ? 0 : 7) - Theme.BorderSize;
        int deskY = UIHelper.GetScreenY(Theme.FromWhitesView ? 7 : 0) - Theme.BorderSize;
        int deskSideLength = 8 * Theme.SquareSideLength + 2 * Theme.BorderSize;

        desk = new Rectangle(deskX, deskY, deskSideLength, deskSideLength);

        squares = new SquareUI[64];
        for (int i = 0; i < 64; i++) {
            squares[i] = new SquareUI(new Coord(i));
        }
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
            squares[i] = new SquareUI(new Coord(i));
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