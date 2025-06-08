namespace Chess.Bot;

using Chess.API;
using Chess.ChessEngine;

class OpeningBook {
    // If we let bot play without opening books, it will respond with random moves again and again
    // Opening book is a collection of grandmaster games' opening moves
    // Recorded moves are used to determine the matching positions and the next move to make
    // If there is a multiple matching position, a random move is selected
    private List<List<Move>> book = new List<List<Move>>();

    public OpeningBook() {
        string[] openings = File.ReadAllLines("Chess/Resources/Openings.txt"); // Openings.txt is a file that contains opening moves in the coordinate notation

        for (int openingIndex = 0; openingIndex < openings.Length; openingIndex++) {
            string opening = openings[openingIndex];
            List<Move> currentOpeningList = new List<Move>();
            string[] moves = opening.Split(' ');

            for (int i = 0; i < moves.Length; i++) {
                string move = moves[i];
                if (i == moves.Length - 1) continue; // When reading the line, '\n' is also read

                int source = BoardHelper.SquareIndexFromName(move.Substring(0, 2)); // First two characters are the source square
                int target = BoardHelper.SquareIndexFromName(move.Substring(2, 2)); // Next two characters are the target square
                int flag = 0;

                // Whenever I was converting the games from pgn to coordinate notation, I added letter at the end of the move to indicate the various flags
                // q - Queen Promotion
                // r - Rook Promotion
                // b - Bishop Promotion
                // n - Knight Promotion
                // c - Castling
                // p - En Passant
                if (move.Length == 5) {
                    switch (move[4]) {
                        case 'q':
                            flag = Move.QueenPromotion;
                            break;
                        case 'r':
                            flag = Move.RookPromotion;
                            break;
                        case 'b':
                            flag = Move.BishopPromotion;
                            break;
                        case 'n':
                            flag = Move.KnightPromotion;
                            break;
                        case 'c':
                            flag = Move.Castling;
                            break;
                        case 'p':
                            flag = Move.EnPassant;
                            break;
                    }
                }

                currentOpeningList.Add(new Move(source, target, (ushort)flag));
            }

            book.Add(currentOpeningList);
        }
    }

    // Gets random move from the possible opening positions
    public Move GetMove(List<Move> movesMade) {
        Random rnd = new Random();
        List<Move> possibleMoves = new List<Move>();

        foreach (List<Move> moves in book) {
            if (moves.Count <= movesMade.Count) continue; // If there is less moves in the opening than the moves made, skip

            bool match = true;
            for (int i = 0; i < movesMade.Count; i++) {
                if (!moves[i].Equals(movesMade[i])) { 
                    // Position was not matched
                    match = false;
                    break;
                }
            }

            if (match) possibleMoves.Add(moves[movesMade.Count]);
        }

        if (possibleMoves.Count == 0) return Move.NullMove;
        return possibleMoves[rnd.Next(possibleMoves.Count)];
    }
}