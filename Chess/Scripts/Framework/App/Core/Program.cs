namespace Chess.Core;

using Chess.App;
using Chess.Utility;
using Chess.Testing;
using Raylib_cs;

class Program {
    static void Main(string[] args) {
        if (args.Length > 0 && args[0].ToLower() == "test") {
            Tester.Test();
            return;
        }

        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(Settings.ScreenWidth, Settings.ScreenHeight, "Chess by Andria Beridze");
        Raylib.SetTargetFPS(90);
        Raylib.InitAudioDevice();

        // Default game: Human vs Bot | Initial position | Board visible from white's view
        Game game = new Game(new HumanPlayer(true), new BotPlayer(false), "", true);

        while (!Raylib.WindowShouldClose()) {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Theme.BackgroundColor);

            game.Update();
            game.Render();

            Raylib.EndDrawing();
        }
    }
}