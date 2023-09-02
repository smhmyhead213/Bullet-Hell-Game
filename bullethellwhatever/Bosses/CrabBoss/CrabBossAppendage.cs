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

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossAppendage : NPC
    {

        public Vector2 End; //where other limbs attach on

        public CrabLeg Leg;

        public Entity Owner;

        public float RotationConstant;

        public float RotationToAdd;
        public CrabBossAppendage(Entity owner, CrabLeg leg, string texture)
        {
            Owner = owner;
            Leg = leg;
            Texture = Assets[texture];
            IsHarmful = true;

            PierceToTake = 20;

            PrepareNPC();
            //Rotation = Rotation + PI / 2;
        }

        public virtual void UpdateLimb()
        {
            //Rotation = Rotation + PI / 60f;
            //if (this is CrabBossUpperArm)
            Rotation = CalculateFinalRotation(); 
            
            End = CalculateEnd();
        }

        public virtual float RotationFromV() // rotation from vertical
        {
            return RotationConstant + PI + Rotation;
        }
        public virtual Vector2 CalculateEnd()
        {
            return Position + new Vector2(-Sin(Rotation), Cos(Rotation)) * Texture.Height;
        }

        public void Rotate(float angle)
        {
            RotationToAdd = RotationToAdd + angle;
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

            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, originOffset, Vector2.One, SpriteEffects.None, 1f);
        }
    }
}
