namespace Chess.App;

using Raylib_cs;
using Chess.Testing;
using Chess.API;

class Program {
    static void Main(string[] args) {
        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(Theme.ScreenWidth, Theme.ScreenHeight, "Chess by Andria Beridze");
        Raylib.SetTargetFPS(60);

        // Default game: Human vs Bot | Initial position | Board visible from white's perspective
        Game game = new Game(new BotPlayer(true), new BotPlayer(false), "", true);

        while (!Raylib.WindowShouldClose()) {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Theme.BackgroundColor);

            game.Update();
            game.Render();

            Raylib.EndDrawing();
        }
    }
}