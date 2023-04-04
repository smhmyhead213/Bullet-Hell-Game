using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever;

public class TitleScreenButton : Button
{
    public GameState.GameStates Destination;

    public TitleScreenButton(Vector2 position, Texture2D texture, GameState.GameStates destination, Vector2 scale)
    {
        Position = position;
        Texture = texture;
        Scale = scale;
        Destination = destination;
    }

    public override void HandleClick()
    {
        GameState.State = Destination;

        foreach (var button in Main.activeButtons)
        {
            //I HATE YOU I HATE YOU I HATE YOU I HATE UI I HATE UI I HATE UI
            button.DeleteNextFrame = true;
            GameState.HasASettingBeenChanged = false;
        }
    }
}