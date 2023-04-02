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

            foreach (Button button in Main.activeButtons)
            {
                //I HATE YOU I HATE YOU I HATE YOU I HATE UI I HATE UI I HATE UI
                button.DeleteNextFrame = true;
            }
        }
    }
}
