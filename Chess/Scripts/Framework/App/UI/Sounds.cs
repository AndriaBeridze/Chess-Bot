namespace Chess.App;

using Raylib_cs;

class Sounds {
    public static Sound capture = Raylib.LoadSound("Chess\\Resources\\Sounds\\capture.mp3");
    public static Sound castle = Raylib.LoadSound("Chess\\Resources\\Sounds\\castle.mp3");
    public static Sound check = Raylib.LoadSound("Chess\\Resources\\Sounds\\move-check.mp3");
    public static Sound gameOver = Raylib.LoadSound("Chess\\Resources\\Sounds\\game-end.mp3");
    public static Sound move = Raylib.LoadSound("Chess\\Resources\\Sounds\\move-self.mp3");
    public static Sound promotion = Raylib.LoadSound("Chess\\Resources\\Sounds\\promote.mp3");
    public static Sound illegal = Raylib.LoadSound("Chess\\Resources\\Sounds\\illegal.mp3");
    public static Sound tenSeconds = Raylib.LoadSound("Chess\\Resources\\Sounds\\tenseconds.mp3");

    private static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>{
        {"capture", capture},
        {"castle", castle},
        {"check", check},
        {"game-over", gameOver},
        {"move", move},
        {"promotion", promotion},
        {"illegal", illegal},
        {"ten-seconds", tenSeconds}
    };

    public static void Play(string sound) {
        Raylib.PlaySound(sounds[sound]);
    }
}