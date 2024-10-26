namespace Chess.App;

class Player {
    public bool IsWhite;
    public bool IsBlack => !IsWhite;

    public bool IsHuman;
    public bool IsBot => !IsHuman;

    public String Color => IsWhite ? "White" : "Black";
    public String PlayerType => IsHuman ? "Human" : "Bot";

    public Player(bool isWhite, bool isHuman) {
        this.IsWhite = isWhite;
        this.IsHuman = isHuman;
    }

    public override String ToString() {
        return $"{ PlayerType } | { Color }";
    }
}