using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.TelegraphLines;
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

            TelegraphLine t = new TelegraphLine(PI, 0, 0, 20, 2000, 9999, new Vector2(ScreenWidth / 2, 0), Color.White, "box", this, false);
            TelegraphLine really = new TelegraphLine(PI / 2, 0, 0, 20, 2000, 9999, new Vector2(0 , ScreenHeight / 2), Color.White, "box", this, false);
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
