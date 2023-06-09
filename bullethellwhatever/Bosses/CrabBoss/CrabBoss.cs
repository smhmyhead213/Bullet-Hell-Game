﻿using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.TelegraphLines;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Policy;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBoss : Boss
    {
        public CrabLeg[] Legs;
        public float idivisior;
        public float c;
        public float ydivisor;
        public CrabBoss()
        {
            Texture = Assets["CrabBody"];
            Size = Vector2.One * 1.5f;
            Position = Utilities.CentreOfScreen();
            MaxHP = 25;
            Health = MaxHP;

            Legs = new CrabLeg[2];

            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                Legs[i] = new CrabLeg(Position + new Vector2(expandedi * Texture.Width / 1.4f, Texture.Height / 2.54f), this);
                if (i == 0)
                {
                    Legs[i].HorizontalFlip = true;
                }
            }

            TelegraphLine t = new TelegraphLine(PI, 0, 0, 20, 2000, 9999, new Vector2(ScreenWidth / 2, 0), Color.White, "box", this, false);
            TelegraphLine really = new TelegraphLine(PI / 2, 0, 0, 20, 2000, 9999, new Vector2(0 , ScreenHeight / 2), Color.White, "box", this, false);
        }
        public override void AI()
        {
            Rotation = Rotation + PI / 180f;
            for (int i = 0; i < 2; i++)
            {
                int expandedi = i * 2 - 1; // i = 0, this = -1, i = 1, this = 1

                if (Legs[i] is not null)
                {
                    Legs[i].Update();
                    Legs[i].Position = Position + Utilities.RotateVectorClockwise(new Vector2(expandedi * Texture.Width / 1.4f, Texture.Height / 2.54f), Rotation);
                }
            }

            if (Health <= 0)
            {
                IsDesperationOver = true;
                Die();
            }
        }
        public override void Die()
        {

            IsDesperationOver = true; //remove
            base.Die();

            Array.Clear(Legs);

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (CrabLeg leg in Legs)
            {
                if (leg is not null)
                {
                    if (leg.Position != Vector2.Zero)
                        leg.Draw(spriteBatch);
                }
            }
        }
    }
}
