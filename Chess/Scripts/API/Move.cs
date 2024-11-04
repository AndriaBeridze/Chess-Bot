namespace Chess.API;

class Move {
    public Coord StartingCoord;
    public Coord EndingCoord;

    public Move(Coord startingCoord, Coord endingCoord) {
        this.StartingCoord = startingCoord;
        this.EndingCoord = endingCoord;
    }
}