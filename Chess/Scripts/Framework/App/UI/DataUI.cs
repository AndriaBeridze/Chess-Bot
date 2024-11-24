namespace Chess.App;

using Chess.API;
using Raylib_cs;
using System.Numerics;

class DataUI {
    private int depth;
    private float eval;

    private Font font;
    private const int fontSize = 80;

    public DataUI(int depth, float eval) {
        this.depth = depth;
        this.eval = eval;
        font = UIHelper.LoadFont(fontSize);
    }

    public void Update(int depth, float eval) {
        this.depth = depth;
        this.eval = eval;
    }

    public void Render() {
        int offset = 2;

        int depthTextX = (Theme.ScreenWidth / 2 - 4 * Theme.SquareSideLength - Theme.BorderSize) / 4;
        int depthTextY = Theme.ScreenHeight / 2 - fontSize - offset;
        Raylib.DrawTextEx(font, $"Depth: {depth}", new Vector2(depthTextX, depthTextY), fontSize, 1, Theme.DepthDataColor);

        int evalTextX = (Theme.ScreenWidth / 2 - 4 * Theme.SquareSideLength - Theme.BorderSize) / 4;
        int evalTextY = Theme.ScreenHeight / 2 + offset;
        string evalText = eval.ToString("F2");
        if (depth == 0) {
            evalText = "Book";
        }
        if (Math.Abs(eval) > 10000.0f) {
            int mateIn = (depth - (int) ((Math.Abs(eval) - 10000.0f) * 100)) / 2;
            evalText = $"M{ mateIn }";
        }
        Raylib.DrawTextEx(font, $"Eval: { evalText }", new Vector2(evalTextX, evalTextY), fontSize, 1, Theme.EvalDataColor);
    }
}