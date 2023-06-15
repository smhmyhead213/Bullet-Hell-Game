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
            UpperArm.Update();
            LowerArm.Update();
            UpperClaw.Update();
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
