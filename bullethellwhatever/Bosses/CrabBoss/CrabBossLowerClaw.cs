﻿using bullethellwhatever.BaseClasses;
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
    public class CrabBossLowerClaw : CrabBossAppendage
    {
        public CrabBossLowerClaw(Entity owner, CrabLeg leg, string texture, int legIndex) : base(owner, leg, texture, legIndex)
        {
            //MaxHP = 35f;
            //Health = MaxHP;
        }

        //public override Vector2 CalculateEnd()
        //{
        //    if (LegIndex == 0)
        //        return Position - new Vector2(-Sin(Rotation), Cos(Rotation)) * Texture.Height * Size.Y * DepthFactor();
        //    else return Position + new Vector2(-Sin(Rotation), Cos(Rotation)) * Texture.Height * Size.Y * DepthFactor();
        //}

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (LegIndex == 0)
            {
                Vector2 originOffset = new Vector2(Texture.Width / 2, 0f);

                Drawing.BetterDraw(Texture, Position, null, Colour, Rotation, GetSize(), SpriteEffects.FlipHorizontally, 1f, originOffset);
            }

            else base.Draw(spriteBatch);
        }
    }
}