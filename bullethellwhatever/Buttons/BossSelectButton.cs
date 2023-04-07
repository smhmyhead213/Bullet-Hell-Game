using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using bullethellwhatever.MainFiles;

namespace bullethellwhatever.Buttons
{
    public class BossSelectButton : Button
    {
        public GameState.Bosses BossToSpawn;
        public GameState.GameStates Destination;
        public BossSelectButton(Vector2 position, Texture2D texture, GameState.GameStates destination, GameState.Bosses boss, Vector2 scale, bool deleteNextFrame)
        {
            Position = position;
            Texture = texture;
            Scale = scale;
            BossToSpawn = boss;
            Destination = destination;
            DeleteNextFrame = deleteNextFrame;
        }

        public override void HandleClick()
        {
            GameState.Boss = BossToSpawn;
            GameState.State = Destination;

            foreach (Button button in Main.activeButtons)
            {
                //I HATE YOU I HATE YOU I HATE YOU I HATE UI I HATE UI I HATE UI
                button.DeleteNextFrame = true;
            }
        }
    }
}
