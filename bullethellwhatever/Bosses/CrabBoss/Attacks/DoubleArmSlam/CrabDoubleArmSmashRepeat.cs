using bullethellwhatever.DrawCode;
using bullethellwhatever.UtilitySystems;
using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks.DoubleArmSlam
{
    public class CrabDoubleArmSmashRepeat : CrabBossAttack
    {
        public Vector2 SlamTargetPosition;
        public Vector2[] PreSlamArmPositions;
        public Func<float, Vector2>[] SlamArmPaths;

        // some of these probabaly arent necessary with keyframe system but i cant be bothered
        public float[] UpperClawsInitialRotations;
        public float[] LowerClawsInitialRotations;

        public float[] UpperClawsTargetRotations;
        public float[] LowerClawsTargetRotations;

        public int PreparationTime = 40;
        public int SlamDuration = 20;
        public int Repetition = 1;
        public static int MaxRepititions = 3;
        public CrabDoubleArmSmashRepeat(CrabBoss owner, int repetitions) : base(owner)
        {
            SlamTargetPosition = Vector2.Zero;
            PreSlamArmPositions = new Vector2[2];
            SlamArmPaths = new Func<float, Vector2>[2];

            UpperClawsInitialRotations = new float[2];
            LowerClawsInitialRotations = new float[2];

            UpperClawsTargetRotations = new float[2];
            LowerClawsTargetRotations = new float[2];

            for (int i = 0; i < 2; i++)
            {
                UpperClawsInitialRotations[i] = Arm(i).UpperClaw.RotationToAdd;
                LowerClawsInitialRotations[i] = Arm(i).LowerClaw.RotationToAdd;
            }

            Repetition = repetitions;
        }

        public override void Execute(int AITimer)
        {
            float distanceToPlayer = Utilities.DistanceBetweenVectors(player.Position, Owner.Position);

            float additionalScale = 1.6f;

            float scaleFactor = (SlamTargetPosition - Owner.Position).Length() / Arm(0).OriginalLength() * additionalScale;

            // make the boss move slightly forward with each additional slam. also need to adjust the final slam position to account for this
            float distanceToShiftForwards = 100f;

            float minimumTargetDistance = 500f;
            float maximumTargetDistance = 700f;

            for (int i = 0; i < 2; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);

                if (AITimer == 0)
                {
                    // lock on to player position early to avoid cheap hit

                    float moveOutwardAngle = PI / 3;
                    float swingOutwards = distanceToPlayer * 0.6f;

                    float distanceFromPlayerToMoveTo = distanceToPlayer * 0.9f;
                    Vector2 targetPosition = Owner.Position + Max(minimumTargetDistance, distanceFromPlayerToMoveTo) * Utilities.SafeNormalise(player.Position - Owner.Position).Rotate(expandedi * -moveOutwardAngle);
                    Vector2 wristPosition = Arm(i).WristPosition();
                    Vector2 toTarget = targetPosition - wristPosition;

                    Func<float, float> outwardsEasing = EasingFunctions.JoinedCurves([EasingFunctions.Linear, EasingFunctions.Linear], [0f, 0.5f, 1f], [0f, 1f, 0f]);
                    SlamArmPaths[i] = (x) => wristPosition + x * toTarget;// + outwardsEasing(x) * swingOutwards * Utilities.SafeNormalise(toTarget);

                    // might be awesome to put a sound effect or glint to show a lock on here?

                    float targetDistance = Max(minimumTargetDistance, distanceToPlayer);
                    targetDistance = Min(targetDistance, maximumTargetDistance);
                    Vector2 directionToPlayer = Owner.Position.DirectionToPlayer();

                    // take into account forward shift
                    SlamTargetPosition = Owner.Position + (targetDistance + distanceToShiftForwards) * directionToPlayer;

                    // project future arm positions to think about what rotations will be needed for the claws so they always give the safe spot

                    Vector2 futureArmRootPosition = wristPosition + distanceToShiftForwards * directionToPlayer;
                    float[] armRotations = MathsUtils.SolveTwoPartIK(futureArmRootPosition, SlamTargetPosition, Arm(i).UpperArm.Length(), Arm(i).LowerArm.Length(), -expandedi);

                    float lowerArmFinalRotation = armRotations[1];
                    UpperClawsTargetRotations[i] = lowerArmFinalRotation;
                    LowerClawsTargetRotations[i] = PI - lowerArmFinalRotation;
                }

                if (AITimer < PreparationTime)
                {
                    float interpolant = (AITimer + 1) / (float)PreparationTime;

                    Arm(i).SetScale(MathHelper.Lerp(Arm(i).Scale(), scaleFactor, interpolant));

                    Vector2 targetPosition = SlamArmPaths[i](interpolant);
                    BoxDrawer.DrawBox(targetPosition);
                    Arm(i).LerpToPoint(targetPosition, 1f, false);

                    float upperClawRotationThisFrame = (-UpperClawsInitialRotations[i] + expandedi * CrabBoss.UpperClawOpenAngle) / PreparationTime;
                    Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);

                    float lowerClawRotationThisFrame = (-LowerClawsInitialRotations[i] - expandedi * CrabBoss.LowerClawOpenAngle) / PreparationTime;
                    Arm(i).LowerClaw.Rotate(lowerClawRotationThisFrame);

                    CrabOwner.LerpToFacePlayer();
                }

                if (AITimer == PreparationTime)
                {
                    PreSlamArmPositions[i] = Arm(i).WristPosition();

                    float initialAngle = Owner.Rotation - Arm(i).WristPosition().ToPlayer().ToAngle();
                    SlamArmPaths[i] = MathsUtils.PathBetweenPoints(PreSlamArmPositions[i], SlamTargetPosition, initialAngle, EasingFunctions.EaseInCubic, EasingFunctions.EaseInExpo);
                }

                if (AITimer >= PreparationTime && AITimer <= PreparationTime + SlamDuration)
                {
                    int framesDone = AITimer - PreparationTime;
                    float progress = framesDone / (float)SlamDuration;

                    if (AITimer != PreparationTime + SlamDuration)
                    {
                        Owner.Velocity = Owner.Position.DirectionTo(SlamTargetPosition) * distanceToShiftForwards * EasingFunctions.EasingNextFrameDiff(EasingFunctions.EaseOutSin, framesDone, SlamDuration);
                        CreateTerribleSpeedEffect();

                        // future: maybe write code that makes the claws always orient so that theres a safe zone regardless of arm orientation
                        float lowerClawAfterSlamAngle = CrabBoss.LowerClawAfterSlamAngle;
                        float upperClawAfterSlamAngle = CrabBoss.UpperClawAfterSlamAngle;

                        float lowerClawRotationThisFrame = expandedi * (lowerClawAfterSlamAngle - CrabBoss.LowerClawOpenAngle) / SlamDuration;
                        float upperClawRotationThisFrame = expandedi * (upperClawAfterSlamAngle - CrabBoss.UpperClawOpenAngle) / SlamDuration;

                        Arm(i).LowerClaw.Rotate(-lowerClawRotationThisFrame);
                        Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);
                    }

                    Arm(i).LerpToPoint(SlamArmPaths[i](progress), 1f, false);
                }

                if (AITimer == PreparationTime + SlamDuration)
                {
                    Drawing.ScreenShake(10, 30);
                }

                int endLag = 10;

                if (AITimer == PreparationTime + SlamDuration + endLag)
                {
                    End();
                }
            }
        }

        public override BossAttack PickNextAttack()
        {
            if (Repetition != MaxRepititions - 1)
                return new CrabDoubleArmSmashRepeat(CrabOwner, Repetition + 1);
            else
            {
                return new CrabSpray(CrabOwner);
            }
        }
        public override void ExtraDraw(SpriteBatch s, int AITimer)
        {
            Drawing.BetterDraw("box", SlamTargetPosition, null, Color.Red, 0f, Vector2.One * 3f, SpriteEffects.None, 0f);

            for (int i = 0; i < SlamArmPaths.Length; i++)
            {
                int points = 20;

                for (int j = 0; j <= points; j++)
                {
                    if (SlamArmPaths is not null && SlamArmPaths[i] is not null)
                        Drawing.DrawBox(SlamArmPaths[i](j / (float)points), Color.Red, 1f);
                }
            }
        }
    }
}
