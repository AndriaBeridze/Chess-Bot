namespace Chess.App;

using Raylib_cs;
using Chess.API;
using Chess.Test;

class Program {
    static void Main(string[] args) {
        Test.TestPosition("");

        // Raylib.SetTraceLogLevel(TraceLogLevel.None);
        // Raylib.InitWindow(Theme.ScreenWidth, Theme.ScreenHeight, "Chess by Andria Beridze");
        // Raylib.SetTargetFPS(60);

        // // Default game: Human vs Bot | Initial position | Board visible from white's perspective
        // Game game = new Game(new Human(true), new Bot(false), "", true);

        // while (!Raylib.WindowShouldClose()) {
        //     Raylib.BeginDrawing();
        //     Raylib.ClearBackground(Theme.BackgroundColor);

        //     game.Update();
        //     game.Render();

        //     Raylib.EndDrawing();
        // }
    }
}