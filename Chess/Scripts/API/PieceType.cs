namespace Chess.API;

class PieceType {
    private int pieceValue;

    public const int None = 0;
    public const int Pawn = 1;
    public const int Knight = 2;
    public const int Bishop = 3;
    public const int Rook = 4;
    public const int Queen = 5;
    public const int King = 6; 

    public const int White = 8;
    public const int Black = 16;

    public PieceType(int color, int type = 0) {
        this.pieceValue = color | type;
    }

    public int FriendlyColor => pieceValue & 0b00011000;
    public int EnemyColor => FriendlyColor == White ? Black : White;
    public int Type => pieceValue & 0b00000111;
}