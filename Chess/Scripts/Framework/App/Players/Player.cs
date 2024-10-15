namespace Chess.App;

class Player {
    private bool isWhite { get; }
    private bool isHuman { get; }

    public Player(bool isWhite, bool isHuman) {
        this.isWhite = isWhite;
        this.isHuman = isHuman;
    }

    public String Color => isWhite ? "White" : "Black";
    public String PlayerType => isHuman ? "Human" : "Bot";

    public bool IsWhite => isWhite;
    public bool IsBlack => !isWhite;

    public bool IsHuman => isHuman;
    public bool IsBot => !isHuman;

    public override String ToString() {
        return $"{ PlayerType } | { Color }";
    }
}