using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabDoubleArmSmash : CrabBossAttack
    {
        public Vector2 SlamTargetPosition;
        public Vector2[] PreSlamArmPositions;
        public Func<float, Vector2>[] SlamArmPaths;
        public CrabDoubleArmSmash(CrabBoss owner) : base(owner)
        {
            SlamTargetPosition = Vector2.Zero;
            PreSlamArmPositions = new Vector2[2];
            SlamArmPaths = new Func<float, Vector2>[2];
        }
        public override void Execute(int AITimer)
        {
            int preparationTime = 60;
            int anticipationTime = 15;
            int slamDuration = 20;

            float upperClawOpenAngle = PI / 5f;
            float lowerClawOpenAngle = PI / 5f;
            // make arms longer so they actually reach

            float distanceToPlayer = Utilities.DistanceBetweenVectors(Owner.Position, player.Position);
            float spaceOnSideOfBoss = 200f;

            // become longer so arms bend to look like crushing the player
            float additionalScale = 1.6f;

            for (int i = 0; i < 2; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);

                if (AITimer <= preparationTime)
                {                   
                    float scaleFactor = distanceToPlayer / Arm(i).OriginalLength() * additionalScale;

                    float interpolant = (float)AITimer / preparationTime;
                    float arcOutLength = 0f;
                    Func<float, float> arc = EasingFunctions.EaseOutCubic;

                    float arcValue = arc(interpolant);
                    float arcOut = MathHelper.Lerp(0f, arcOutLength, arc(interpolant));

                    //float yPos = Arm(i).Position.Y
                    Vector2 targetPosition = Arm(i).Position + new Vector2(expandedi * (arcOut + spaceOnSideOfBoss), 100).Rotate(Owner.Rotation);

                    BoxDrawer.DrawBox(targetPosition);

                    Arm(i).LerpToPoint(targetPosition, interpolant, false);
                    Arm(i).SetScale(MathHelper.Lerp(Arm(i).Scale(), scaleFactor, interpolant));

                    if (AITimer != preparationTime)
                    {
                        //float clawInterpolant = EasingFunctions.EasingNextFrameDiff(EasingFunctions.EaseOutCubic, AITimer, preparationTime);
                        float lowerClawRotationThisFrame = expandedi * lowerClawOpenAngle / preparationTime;
                        float upperClawRotationThisFrame = expandedi * upperClawOpenAngle / preparationTime;
                        Arm(i).LowerClaw.Rotate(-lowerClawRotationThisFrame);
                        Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);
                    }

                    CrabOwner.FacePlayer();

                    if (AITimer == preparationTime)
                    {
                        // might be awesome to put a sound effect or glint to show a lock on here?
                        SlamTargetPosition = player.Position;
                        PreSlamArmPositions[i] = Arm(i).WristPosition();

                        Vector2 towardsPlayerFromWrist = Arm(i).WristPosition().ToPlayer();
                        float toPlayerDistance = towardsPlayerFromWrist.Length();
                        towardsPlayerFromWrist = Utilities.SafeNormalise(towardsPlayerFromWrist);

                        float angle = Owner.Rotation - towardsPlayerFromWrist.ToAngle();
                        Vector2 towardsPlayerParallel = towardsPlayerFromWrist.Rotate(angle);
                        Vector2 towardsPlayerLateral = towardsPlayerParallel.Rotate(-PI / 2);

                        towardsPlayerParallel *= toPlayerDistance * Cos(angle);
                        towardsPlayerLateral *= toPlayerDistance * Sin(angle);

                        int locali = i;
                        
                        Func<float, Vector2> path = (x) => PreSlamArmPositions[locali] + EasingFunctions.EaseInCubic(x) * towardsPlayerParallel + EasingFunctions.EaseInExpo(x) * towardsPlayerLateral;
                        SlamArmPaths[i] = path;
                    }                    
                }

                //if (AITimer >= preparationTime && AITimer <= preparationTime + anticipationTime)
                //{
                //    float progress = (AITimer - preparationTime) / (float)anticipationTime;

                //    Arm(i).LerpToPoint(SlamTargetPosition, progress, false);

                //}

                if (AITimer >= preparationTime && AITimer <= preparationTime + slamDuration)
                {
                    float progress = (AITimer - preparationTime) / (float)slamDuration;

                    Arm(i).LerpToPoint(SlamArmPaths[i](progress), 1f, false);

                    float lowerClawAfterSlamAngle = PI / 2;
                    float upperClawAfterSlamAngle = PI / 2;

                    float lowerClawRotationThisFrame = expandedi * (lowerClawAfterSlamAngle - lowerClawOpenAngle) / slamDuration;
                    float upperClawRotationThisFrame = expandedi * (upperClawAfterSlamAngle - upperClawOpenAngle) / slamDuration;
                    Arm(i).LowerClaw.Rotate(-lowerClawRotationThisFrame);
                    Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);
                }
            }

            if (AITimer == preparationTime + slamDuration)
            {
                Drawing.ScreenShake(10, 30);
            }
        }
    }
}
