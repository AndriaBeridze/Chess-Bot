namespace Chess.App;

using Chess.API;
using Raylib_cs;
using Chess.Testing;

class Program {
    static void Main(string[] args) {
        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(Theme.ScreenWidth, Theme.ScreenHeight, "Chess by Andria Beridze");
        Raylib.SetTargetFPS(60);

        // Default game: Human vs Bot | Initial position | Board visible from white's view
        Game game = new Game(new BotPlayer(true), new BotPlayer(false), "8/3k4/8/b7/8/8/6K1/8 w - - 0 1", true);

        while (!Raylib.WindowShouldClose()) {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Theme.BackgroundColor);

            game.Update();
            game.Render();

            Raylib.EndDrawing();
        }
    }
}