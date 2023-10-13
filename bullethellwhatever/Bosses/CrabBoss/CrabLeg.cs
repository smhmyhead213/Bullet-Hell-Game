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
        public Vector2 Velocity;

        public Entity Owner;

        public bool Dead;

        //public bool LockPosition;

        public CrabBossUpperArm UpperArm;
        public CrabBossLowerArm LowerArm;
        public CrabBossUpperClaw UpperClaw;
        public CrabBossLowerClaw LowerClaw;

        public bool HorizontalFlip;
        public CrabLeg(Vector2 position, Entity owner, int legIndex)
        {
            Owner = owner;

            UpperArm = new CrabBossUpperArm(Owner, this, "CrabUpperArm", legIndex);
            LowerArm = new CrabBossLowerArm(Owner, this, "CrabLowerArm", legIndex);
            UpperClaw = new CrabBossUpperClaw(Owner, this, "CrabUpperClaw", legIndex);
            LowerClaw = new CrabBossLowerClaw(Owner, this, "CrabLowerClaw", legIndex);

            HorizontalFlip = false;

            Position = position;
            UpperArm.Position = Position;
            LowerArm.Position = UpperArm.CalculateEnd();
            UpperClaw.Position = LowerArm.CalculateEnd();
            LowerClaw.Position = LowerArm.CalculateEnd();

            Dead = false;
        }

        public float Length()
        {
            return UpperArm.Texture.Height * UpperArm.GetSize().Y + LowerArm.Texture.Height * LowerArm.GetSize().Y + UpperClaw.Texture.Height * UpperClaw.GetSize().Y + LowerClaw.Texture.Height * LowerClaw.GetSize().Y;
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
            Position = Position + Velocity;

            UpperArm.Position = Position;

            LowerArm.Position = UpperArm.CalculateEnd();

            LowerClaw.Position = LowerArm.CalculateEnd();

            if (UpperClaw.LegIndex == 0)
            {
                UpperClaw.Position = LowerArm.CalculateEnd();
                UpperClaw.Position = UpperClaw.Position + Utilities.RotateVectorClockwise(new Vector2(UpperClaw.Texture.Width * 1f * UpperClaw.DepthFactor() * UpperClaw.GetSize().X, 0f), UpperClaw.Rotation);
            }
            else UpperClaw.Position = LowerArm.CalculateEnd();

            if (UpperArm.Health <= 0 && LowerArm.Health <= 0 && UpperClaw.Health <= 0 && LowerClaw.Health <= 0)
            {
                Dead = true;
            }
        }
    }
}
