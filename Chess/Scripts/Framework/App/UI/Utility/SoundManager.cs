namespace Chess.UI;

using Raylib_cs;

class SoundManager {
    public static Sound capture = Raylib.LoadSound("Chess\\Resources\\Sounds\\Capture.mp3");
    public static Sound castle = Raylib.LoadSound("Chess\\Resources\\Sounds\\Castle.mp3");
    public static Sound check = Raylib.LoadSound("Chess\\Resources\\Sounds\\Check.mp3");
    public static Sound gameOver = Raylib.LoadSound("Chess\\Resources\\Sounds\\Game-Over.mp3");
    public static Sound move = Raylib.LoadSound("Chess\\Resources\\Sounds\\Move.mp3");
    public static Sound promotion = Raylib.LoadSound("Chess\\Resources\\Sounds\\Promote.mp3");
    public static Sound illegal = Raylib.LoadSound("Chess\\Resources\\Sounds\\Illegal.mp3");

    private static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>{
        {"capture", capture},
        {"castle", castle},
        {"check", check},
        {"game-over", gameOver},
        {"move", move},
        {"promotion", promotion},
        {"illegal", illegal},
        {"Capture", capture},
        {"Castle", castle},
        {"Check", check},
        {"Game-Over", gameOver},
        {"Move", move},
        {"Promotion", promotion},
        {"Illegal", illegal},
    };

    public static void Play(string sound) {
        Raylib.PlaySound(sounds[sound]);
    }
}