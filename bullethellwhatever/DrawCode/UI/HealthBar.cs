using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode.UI
{
    public class HealthBar : UIElement
    { 
        public HealthBar()
        {
            
        }
        public static void DrawHealthBar(SpriteBatch _spriteBatch, Entity entityToDrawHPBarFor, Vector2 positionOfBar, float BarWidth, float BarHeight)
        {
            float healthRatio = entityToDrawHPBarFor.Health / entityToDrawHPBarFor.MaxHP;

            DrawHealthBar(_spriteBatch, healthRatio, positionOfBar, BarWidth, BarHeight);
        }
        public static void DrawHealthBar(SpriteBatch _spriteBatch, float progress, Vector2 positionOfBar, float BarWidth, float BarHeight)
        {
            progress = MathHelper.Clamp(progress, 0f, 1f);

            float emptySpaceOnLeft = (1 - progress) / 2; // fraction of bar width

            Texture2D texture = AssetRegistry.GetTexture2D("box");

            RotatedRectangle HPBar = new(0, BarWidth, BarHeight, positionOfBar, player);
            HPBar.UpdateVertices();

            float opacity = HPBar.Intersects(player.Hitbox).Collided ? 0.2f : 1f;

            Vector2 size = new Vector2((float)BarWidth / texture.Width, (float)BarHeight / texture.Height);

            //HP bar background.

            _spriteBatch.Draw(texture, positionOfBar, null, Color.LimeGreen * opacity, 0f, new Vector2(texture.Width / 2, texture.Height / 2), size, SpriteEffects.None, 0f);

            _spriteBatch.Draw(texture, new Vector2(positionOfBar.X - emptySpaceOnLeft * BarWidth, positionOfBar.Y), null, Color.Red * opacity, 0f, new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(size.X * progress, size.Y), SpriteEffects.None, 0f);
        }
    }
}
