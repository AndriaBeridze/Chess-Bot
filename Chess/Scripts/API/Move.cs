namespace Chess.API;

class Move {
    public Coord startingCoord;
    public Coord endingCoord;

    public Move(Coord startingCoord, Coord endingCoord) {
        this.startingCoord = startingCoord;
        this.endingCoord = endingCoord;
    }
}