namespace Chess.App;

using System.ComponentModel;
using System.Numerics;
using Chess.API;
using Raylib_cs;

class Buttons {
    private Rectangle buttonOneBounds;
    private Vector2 buttonOneTextPosition;
    private Rectangle buttonTwoBounds;
    private Vector2 buttonTwoTextPosition;
    private Rectangle buttonThreeBounds;
    private Vector2 buttonThreeTextPosition;

    private int width = 350;
    private int height = 75;
    private int margin = 20;

    private int buttonOneState = 0;
    private int buttonTwoState = 0;
    private int buttonThreeState = 0;

    private string buttonOneText = "AI vs AI";
    private string buttonTwoText = "Play as Black";
    private string buttonThreeText = "Play as White";

    private static int fontSize = 55;
    private Font font = UIHelper.LoadFont(fontSize);

    public Buttons() {
        int posX = (Theme.ScreenWidth / 2 - 4 * Theme.SquareSideLength - Theme.BorderSize) / 2 - width / 2;
        int posY = Theme.ScreenHeight / 2 + 4 * Theme.SquareSideLength - height;
        Vector2 textSize = Raylib.MeasureTextEx(font, buttonOneText, fontSize, 0);

        buttonOneTextPosition = new Vector2(posX + width / 2 - textSize.X / 2, posY + height / 2 - textSize.Y / 2);
        buttonOneBounds = new Rectangle(posX, posY, width, height);

        posY -= height + margin; // To create a margin between buttons
        textSize = Raylib.MeasureTextEx(font, buttonTwoText, fontSize, 0);

        buttonTwoTextPosition = new Vector2(posX + width / 2 - textSize.X / 2, posY + height / 2 - textSize.Y / 2);
        buttonTwoBounds = new Rectangle(posX, posY, width, height);

        posY -= height + margin;
        textSize = Raylib.MeasureTextEx(font, buttonThreeText, fontSize, 0);

        buttonThreeTextPosition = new Vector2(posX + width / 2 - textSize.X / 2, posY + height / 2 - textSize.Y / 2);
        buttonThreeBounds = new Rectangle(posX, posY, width, height);
    }

    public void Render() {
        if (buttonOneState == 1) {
            Raylib.DrawRectangleRec(buttonOneBounds, Theme.ButtonHoverColor);
            Raylib.DrawTextEx(font, buttonOneText, buttonOneTextPosition, fontSize, 0, Theme.ButtonHoverTextColor);
        } else {
            Raylib.DrawRectangleRec(buttonOneBounds, Theme.ButtonColor);
            Raylib.DrawTextEx(font, buttonOneText, buttonOneTextPosition, fontSize, 0, Theme.ButtonTextColor);
        }

        if (buttonTwoState == 1) {
            Raylib.DrawRectangleRec(buttonTwoBounds, Theme.ButtonHoverColor);
            Raylib.DrawTextEx(font, buttonTwoText, buttonTwoTextPosition, fontSize, 0, Theme.ButtonHoverTextColor);
        } else {
            Raylib.DrawRectangleRec(buttonTwoBounds, Theme.ButtonColor);
            Raylib.DrawTextEx(font, buttonTwoText, buttonTwoTextPosition, fontSize, 0, Theme.ButtonTextColor);
        }

        if (buttonThreeState == 1) {
            Raylib.DrawRectangleRec(buttonThreeBounds, Theme.ButtonHoverColor);
            Raylib.DrawTextEx(font, buttonThreeText, buttonThreeTextPosition, fontSize, 0, Theme.ButtonHoverTextColor);
        } else {
            Raylib.DrawRectangleRec(buttonThreeBounds, Theme.ButtonColor);
            Raylib.DrawTextEx(font, buttonThreeText, buttonThreeTextPosition, fontSize, 0, Theme.ButtonTextColor);
        }
    }

    // If there is button clicked, the value is returned and then recognized in the Game class
    public int Update() {
        Vector2 mousePos = Raylib.GetMousePosition();

        if (Raylib.CheckCollisionPointRec(mousePos, buttonOneBounds)) {
            buttonOneState = 1;

            if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                return 2;
            }
        } else {
            buttonOneState = 0;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, buttonTwoBounds)) {
            buttonTwoState = 1;

            if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                return 1;
            }
        } else {
            buttonTwoState = 0;
        }

        if (Raylib.CheckCollisionPointRec(mousePos, buttonThreeBounds)) {
            buttonThreeState = 1;

            if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                return 0;
            }
        } else {
            buttonThreeState = 0;
        }

        return -1;
    }
}