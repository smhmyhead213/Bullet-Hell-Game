using bullethellwhatever.UtilitySystems;
using log4net.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks.DoubleArmSlam
{
    public class CrabDoubleArmSmashRepeat : CrabBossAttack
    {
        public Vector2 SlamTargetPosition;
        public Vector2[] PreSlamArmPositions;
        public Func<float, Vector2>[] SlamArmPaths;

        public float[] UpperClawsInitialRotations;
        public float[] LowerClawsInitialRotations;

        public int PreparationTime = 40;
        public int SlamDuration = 20;
        public CrabDoubleArmSmashRepeat(CrabBoss owner) : base(owner)
        {
            SlamTargetPosition = Vector2.Zero;
            PreSlamArmPositions = new Vector2[2];
            SlamArmPaths = new Func<float, Vector2>[2];

            UpperClawsInitialRotations = new float[2];
            LowerClawsInitialRotations = new float[2];

            for (int i = 0; i < 2; i++)
            {
                UpperClawsInitialRotations[i] = Arm(i).UpperClaw.RotationToAdd;
                LowerClawsInitialRotations[i] = Arm(i).LowerClaw.RotationToAdd;
            }           
        }

        public override void Execute(int AITimer)
        {
            float distanceToPlayer = Utilities.DistanceBetweenVectors(player.Position, Owner.Position);
            float additionalScale = 1.6f;

            for (int i = 0; i < 2; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);

                if (AITimer == 0)
                {
                    float moveOutwardAngle = PI / 3;

                    float distanceFromPlayerToMoveTo = distanceToPlayer * 0.9f;
                    Vector2 targetPosition = player.Position + distanceFromPlayerToMoveTo * Utilities.SafeNormalise(Owner.Position - player.Position).Rotate(expandedi * moveOutwardAngle);
                    SlamArmPaths[i] = MathsUtils.PathBetweenPoints(Arm(i).WristPosition(), targetPosition, -expandedi * moveOutwardAngle, EasingFunctions.EaseInCubic, EasingFunctions.EaseInQuad);
                }

                if (AITimer <= PreparationTime)
                {
                    float interpolant = AITimer / (float)PreparationTime;

                    float scaleFactor = distanceToPlayer / Arm(i).OriginalLength() * additionalScale;
                    Arm(i).SetScale(MathHelper.Lerp(Arm(i).Scale(), scaleFactor, interpolant));

                    Arm(i).LerpToPoint(SlamArmPaths[i](interpolant), 1f, false);

                    float upperClawRotationThisFrame = (-UpperClawsInitialRotations[i] + expandedi * CrabBoss.UpperClawOpenAngle) / PreparationTime;
                    Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);

                    float lowerClawRotationThisFrame = (-LowerClawsInitialRotations[i] - expandedi * CrabBoss.LowerClawOpenAngle) / PreparationTime;
                    Arm(i).LowerClaw.Rotate(lowerClawRotationThisFrame);
                }

                if (AITimer == PreparationTime)
                {
                    // might be awesome to put a sound effect or glint to show a lock on here?
                    SlamTargetPosition = player.Position;
                    PreSlamArmPositions[i] = Arm(i).WristPosition();

                    float initialAngle = Owner.Rotation - Arm(i).WristPosition().ToPlayer().ToAngle();
                    SlamArmPaths[i] = MathsUtils.PathBetweenPoints(PreSlamArmPositions[i], SlamTargetPosition, initialAngle, EasingFunctions.EaseInCubic, EasingFunctions.EaseInExpo);
                }

                if (AITimer >= PreparationTime && AITimer <= PreparationTime + SlamDuration)
                {
                    float progress = (AITimer - PreparationTime) / (float)SlamDuration;

                    Arm(i).LerpToPoint(SlamArmPaths[i](progress), 1f, false);

                    float lowerClawAfterSlamAngle = CrabBoss.LowerClawAfterSlamAngle;
                    float upperClawAfterSlamAngle = CrabBoss.UpperClawAfterSlamAngle;

                    float lowerClawRotationThisFrame = expandedi * (lowerClawAfterSlamAngle - CrabBoss.LowerClawOpenAngle) / SlamDuration;
                    float upperClawRotationThisFrame = expandedi * (upperClawAfterSlamAngle - CrabBoss.UpperClawOpenAngle) / SlamDuration;
                    Arm(i).LowerClaw.Rotate(-lowerClawRotationThisFrame);
                    Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);
                }

                int endLag = 10;

                if (AITimer == PreparationTime + SlamDuration + endLag)
                {
                    End();
                }
                //if (AITimer >= PreparationTime && AITimer <= PreparationTime + slamDuration)
                //{
                //    float progress = (AITimer - PreparationTime) / (float)slamDuration;

                //    Arm(i).LerpToPoint(SlamArmPaths[i](progress), 1f, false);

                //    float lowerClawAfterSlamAngle = PI / 2;
                //    float upperClawAfterSlamAngle = PI / 2;

                //    float lowerClawRotationThisFrame = expandedi * (lowerClawAfterSlamAngle - lowerClawOpenAngle) / slamDuration;
                //    float upperClawRotationThisFrame = expandedi * (upperClawAfterSlamAngle - upperClawOpenAngle) / slamDuration;
                //    Arm(i).LowerClaw.Rotate(-lowerClawRotationThisFrame);
                //    Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);
                //}
            }

            //if (AITimer == PreparationTime + slamDuration)
            //{
            //    Drawing.ScreenShake(10, 30);
            //}
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabDoubleArmSmashRepeat(CrabOwner);
        }
    }
}
