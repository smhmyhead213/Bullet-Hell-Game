using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever;

public class DifficultySelectButton : Button
{
    public GameState.GameStates Destination;
    public GameState.Difficulties DifficultyChange;

    public DifficultySelectButton(Vector2 position, Texture2D texture, GameState.GameStates destination,
        GameState.Difficulties difficultyChange, Vector2 scale)
    {
        Position = position;
        Texture = texture;
        Scale = scale;
        DifficultyChange = difficultyChange;
        Destination = destination;
    }

    public override void HandleClick()
    {
        GameState.Difficulty = DifficultyChange;
        GameState.State = Destination;

        foreach (var button in Main.activeButtons)
            //I HATE YOU I HATE YOU I HATE YOU I HATE UI I HATE UI I HATE UI
            button.DeleteNextFrame = true;
    }
}