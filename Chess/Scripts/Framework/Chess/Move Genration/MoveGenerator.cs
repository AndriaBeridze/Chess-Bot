namespace Chess.ChessEngine;

using Chess.API;

class MoveGenerator {
    public static List<Move> GenerateMoves(Board board, int squareIndex) {
        List<Move> moves = new List<Move>();
        if (board.Square[squareIndex].IsSlidingPiece) {
            moves = GenerateSlidingMoves(board, squareIndex);
        } else if (board.Square[squareIndex].IsKnight) {
            moves = GenerateKnightMoves(board, squareIndex);
        } else if (board.Square[squareIndex].IsKing) {
            moves = GenerateKingMoves(board, squareIndex);
        } else if (board.Square[squareIndex].IsPawn) {
            moves = GeneratePawnMoves(board, squareIndex);
        }

        return moves;
    }

    private static List<Move> GenerateSlidingMoves(Board board, int squareIndex) {
        List<Move> moves = new List<Move>();
        Coord[] direction = { 
            new Coord(1, 0), new Coord(0, 1), new Coord(-1, 0), new Coord(0, -1),
            new Coord(1, 1), new Coord(-1, 1), new Coord(-1, -1), new Coord(1, -1)
        };
        int start = board.Square[squareIndex].IsBishop ? 4 : 0;
        int end = board.Square[squareIndex].IsRook ? 4 : 8;

        for (int i = start; i < end; i++) {
            for (int j = 1; j <= 7; j++) {
                Coord newCoord = new Coord(squareIndex) + direction[i] * j;
                if (!newCoord.IsValidSquare) break;

                if (!board.Square[newCoord.SquareIndex].IsNone && board.Square[newCoord.SquareIndex].IsWhite == board.Square[squareIndex].IsWhite) break;
                
                moves.Add(new Move(new Coord(squareIndex), newCoord));
            
                if (!board.Square[newCoord.SquareIndex].IsNone) break;
            }
        }

        return moves;
    }

    private static List<Move> GenerateKnightMoves(Board board, int squareIndex) {
        List<Move> moves = new List<Move>();
        Coord[] direction = new Coord[] { 
            new Coord(1, 2), new Coord(2, 1), new Coord(-1, 2), new Coord(-2, 1),
            new Coord(1, -2), new Coord(2, -1), new Coord(-1, -2), new Coord(-2, -1)
        };

        for (int i = 0; i < 8; i++) {
            Coord newCoord = new Coord(squareIndex) + direction[i];
            if (!newCoord.IsValidSquare) continue;

            if (!board.Square[newCoord.SquareIndex].IsNone && board.Square[newCoord.SquareIndex].IsWhite == board.Square[squareIndex].IsWhite) continue;
            
            moves.Add(new Move(new Coord(squareIndex), newCoord));
        }

        return moves;
    }

    private static List<Move> GenerateKingMoves(Board board, int squareIndex) {
        List<Move> moves = new List<Move>();
        Coord[] direction = new Coord[] { 
            new Coord(1, 0), new Coord(0, 1), new Coord(-1, 0), new Coord(0, -1),
            new Coord(1, 1), new Coord(-1, 1), new Coord(-1, -1), new Coord(1, -1)
        };

        for (int i = 0; i < 8; i++) {
            Coord newCoord = new Coord(squareIndex) + direction[i];
            if (!newCoord.IsValidSquare) continue;

            if (!board.Square[newCoord.SquareIndex].IsNone && board.Square[newCoord.SquareIndex].IsWhite == board.Square[squareIndex].IsWhite) continue;
            
            moves.Add(new Move(new Coord(squareIndex), newCoord));
        }

        return moves;
    }

    private static List<Move> GeneratePawnMoves(Board board, int squareIndex) {
        List<Move> moves = new List<Move>();
        int dir = (board.Square[squareIndex].IsWhite) ? 1 : -1;
        
        if (board.Square[squareIndex + 8 * dir].IsNone) {
            moves.Add(new Move(new Coord(squareIndex), new Coord(squareIndex + 8 * dir)));

            if (board.Square[squareIndex].IsWhite && squareIndex / 8 == 1) {
                if (board.Square[squareIndex + 16 * dir].IsNone) {
                    moves.Add(new Move(new Coord(squareIndex), new Coord(squareIndex + 16 * dir)));
                }
            } else if (!board.Square[squareIndex].IsWhite && squareIndex / 8 == 6) {
                if (board.Square[squareIndex + 16 * dir].IsNone) {
                    moves.Add(new Move(new Coord(squareIndex), new Coord(squareIndex + 16 * dir)));
                }
            }
        }

        if (board.Square[squareIndex + 7 * dir].IsWhite != board.Square[squareIndex].IsWhite && !board.Square[squareIndex + 7 * dir].IsNone) {
            moves.Add(new Move(new Coord(squareIndex), new Coord(squareIndex + 7 * dir)));
        }

        if (board.Square[squareIndex + 9 * dir].IsWhite != board.Square[squareIndex].IsWhite && !board.Square[squareIndex + 9 * dir].IsNone) {
            moves.Add(new Move(new Coord(squareIndex), new Coord(squareIndex + 9 * dir)));
        }

        return moves;
    }
}