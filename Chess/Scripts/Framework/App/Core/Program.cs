﻿namespace Chess.App;

using Chess.Utility;
using Raylib_cs;

class Program {
    static void Main(string[] args) {
        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(Settings.ScreenWidth, Settings.ScreenHeight, "Chess by Andria Beridze");
        Raylib.SetTargetFPS(60);
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