namespace Chess.App;

using Raylib_cs;
using Chess.API;

class BoardUI {
    private SquareUI[] squares;

    public BoardUI() {
        squares = new SquareUI[64];

        for (int i = 0; i < 64; i++) {
            squares[i] = new SquareUI(new Coord(i));
        }
    }

    public void Render() {
        for (int i = 0; i < 64; i++) {
            squares[i].Render();
        }
    }
}