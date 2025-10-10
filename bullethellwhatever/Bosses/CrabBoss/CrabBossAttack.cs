
using bullethellwhatever.BaseClasses;
using bullethellwhatever.NPCs;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
