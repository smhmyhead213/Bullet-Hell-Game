using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabLaser : CrabBossAttack
    {
        public Vector2 TargetPos;
        public CrabLaser(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            // decide how far to stretch arms. the length includes the claw but the following claculations ignore it, this will be the source of any issues where the claws lie past the target position.
            float lengthOfLeg = Leg(0).Length();

            // decide on a target. if the target it out of reach, choose a new target in the same direction that's reachable
            Vector2 targetPosition = Leg(0).Position + 0.2f * (player.Position - Leg(0).Position); // change this to make bend testing easier

            if (Utilities.DistanceBetweenVectors(Leg(0).Position, targetPosition) > lengthOfLeg)
            {
                // get the direction vector
                Vector2 direction = Utilities.SafeNormalise(targetPosition - Leg(0).Position);
                targetPosition = Leg(0).Position + lengthOfLeg * direction;
            }

            TargetPos = targetPosition;

            float upperArmLength = Leg(0).UpperArm.Length();
            float lowerArmLength = Leg(0).LowerArm.Length() + Leg(0).UpperClaw.Length();

            // when the floating point is imprecise!!!!!!!
            float distance = Round(Utilities.DistanceBetweenVectors(Leg(0).Position, targetPosition));

            // cosine rule
            float upperArmSquared = Pow(upperArmLength, 2);
            float lowerArmSquared = Pow(lowerArmLength, 2);
            float distanceSquared = Pow(distance, 2);

            float upperArmAngle = Acos((upperArmSquared + distanceSquared - lowerArmSquared) / (2f * upperArmLength * distance));
            float lowerArmAngle = Acos((lowerArmSquared + distanceSquared - upperArmSquared) / (2f * lowerArmLength * distance));

            Leg(0).UpperArm.PointInDirection(Owner.Rotation + PI + upperArmAngle);
            Leg(0).LowerArm.PointInDirection(Owner.Rotation + PI + lowerArmAngle);
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabFlail(CrabOwner, 0);
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            Drawing.BetterDraw("box", TargetPos, null, Color.Red, 0f, Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
