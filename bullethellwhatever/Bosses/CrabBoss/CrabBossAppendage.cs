using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossAppendage : NPC
    {

        public Vector2 End; //where other limbs attach on

        public CrabLeg Leg;

        public Entity Owner;

        public int LegIndex;

        public float RotationConstant;

        public float RotationToAdd;
        public CrabBossAppendage(Entity owner, CrabLeg leg, string texture, int legIndex)
        {
            Owner = owner;
            Leg = leg;
            Texture = Assets[texture];
            Size = Vector2.One;
            IsHarmful = true;
            Damage = 1f;

            Updates = 1;

            PierceToTake = 20;

            PrepareNPC();
            LegIndex = legIndex;
            //Rotation = Rotation + PI / 2;
        }

        public override void Update()
        {
            //Rotation = Rotation + PI / 60f;
            //if (this is CrabBossUpperArm)
            Rotation = CalculateFinalRotation();

            //End = CalculateEnd();

            if (Health <= 0)
            {
                TargetableByHoming = false;
            }
            else TargetableByHoming = true;
        }

        public virtual float RotationFromV() // rotation from vertical
        {
            return RotationConstant + PI + Rotation;
        }
        public virtual Vector2 CalculateEnd()
        {
            return Position + new Vector2(-Sin(Rotation), Cos(Rotation)) * Texture.Height * Size.Y * DepthFactor();
        }

        public void Rotate(float angle)
        {
            RotationToAdd = RotationToAdd + angle;
        }

        public override void UpdateHitbox()
        {
            Vector2 centre = Vector2.Lerp(Position, CalculateEnd(), 0.5f); // centre is halfway along arm

            Hitbox.UpdateRectangle(Rotation, Texture.Width * GetSize().X, Texture.Height * GetSize().Y, centre);

            Hitbox.UpdateVertices();
        }
        public float CalculateFinalRotation()
        {
            return Owner.Rotation + RotationConstant + RotationToAdd;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 originOffset = new Vector2(Texture.Width / 2, 0f);

            if (this is CrabBossUpperClaw)
            {
                originOffset = new Vector2(Texture.Width, 0);
            }

            Drawing.Draw(Texture, Position, null, Color.White, Rotation, originOffset, GetSize(), SpriteEffects.None, 1f);

            //Hitbox.DrawHitbox();
        }
    }
}
