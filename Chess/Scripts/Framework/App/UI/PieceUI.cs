namespace Chess.App;

using Raylib_cs;
using System.Numerics;
using Chess.API;
using System.Security.Cryptography.X509Certificates;

class PieceUI {
    private string imgURL = "Chess/Resources/Sprites/";
    private Coord coord;
    private int x, y;

    public PieceUI(Piece piece, Coord coord) {
        this.coord = coord;
        imgURL += UIHelper.GetImageNameByPiece(piece);
        x = UIHelper.GetScreenX(coord);
        y = UIHelper.GetScreenY(coord);
    }

    public void Render() {
        Texture2D texture = Raylib.LoadTexture(imgURL);

        Raylib.SetTextureFilter(texture, TextureFilter.Bilinear);
        
        Raylib.DrawTexturePro(
            texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            new Rectangle(x, y, Theme.SquareSideLength, Theme.SquareSideLength),
            new Vector2(0, 0),
            0,
            Color.White
        );
    }
}