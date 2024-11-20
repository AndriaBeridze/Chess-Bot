namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

class OpeningBook {
    public List<List<Move>> book = new List<List<Move>>();

    public OpeningBook() {
        string[] lines = File.ReadAllLines("Chess/Resources/Openings.txt");
        foreach (string line in lines) {
            List<Move> current = new List<Move>();
            string[] moves = line.Split(' ');

            for (int i = 0; i < moves.Length; i++) {
                string move = moves[i];
                if (i == moves.Length - 1) continue;

                int source = BoardHelper.SquareIndexFromName(move.Substring(0, 2));
                int target = BoardHelper.SquareIndexFromName(move.Substring(2, 2));
                if (move.Length == 5) {
                    if (move[4] == 'c') {
                        current.Add(new Move(source, target, Move.Castling));
                    } else if (move[4] == 'q') {
                        current.Add(new Move(source, target, Move.QueenPromotion));
                    } else if (move[4] == 'r') {
                        current.Add(new Move(source, target, Move.RookPromotion));
                    } else if (move[4] == 'b') {
                        current.Add(new Move(source, target, Move.BishopPromotion));
                    } else if (move[4] == 'n') {
                        current.Add(new Move(source, target, Move.KnightPromotion));
                    }
                } else {
                    current.Add(new Move(source, target));
                }
            }

            book.Add(current);
        }
    }

    public Move GetMove(List<Move> movesMade) {
        Random rnd = new Random();
        List<Move> possibleGames = new List<Move>();

        foreach (List<Move> moves in book) {
            if (moves.Count <= movesMade.Count) continue;

            bool match = true;
            for (int i = 0; i < movesMade.Count; i++) {
                if (!moves[i].Equals(movesMade[i])) { 
                    match = false;
                    break;
                }
            }

            if (match) {
                possibleGames.Add(moves[movesMade.Count]);
            }
        }

        if (possibleGames.Count > 0) return possibleGames[rnd.Next(possibleGames.Count)];
        return Move.NullMove;
    }
}