namespace Chess.ChessEngine;

using Chess.API;

class Board {
    public Piece[] Square = new Piece[64];
    public bool IsWhiteTurn = true;

    public bool WhiteShortCastle = true;
    public bool WhiteLongCastle = true;
    public bool BlackShortCastle = true;
    public bool BlackLongCastle = true;

    public int enPassantSquare = -1;

    public int HalfMoveClock = 0;

    public int MoveCount = 1;

    public Board(string fen) {
        FenUtility.LoadFen(fen, this);
    }

    public void SwitchTurn() {
        IsWhiteTurn = !IsWhiteTurn;
    }

    public Piece GetPiece(int index) {
        return Square[index];
    }

    public void MakeMove(Move move) {
        MoveUtility.MakeMove(this, move);
    }
}