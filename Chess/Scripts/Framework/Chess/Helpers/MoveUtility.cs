namespace Chess.ChessEngine;

using Chess.API;

class MoveUtility {
    public static void MakeMove(Board board, Move move) {
        board.Square[move.EndingCoord.SquareIndex] = board.Square[move.StartingCoord.SquareIndex];
        board.Square[move.StartingCoord.SquareIndex] = new Piece(PieceType.None, null);
    }
}