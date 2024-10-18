namespace Chess.App;

using Raylib_cs;

class Program {
    static void Main(string[] args) {
        Game game = new Game(new Human(true), new Bot(false));

        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(Theme.screenWidth, Theme.screenHeight, "Chess by Andria Beridze");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose()) {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Theme.backgroundColor);

            game.Render();

            Raylib.EndDrawing();
        }
    }
}