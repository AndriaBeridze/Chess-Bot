namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

class Bot {
    static Random rnd = new Random();

    public static Move Think(Board board) {
        List<Move> moves = MoveGenerator.GenerateMoves(board);

        return moves[rnd.Next(0, moves.Count)];
    }
}