namespace Chess.App;

using Chess.API;
using Chess.Utility;
using Raylib_cs;
using System.Numerics;

class PieceUI {
    public Coord Coord;
    public float X, Y;
    private string imgURL = "Chess/Resources/Sprites/"; // Every piece image is stored in this directory
    private Texture2D texture; // Preloaded texture to avoid loading it every frame

    public PieceUI(Piece piece, Coord coord) {
        Coord = coord;
        imgURL += UIHelper.GetImageNameByPiece(piece);
        X = UIHelper.GetScreenX(coord);
        Y = UIHelper.GetScreenY(coord);

        texture = Raylib.LoadTexture(imgURL);
        Raylib.SetTextureFilter(texture, TextureFilter.Bilinear); // To avoid resolution loss when scaling
    }

    // When illegal move is played, piece goes to its original position
    public void ResetPosition() {
        X = UIHelper.GetScreenX(Coord);
        Y = UIHelper.GetScreenY(Coord);
    }

    public void Render() {
        Raylib.DrawTexturePro(
            texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            new Rectangle(X, Y, Settings.SquareSideLength, Settings.SquareSideLength),
            new Vector2(0, 0),
            0,
            Color.White
        );
    }
}