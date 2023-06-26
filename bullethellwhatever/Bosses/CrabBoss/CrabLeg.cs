using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;

namespace bullethellwhatever.Bosses.CrabBoss
{
   public class CrabLeg
    {
        public Vector2 Position;

        public Entity Owner;

        public CrabBossUpperArm UpperArm;
        public CrabBossLowerArm LowerArm;
        public CrabBossUpperClaw UpperClaw;
        public CrabBossLowerClaw LowerClaw;

        public CrabLeg(Vector2 position, Entity owner)
        {
            Owner = owner;

            UpperArm = new CrabBossUpperArm(Owner, "CrabUpperArm");
            LowerArm = new CrabBossLowerArm(Owner, "CrabLowerArm");
            UpperClaw = new CrabBossUpperClaw(Owner, "CrabUpperClaw");
            LowerClaw = new CrabBossLowerClaw(Owner, "CrabLowerClaw");

            Position = position;
            UpperArm.Position = Position;
            LowerArm.Position = UpperArm.End;
            UpperClaw.Position = LowerArm.End;
            LowerClaw.Position = LowerArm.End;
        }

        public void Update() 
        {

            UpperArm.Position = Position;
            UpperArm.Rotation = UpperArm.Rotation + PI / 18f;
            UpperArm.Update();

            LowerArm.Position = UpperArm.End;
            //LowerArm.Rotation = UpperArm.Rotation; //comment this out later just for test
            //LowerArm.Position = new Vector2(ScreenWidth / 1.5f, ScreenHeight / 2);
            LowerArm.Rotation = LowerArm.Rotation + PI / 20f;
            LowerArm.Update();

            UpperClaw.Position = LowerArm.End;
            UpperClaw.Rotation = UpperClaw.Rotation + PI / 16f;
            UpperClaw.Update();

            LowerClaw.Position = LowerArm.End;
            LowerClaw.Update();


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            UpperArm.Draw(spriteBatch);
            LowerArm.Draw(spriteBatch);
            UpperClaw.Draw(spriteBatch);
            LowerClaw.Draw(spriteBatch);
        }
    }
}
