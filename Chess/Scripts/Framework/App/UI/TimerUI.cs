namespace Chess.App;

using Chess.API;
using Chess.Utility;
using Raylib_cs;
using System.Numerics;

class TimerUI {
    public Timer Timer;
    private bool isOnTop;

    private static int fontSize = 52;

    private const int rectWidth = 200;
    private const int rectHeight = 48;

    private static int offset = 10; // There is a small offset between the timer display and the border

    private Font font = UIHelper.LoadFont(fontSize);

    private bool isWhite => isOnTop != Settings.FromWhitesView;

    public TimerUI(Timer timer, bool isOnTop) {
        this.Timer = timer;
        this.isOnTop = isOnTop;
    }

    public void Update() {
        Timer.Update();
    }

    public void Start() => Timer.Start();

    public void Stop() => Timer.Stop();

    public void Render() {
        int rectX = Settings.ScreenWidth / 2 + 4 * Settings.SquareSideLength - rectWidth;
        int rectY;
        
        int spaceBetween = Settings.ScreenHeight / 2 - (Settings.SquareSideLength * 4 + Settings.BorderSize);

        if (isOnTop) rectY = spaceBetween - offset - fontSize + 6;
        else rectY = spaceBetween + Settings.SquareSideLength * 8 + Settings.BorderSize * 2 + offset ;

        Color rectColor;
        if (Timer.IsRunning) rectColor = isWhite ? Theme.TimerColorLightActive : Theme.TimerColorDarkActive;
        else rectColor = isWhite ? Theme.TimerColorLightDisabled : Theme.TimerColorDarkDisabled;

        Raylib.DrawRectangle(rectX, rectY, rectWidth, rectHeight, rectColor);

        string text;
        // If the time is less than 1 minute, display it in seconds.deciseconds
        // Otherwise, display it in minutes:seconds
        int deciseconds = (int) (Timer.Time * 10);
        if (deciseconds < 600) {
            text = $"{ deciseconds / 10:D2}.{ deciseconds % 10 }";
        } else {
            text = $"{ deciseconds / 600:D2}:{ deciseconds % 600 / 10:D2}";
        }
        
        Vector2 textSize = Raylib.MeasureTextEx(font, text, fontSize, 1);
        int textX = rectX + rectWidth / 2 - (int) textSize.X / 2;
        int textY = rectY + rectHeight / 2 - (int) textSize.Y / 2 + 2;


        Color textColor;
        if (Timer.IsRunning) textColor = isWhite ? Theme.TimerColorDarkActive : Theme.TimerColorLightActive;
        else textColor = isWhite ? Theme.TimerColorDarkDisabled : Theme.TimerColorLightDisabled;

        Raylib.DrawTextEx(font, text, new Vector2(textX, textY), fontSize, 1, textColor);
    }
}