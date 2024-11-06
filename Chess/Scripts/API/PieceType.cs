namespace Chess.API;

class PieceType {
    private int pieceValue;

    public static int None = 0;
    public static int Pawn = 1;
    public static int Knight = 2;
    public static int Bishop = 3;
    public static int Rook = 4;
    public static int Queen = 5;
    public static int King = 6; 

    public static int White = 8;
    public static int Black = 16;

    public PieceType(int color, int type = 0) {
        this.pieceValue = color | type;
    }

    public int Color => pieceValue & 0b00011000;
    public int Type => pieceValue & 0b00000111;
}