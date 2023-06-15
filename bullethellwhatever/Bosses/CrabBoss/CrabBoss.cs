using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBoss : Boss
    {
        public CrabLeg[] Legs;
        public CrabBoss()
        {
            Texture = Main.Assets["CrabBody"];
            Size = Vector2.One * 1.5f;
            Position = Utilities.CentreOfScreen();
            MaxHP = 200;
            Health = MaxHP;

            Legs = new CrabLeg[2];

            for (int i = 0; i < 2 ; i++)
            {
                Legs[i] = new CrabLeg(Position, this);
            }
        }
        public override void AI()
        {
            foreach (CrabLeg leg in Legs)
            {
                leg.Update();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //base.Draw(spriteBatch);

            foreach (CrabLeg leg in Legs)
            {
                leg.Draw(spriteBatch);
            }
        }
    }
}
