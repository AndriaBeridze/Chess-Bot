namespace Chess.App;

using Chess.API;
using Chess.Bot;
using Chess.ChessEngine;

class BotPlayer : Player {
    public BotPlayer(bool isWhite) : base(isWhite, false) {}

    public override Move Search(Board board) {
        return Bot.Think(board);
    }
}