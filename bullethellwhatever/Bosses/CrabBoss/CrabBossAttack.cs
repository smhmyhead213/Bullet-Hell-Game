
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode.Particles;
using bullethellwhatever.NPCs;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabBossAttack : BossAttack
    {
        public CrabBoss CrabOwner;

        public CrabBossAttack(CrabBoss owner) : base(owner) 
        {
            CrabOwner = owner;
        }
        public override void InitialiseAttackValues()
        {
            CrabOwner = (CrabBoss)Owner;
        }

        public override void ExtraAttackEnd()
        {
            CrabOwner.ResetArmRotations();
        }

        public void ChooseMainArm(int index)
        {
            Owner.ExtraData[0] = index;
        }

        public int ChosenArmIndex()
        {
            return (int)Owner.ExtraData[0];
        }
        public CrabArm ChosenArm()
        {
            return Arm((int)Owner.ExtraData[0]);
        }

        // time should be from 0 to durationTotal
        public void RotateArm(int index, float totalAngle, int time, int durationTotal, Func<float, float> easing)
        {
            float angleNextFrame = totalAngle * easing((time + 1) / (float)durationTotal);
            float angleThisFrame = totalAngle * easing(time / (float)durationTotal);

            CrabOwner.Arms[index].RotateLeg(angleNextFrame - angleThisFrame);
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabNeutralState(CrabOwner);
        }
        public override void HandleBounces()
        {
            
        }
        
        public void CreateTerribleSpeedEffect(float velocityMult = 1f)
        {
            float angleVariation = 0f * PI / 36f;
            float rotation = Owner.Velocity.ToAngle() + Utilities.RandomAngle(angleVariation);
            Vector2 spawnPos = Owner.Position + new Vector2(Owner.Width() * Utilities.RandomFloat(-0.5f, 0.5f), 0f).Rotate(rotation);
            Particle p = new Particle();
            float scaleLength = Owner.Velocity.Length() / 4f;
            p.Spawn("box", spawnPos, Vector2.Zero, Vector2.Zero, new Vector2(0.25f, scaleLength), rotation, Color.White, 1f, 6);
        }

        public Vector2 PlayerPositionInGrab(int armIndex)
        {
            float armScale = Arm(armIndex).UpperArm.GetScale().X;
            return Arm(armIndex).WristOffsetBy(new Vector2(15f * armScale, -15f * armScale));
        }

        public Vector2 RestingPosition(int armIndex)
        {
            return Arm(armIndex).RestPositionEnd();
        }
        public CrabArm Arm(int index)
        {
            return CrabOwner.Arms[index];
        }

        public CrabArm[] Arms()
        {
            return CrabOwner.Arms;
        }
    }
}
