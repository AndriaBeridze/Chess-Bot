namespace Chess.UI;

using Raylib_cs;
using System.Numerics;
using Chess.Utility;

class Menu {
    private List<Button> buttons = new();
    private int width = 250;
    private int height = 55;
    private int margin = 20;

    public Menu() {
        string[] buttonTexts = { "Play as White", "Play as Black", "AI vs AI" };

        int posX = (Settings.BoardMarginLeft - Settings.BorderSize) / 2 - width / 2;
        int posY = Settings.ScreenHeight / 2 + 4 * Settings.SquareSideLength - height;

        for (int i = buttonTexts.Length - 1; i >= 0; i--) {
            Button button = new(buttonTexts[i], posX, posY, width, height);
            buttons.Add(button);
            posY -= height + margin;
        }
    }

    public void Render() {
        foreach (var button in buttons) {
            button.Render(
                Theme.ButtonColor,
                Theme.ButtonHoverColor,
                Theme.ButtonTextColor,
                Theme.ButtonHoverTextColor
            );
        }
    }

    public int Update() {
        Vector2 mousePos = Raylib.GetMousePosition();
        int clicked = -1;

        for (int i = 0; i < buttons.Count; i++) {
            var button = buttons[i];
            button.UpdateHover(mousePos);

            if (button.WasClicked()) {
                clicked = 2 - i;
            }
        }

        return clicked;
    }
}
