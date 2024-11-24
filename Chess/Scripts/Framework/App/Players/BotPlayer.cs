namespace Chess.App;

using Chess.API;
using Chess.Bot;
using Chess.ChessEngine;

class BotPlayer : Player {
    public BotPlayer(bool isWhite) : base(isWhite, false) {}

    public override Move Search(Board board, ref int depth, ref float eval) {
        return Bot.Think(board, ref depth, ref eval);
    }
}