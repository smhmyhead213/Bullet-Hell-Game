﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossAppendage : NPC
    {

        public Vector2 End; //where other limbs attach on

        public CrabLeg Leg;

        public Entity Owner;
        public CrabBossAppendage(Entity owner, CrabLeg leg, string texture)
        {
            Owner = owner;
            Leg = leg;
            Texture = Assets[texture];
            IsHarmful = true;

            PrepareNPC();
            //Rotation = Rotation + PI / 2;
        }

        public virtual void UpdateLimb()
        {
            End = CalculateEnd();
        }

        public virtual Vector2 CalculateEnd()
        {
            return Position + new Vector2(-Sin(Rotation), Cos(Rotation)) * Texture.Height;
        }

        public void Rotate(float angle)
        {
            Rotation = Rotation + angle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 originOffset = new Vector2(Texture.Width / 2, 0f);

            if (this is CrabBossUpperClaw)
            {
                originOffset = new Vector2(Texture.Width, 0);
            }

            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, originOffset, Vector2.One, SpriteEffects.None, 1f);
        }
    }
}
