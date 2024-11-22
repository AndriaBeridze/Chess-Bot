namespace Chess.ChessEngine;

class Masks {
    public static Bitboard[] Row = {
        new Bitboard(0x00000000000000ff),
        new Bitboard(0x000000000000ff00),
        new Bitboard(0x0000000000ff0000),
        new Bitboard(0x00000000ff000000),
        new Bitboard(0x000000ff00000000),
        new Bitboard(0x0000ff0000000000),
        new Bitboard(0x00ff000000000000),
        new Bitboard(0xff00000000000000)
    };

    public static Bitboard[] Column = {
        new Bitboard(0x0101010101010101),
        new Bitboard(0x0202020202020202),
        new Bitboard(0x0404040404040404),
        new Bitboard(0x0808080808080808),
        new Bitboard(0x1010101010101010),
        new Bitboard(0x2020202020202020),
        new Bitboard(0x4040404040404040),
        new Bitboard(0x8080808080808080)
    };

    // Diagonals have an unique property: row - col = constant for each square on diagonal
    // We can use this property to generate bitboards for each diagonal where index of the bitboard is row - col + 7
    // For example, for the main diagonal (row - col = 0) index is 7 
    // Reason why I chose +7 is because for the square (0, 7) diagonal index is -7, which is invalid
    public static Bitboard[] Diagonal = {
        new Bitboard(0x0000000000000080),
        new Bitboard(0x0000000000008040),
        new Bitboard(0x0000000000804020),
        new Bitboard(0x0000000080402010),
        new Bitboard(0x0000008040201008),
        new Bitboard(0x0000804020100804),
        new Bitboard(0x0080402010080402),
        new Bitboard(0x8040201008040201),
        new Bitboard(0x4020100804020100),
        new Bitboard(0x2010080402010000),
        new Bitboard(0x1008040201000000),
        new Bitboard(0x0804020100000000),
        new Bitboard(0x0402010000000000),
        new Bitboard(0x0201000000000000),
        new Bitboard(0x0100000000000000)
    };

    // Anti-diagonals have an unique property: row + col = constant for each square on anti-diagonal
    // We can use this property to generate bitboards for each anti-diagonal where index of the bitboard is row + col
    public static Bitboard[] AntiDiagonal = {
        new Bitboard(0x0000000000000001),
        new Bitboard(0x0000000000000102),
        new Bitboard(0x0000000000010204),
        new Bitboard(0x0000000001020408),
        new Bitboard(0x0000000102040810),
        new Bitboard(0x0000010204081020),
        new Bitboard(0x0001020408102040),
        new Bitboard(0x0102040810204080),
        new Bitboard(0x0204081020408000),
        new Bitboard(0x0408102040800000),
        new Bitboard(0x0810204080000000),
        new Bitboard(0x1020408000000000),
        new Bitboard(0x2040800000000000),
        new Bitboard(0x4080000000000000),
        new Bitboard(0x8000000000000000)

    };  

    public static Bitboard KnightMoves = new Bitboard(0xa1100110a);
    public static Bitboard KingMoves = new Bitboard(0x70507);
}