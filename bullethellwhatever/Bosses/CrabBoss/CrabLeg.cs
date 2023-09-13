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

        public bool HorizontalFlip;

        public float Health;
        public CrabLeg(Vector2 position, Entity owner)
        {
            Owner = owner;

            UpperArm = new CrabBossUpperArm(Owner, this, "CrabUpperArm");
            LowerArm = new CrabBossLowerArm(Owner, this, "CrabLowerArm");
            UpperClaw = new CrabBossUpperClaw(Owner, this, "CrabUpperClaw");
            LowerClaw = new CrabBossLowerClaw(Owner, this, "CrabLowerClaw");

            HorizontalFlip = false;

            Position = position;
            UpperArm.Position = Position;
            LowerArm.Position = UpperArm.CalculateEnd();
            UpperClaw.Position = LowerArm.CalculateEnd();
            LowerClaw.Position = LowerArm.CalculateEnd();
        }

        public float Length()
        {
            return UpperArm.Texture.Height * UpperArm.Size.Y + LowerArm.Texture.Height * LowerArm.Size.Y + UpperClaw.Texture.Height * UpperClaw.Size.Y + LowerClaw.Texture.Height * LowerClaw.Size.Y;
        }
        public void ResetRotations()
        {
            UpperArm.RotationToAdd = 0f;
            LowerArm.RotationToAdd = 0f;
            UpperClaw.RotationToAdd = 0f;
            LowerClaw.RotationToAdd = 0f;
        }

        public void DrawHitboxes()
        {
            UpperArm.Hitbox.DrawHitbox();
            LowerArm.Hitbox.DrawHitbox();
            UpperClaw.Hitbox.DrawHitbox();
            LowerClaw.Hitbox.DrawHitbox();
        }
        public void Update() 
        {
            UpperArm.Position = Position;
            //UpperArm.Rotation = UpperArm.Rotation + PI / 90f;
            UpperArm.UpdateLimb();

            LowerArm.Position = UpperArm.End;
            //LowerArm.Rotation = UpperArm.Rotation; //comment this out later just for test
            //LowerArm.Position = new Vector2(ScreenWidth / 1.5f, ScreenHeight / 2);
            //LowerArm.Rotation = LowerArm.Rotation + PI / 100f;
            LowerArm.UpdateLimb();

            UpperClaw.Position = LowerArm.End;
            //UpperClaw.Rotation = UpperClaw.Rotation + PI / 80f;
            UpperClaw.UpdateLimb();

            LowerClaw.Position = LowerArm.End;
            LowerClaw.UpdateLimb();
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
