namespace Chess.UI;

using Chess.API;
using Chess.Utility;
using Raylib_cs;
using System.Numerics;

class TimerUI {
    private double time;
    private bool isRunning = true;
    private DateTime prevUpdate = DateTime.Now;

    private readonly bool isOnTop;
    private readonly Font font = UIHelper.LoadFont(52);

    private const int FontSize = 52;
    private const int RectWidth = 200;
    private const int RectHeight = 48;
    private const int Offset = 10;

    public double Time => time;

    public TimerUI(double initialTime, bool isOnTop) {
        time = initialTime;
        this.isOnTop = isOnTop;
    }

    public void Start() => isRunning = true;
    public void Stop() => isRunning = false;

    public void Update() {
        if (!isRunning) return;

        time -= (DateTime.Now - prevUpdate).TotalSeconds;
        if (time <= 0) {
            time = 0;
            isRunning = false;
            SoundManager.Play("Game-Over");
        }

        prevUpdate = DateTime.Now;
    }

    public void Render() {
        bool isWhite = isOnTop != Settings.FromWhitesView;
        bool active = isRunning;

        Color rectColor = GetColor(active, isWhite, true);
        Color textColor = GetColor(active, isWhite, false);

        int x = Settings.ScreenWidth / 2 + 4 * Settings.SquareSideLength - RectWidth;
        int space = Settings.ScreenHeight / 2 - (Settings.SquareSideLength * 4 + Settings.BorderSize);
        int y = isOnTop
            ? space - Offset - FontSize + 6
            : space + Settings.SquareSideLength * 8 + Settings.BorderSize * 2 + Offset;

        Raylib.DrawRectangle(x, y, RectWidth, RectHeight, rectColor);

        string text = FormatTime(time);
        Vector2 size = Raylib.MeasureTextEx(font, text, FontSize, 1);
        Vector2 pos = new Vector2(x + RectWidth / 2 - size.X / 2, y + RectHeight / 2 - size.Y / 2 + 2);

        Raylib.DrawTextEx(font, text, pos, FontSize, 1, textColor);
    }

    private static string FormatTime(double t) {
        int d = (int)(t * 10);
        return d < 600
            ? $"{d / 10:D2}.{d % 10}"
            : $"{d / 600:D2}:{(d / 10) % 60:D2}";
    }

    private static Color GetColor(bool active, bool white, bool isRect) {
        if (active) {
            return white
                ? (isRect ? Theme.TimerColorLightActive : Theme.TimerColorDarkActive)
                : (isRect ? Theme.TimerColorDarkActive : Theme.TimerColorLightActive);
        } else {
            return white
                ? (isRect ? Theme.TimerColorLightDisabled : Theme.TimerColorDarkDisabled)
                : (isRect ? Theme.TimerColorDarkDisabled : Theme.TimerColorLightDisabled);
        }
    }
}
