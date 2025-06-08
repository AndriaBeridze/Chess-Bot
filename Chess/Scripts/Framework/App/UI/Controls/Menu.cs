using Raylib_cs;
using System.Numerics;
using Chess.Utility;

namespace Chess.UI;

class Menu {
    private List<Button> buttons = new();
    private int width = 250;
    private int height = 55;
    private int margin = 20;

    private static int fontSize = 42;
    private Font font = UIHelper.LoadFont(fontSize);

    private float transitionSpeed = 7f;

    public Menu() {
        string[] buttonTexts = { "Play as White", "Play as Black", "AI vs AI" };

        int posX = (Settings.ScreenWidth / 2 - 4 * Settings.SquareSideLength - Settings.BorderSize) / 2 - width / 2;
        int posY = Settings.ScreenHeight / 2 + 4 * Settings.SquareSideLength - height;

        for (int i = buttonTexts.Length - 1; i >= 0; i--) {
            Button button = new(buttonTexts[i]);

            Vector2 textSize = Raylib.MeasureTextEx(font, button.Text, fontSize, 0);
            button.TextPosition = new Vector2(
                posX + width / 2 - textSize.X / 2,
                posY + height / 2 - textSize.Y / 2
            );
            button.Bounds = new Rectangle(posX, posY, width, height);

            buttons.Add(button);
            posY -= height + margin;
        }
    }

    public void Render() {
        float delta = Raylib.GetFrameTime();

        foreach (var button in buttons) {
            // Smooth hover interpolation
            if (button.IsHovered) {
                button.HoverLerp += delta * transitionSpeed;
                if (button.HoverLerp > 1f) button.HoverLerp = 1f;
            } else {
                button.HoverLerp -= delta * transitionSpeed;
                if (button.HoverLerp < 0f) button.HoverLerp = 0f;
            }

            Color background = LerpColor(Theme.ButtonColor, Theme.ButtonHoverColor, button.HoverLerp);
            Raylib.DrawRectangleRec(button.Bounds, background);

            Color textColor = LerpColor(Theme.ButtonTextColor, Theme.ButtonHoverTextColor, button.HoverLerp);
            Raylib.DrawTextEx(font, button.Text, button.TextPosition, fontSize, 0, textColor);
        }
    }

    public int Update() {
        Vector2 mousePos = Raylib.GetMousePosition();
        int clicked = -1;

        for (int i = 0; i < buttons.Count; i++) {
            var button = buttons[i];
            button.IsHovered = Raylib.CheckCollisionPointRec(mousePos, button.Bounds);

            if (button.IsHovered && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                clicked = 2 - i;
            }
        }

        return clicked;
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
