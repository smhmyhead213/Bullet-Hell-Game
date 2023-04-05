using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace bullethellwhatever
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
