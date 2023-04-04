using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever;

public class Button
{
    public bool DeleteNextFrame;
    public Vector2 Position;
    public Vector2 Scale; //how muhc the button is scaled up when drawn
    public Texture2D Texture;

    public Rectangle ButtonRectangle => new((int)Position.X - Texture.Width / 2 * (int)Scale.X,
        (int)Position.Y - Texture.Height / 2 * (int)Scale.Y, Texture.Width * (int)Scale.X,
        Texture.Height * (int)Scale.Y);


    public bool IsButtonClicked()
    {
        var mouseState = Mouse.GetState();
        var mousePosition = new Point(mouseState.X, mouseState.Y);

        return ButtonRectangle.Contains(mousePosition) && mouseState.LeftButton == ButtonState.Pressed;
    }

    public virtual void HandleClick()
    {
    }
}