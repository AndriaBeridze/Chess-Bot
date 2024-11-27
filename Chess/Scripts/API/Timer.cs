namespace Chess.API;

using Chess.App;
using Raylib_cs;

class Timer {
    public int Time;
    public bool IsRunning = true;
    private DateTime prevUpdate = DateTime.Now;

    public Timer(int time) {
        Time = time;
    }

    public void Update() {
        if(IsRunning) Time -= (int) (DateTime.Now - prevUpdate).TotalMicroseconds;
        if (Time < 0) {
            Time = 0;
            IsRunning = false;
            Sounds.Play("game-over");
        }
        prevUpdate = DateTime.Now;
    }

    public void Start() => IsRunning = true;
    public void Stop() => IsRunning = false;
}