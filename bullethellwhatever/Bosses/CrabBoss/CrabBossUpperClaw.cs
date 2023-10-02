using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossUpperClaw : CrabBossAppendage
    {
        public CrabBossUpperClaw(Entity owner, CrabLeg leg, string texture, int legIndex) : base(owner, leg, texture, legIndex)
        {
            MaxHP = 50f;
            Health = 50f;
            Rotation = PI / 4;
        }

        //public override Vector2 CalculateEnd()
        //{
        //    if (LegIndex == 0)
        //        return Position - new Vector2(-Sin(Rotation), Cos(Rotation)) * Texture.Height * Size.Y * DepthFactor();
        //    else return Position + new Vector2(-Sin(Rotation), Cos(Rotation)) * Texture.Height * Size.Y * DepthFactor();
        //}

        public override void UpdateHitbox()
        {
            Vector2 centre = Vector2.Lerp(Position, CalculateEnd(), 0.5f); // centre is halfway along arm

            centre.X = centre.X - Texture.Width / 2f * GetSize().X; // yeah totally sure yeah i was there yeah thats crazy man so true for real?

            Hitbox.UpdateRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, centre);

            Hitbox.UpdateVertices();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (LegIndex == 0)
            {
                Vector2 originOffset = new Vector2(Texture.Width / 2, 0f);

                if (this is CrabBossUpperClaw)
                {
                    originOffset = new Vector2(Texture.Width, 0);
                }

                Drawing.Draw(Texture, Position, null, Color.White, Rotation, originOffset, GetSize(), SpriteEffects.FlipHorizontally, 1f);
            }
            else base.Draw(spriteBatch);
        }
    }
}