﻿namespace Chess.App;

using Raylib_cs;
using Chess.API;

class Program {
    static void Main(string[] args) {
        Game game = new Game(new Human(true), new Bot(false), true);

        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(Theme.ScreenWidth, Theme.ScreenHeight, "Chess by Andria Beridze");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose()) {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Theme.BackgroundColor);

            game.Render();

            Raylib.EndDrawing();
        }
    }
}