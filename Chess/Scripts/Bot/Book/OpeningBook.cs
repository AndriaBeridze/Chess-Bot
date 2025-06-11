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
        using (FileStream fs = File.OpenRead("Chess/Resources/Openings/Books.bin"))
        using (BinaryReader br = new BinaryReader(fs)) {
            int openingCount = br.ReadInt32();
            for (int i = 0; i < openingCount; i++) {
                int moveCount = br.ReadInt32();
                List<Move> opening = new List<Move>();
                for (int j = 0; j < moveCount; j++) {
                    int source = br.ReadInt32();
                    int target = br.ReadInt32();
                    ushort flag = br.ReadUInt16();
                    opening.Add(new Move(source, target, flag));
                }
                book.Add(opening);
            }
        }
    }

    // Run this once to generate the .bin file
    public static void GenerateBinaryOpeningBook(string inputPath, string outputPath) {
        var openings = File.ReadAllLines(inputPath);
        using var fs = File.Create(outputPath);
        using var bw = new BinaryWriter(fs);
        bw.Write(openings.Length);

        foreach (string line in openings) {
            var moves = line.Split(' ');
            bw.Write(moves.Length - 1); // Skip last if empty
            for (int i = 0; i < moves.Length - 1; i++) {
                string move = moves[i];
                int source = BoardHelper.SquareIndexFromName(move.Substring(0, 2));
                int target = BoardHelper.SquareIndexFromName(move.Substring(2, 2));
                ushort flag = 0;
                if (move.Length == 5) {
                    flag = move[4] switch {
                        'q' => Move.QueenPromotion,
                        'r' => Move.RookPromotion,
                        'b' => Move.BishopPromotion,
                        'n' => Move.KnightPromotion,
                        'c' => Move.Castling,
                        'p' => Move.EnPassant,
                        _ => 0
                    };
                }
                bw.Write(source);
                bw.Write(target);
                bw.Write(flag);
            }
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