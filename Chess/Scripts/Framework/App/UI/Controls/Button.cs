namespace Chess.UI;

using Raylib_cs;
using System.Numerics;

class Button {
    public Rectangle Bounds;
    public Vector2 TextPosition;
    public string Text;
    public bool IsHovered;
    public float HoverLerp;

    private static int fontSize = 42;
    private static Font font = UIHelper.LoadFont(fontSize);

    public Button(string text, int x, int y, int width, int height) {
        Text = text;
        IsHovered = false;
        HoverLerp = 0f;
        Bounds = new Rectangle(x, y, width, height);

        Vector2 textSize = Raylib.MeasureTextEx(font, text, fontSize, 0);
        TextPosition = new Vector2(
            x + width / 2 - textSize.X / 2,
            y + height / 2 - textSize.Y / 2
        );
    }

    public void UpdateHover(Vector2 mousePos) {
        IsHovered = Raylib.CheckCollisionPointRec(mousePos, Bounds);
    }

    public bool WasClicked() {
        return IsHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);
    }

    public void Render(Color baseColor, Color hoverColor, Color textColor, Color hoverTextColor) {
        float delta = Raylib.GetFrameTime();
        float transitionSpeed = 7f;

        if (IsHovered) {
            HoverLerp += delta * transitionSpeed;
            if (HoverLerp > 1f) HoverLerp = 1f;
        } else {
            HoverLerp -= delta * transitionSpeed;
            if (HoverLerp < 0f) HoverLerp = 0f;
        }

        Color background = LerpColor(baseColor, hoverColor, HoverLerp);
        Color fgColor = LerpColor(textColor, hoverTextColor, HoverLerp);

        Raylib.DrawRectangleRec(Bounds, background);
        Raylib.DrawTextEx(font, Text, TextPosition, fontSize, 0, fgColor);
    }

    private Color LerpColor(Color a, Color b, float t) {
        return new Color(
            (byte)(a.R + t * (b.R - a.R)),
            (byte)(a.G + t * (b.G - a.G)),
            (byte)(a.B + t * (b.B - a.B)),
            (byte)(a.A + t * (b.A - a.A))
        );
    }
}
