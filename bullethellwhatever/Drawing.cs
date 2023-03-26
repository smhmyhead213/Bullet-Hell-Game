using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever
{
    public class Drawing
    {
        public static void DrawGame()
        {
            float transparency = 4f * (1f / (Main.player.IFrames + 1f)); //to indicate iframes

            Main._spriteBatch.Draw(Main.player.Texture, Main.player.Position, null, Color.White * transparency, 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);

            Utilities.drawTextInDrawMethod(Main.activeProjectiles.Count.ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 4, Main._graphics.PreferredBackBufferHeight / 2), Main._spriteBatch, Main.font);
            Utilities.drawTextInDrawMethod(Main.player.Health.ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 6, Main._graphics.PreferredBackBufferHeight / 6), Main._spriteBatch, Main.font);
            if (Main.activeNPCs.Count > 0)
                Utilities.drawTextInDrawMethod(Main.activeNPCs[0].Health.ToString(), new Vector2(Main._graphics.PreferredBackBufferWidth / 8, Main._graphics.PreferredBackBufferHeight / 8), Main._spriteBatch, Main.font);

            foreach (NPC npc in Main.activeNPCs)
            {
                Main._spriteBatch.Draw(Main.player.Texture, npc.Position, null, npc.Colour(), 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(npc.Size, npc.Size), SpriteEffects.None, 0f); 
            }

            foreach (Projectile projectile in Main.activeProjectiles)
            {
                Main._spriteBatch.Draw(Main.player.Texture, projectile.Position, null, projectile.Colour(), 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
            }

            foreach (Projectile projectile in Main.activeFriendlyProjectiles)
            {
                Main._spriteBatch.Draw(Main.player.Texture, projectile.Position, null, projectile.Colour(), 0f, new Vector2(Main.player.Texture.Width / 2, Main.player.Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
            }
        }
    }
}
