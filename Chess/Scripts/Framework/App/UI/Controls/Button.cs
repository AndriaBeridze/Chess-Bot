using Raylib_cs;
using System.Numerics;

namespace Chess.UI;

class Button {
    public Rectangle Bounds;
    public Vector2 TextPosition;
    public string Text;
    public bool IsHovered;
    public float HoverLerp;

    public Button(string text) {
        Text = text;
        IsHovered = false;
        HoverLerp = 0f;
    }
}