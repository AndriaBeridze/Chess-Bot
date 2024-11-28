namespace Chess.App;

using Raylib_cs;
using Chess.API;
using System.Numerics;

class TimerUI {
    public Timer Timer;
    private bool isOnTop;

    private static int fontSize = 52;

    private const int rectWidth = 200;
    private const int rectHeight = 48;

    private static int offset = 10; // There is a small offset between the timer display and the border

    private Font font = UIHelper.LoadFont(fontSize);

    private bool isWhite => isOnTop != Theme.FromWhitesView;

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
        int rectX = Theme.ScreenWidth / 2 + 4 * Theme.SquareSideLength - rectWidth;
        int rectY;
        
        int spaceBetween = Theme.ScreenHeight / 2 - (Theme.SquareSideLength * 4 + Theme.BorderSize);

        if (isOnTop) rectY = spaceBetween - offset - fontSize + 6;
        else rectY = spaceBetween + Theme.SquareSideLength * 8 + Theme.BorderSize * 2 + offset ;

        Color rectColor;
        if (Timer.IsRunning) rectColor = isWhite ? Theme.TimerColorLightActive : Theme.TimerColorDarkActive;
        else rectColor = isWhite ? Theme.TimerColorLightDisabled : Theme.TimerColorDarkDisabled;

        Raylib.DrawRectangle(rectX, rectY, rectWidth, rectHeight, rectColor);

        string text;
        // If the time is less than 1 minute, display it in seconds.deciseconds
        // Otherwise, display it in minutes:seconds
        if (Timer.Time < 60000000) {
            text = $"{ Timer.Time / 1000000:D2}.{ Timer.Time % 1000000 / 100000 }";
        } else {
            text = $"{ Timer.Time / 60000000:D2}:{ Timer.Time % 60000000 / 1000000:D2}";
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