namespace Chess.ChessEngine;

using Chess.API;

class MoveUtility {
    public static void MakeMove(Board board, Move move) {
        board.Square[move.endingCoord.SquareIndex] = board.Square[move.startingCoord.SquareIndex];
        board.Square[move.startingCoord.SquareIndex] = new Piece(PieceType.None, null);
    }
}