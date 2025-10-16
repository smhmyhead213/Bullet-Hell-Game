using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks.DoubleArmSlam
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
            int PreparationTime = 60;
            int anticipationTime = 15;
            int slamDuration = 20;
            int idleTimeAtEnd = 15;

            float upperClawOpenAngle = CrabBoss.UpperClawOpenAngle;
            float lowerClawOpenAngle = CrabBoss.LowerClawOpenAngle;
            float additionalAngleSoClawsTouch = CrabBoss.AngleToCloseClaw;

            // make arms longer so they actually reach

            float distanceToPlayer = Utilities.DistanceBetweenVectors(Owner.Position, player.Position);
            float spaceOnSideOfBoss = 200f;

            // become longer so arms bend to look like crushing the player
            float additionalScale = 1.6f;

            //End();

            for (int i = 0; i < 2; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);

                if (AITimer <= PreparationTime)
                {
                    float interpolant = (float)AITimer / PreparationTime;
                    float arcOutLength = 0f;
                    Func<float, float> arc = EasingFunctions.EaseOutCubic;

                    float arcValue = arc(interpolant);
                    float arcOut = MathHelper.Lerp(0f, arcOutLength, arc(interpolant));

                    //float yPos = Arm(i).Position.Y
                    Vector2 targetPosition = Arm(i).Position + new Vector2(expandedi * (arcOut + spaceOnSideOfBoss), 100).Rotate(Owner.Rotation);

                    //BoxDrawer.DrawBox(targetPosition);

                    Arm(i).LerpToPoint(targetPosition, interpolant, false);

                    float scaleFactor = distanceToPlayer / Arm(i).OriginalLength() * additionalScale;
                    Arm(i).SetScale(MathHelper.Lerp(Arm(i).Scale(), scaleFactor, interpolant));

                    if (AITimer != PreparationTime)
                    {
                        Func<float, float, bool> rotateUpperClaw = (angle, duration) =>
                        {
                            float upperClawRotationThisFrame = expandedi * angle / duration;
                            Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);

                            return false;
                        };

                        Func<float, float, bool> rotateLowerClaw = (angle, duration) =>
                        {
                            float lowerClawRotationThisFrame = expandedi * angle / duration;
                            Arm(i).LowerClaw.Rotate(-lowerClawRotationThisFrame);

                            return false;
                        };

                        // do one claw without clicking
                        if (i == 0)
                        {
                            rotateUpperClaw(upperClawOpenAngle, PreparationTime);
                            rotateLowerClaw(lowerClawOpenAngle, PreparationTime);
                        }

                        // cause other claw to click twice to shoot
                        else
                        {
                            int clicks = 3;

                            int preClicksTime = 12;
                            int postClicksTime = 10;
                            int clickFrequency = (PreparationTime - postClicksTime - preClicksTime) / clicks;

                            Func<float, float>[] clickEasings = new Func<float, float>[2 * clicks];

                            int numPoints = 2 * clicks + 1;
                            float[] lerpEndPoints = new float[numPoints];
                            float[] progressValues = new float[numPoints];

                            int upperLimit = clicks * 2;

                            for (int j = 0; j < upperLimit; j += 2)
                            {
                                clickEasings[j] = EasingFunctions.EaseOutSin;
                                clickEasings[j + 1] = EasingFunctions.EaseOutSin;

                                lerpEndPoints[j] = 0f;
                                lerpEndPoints[j + 1] = 1f;

                                progressValues[j] = j / (float)upperLimit;
                                progressValues[j + 1] = (j + 1) / (float)upperLimit;                              
                            }

                            lerpEndPoints[upperLimit] = 0f;
                            progressValues[upperLimit] = 1f;

                            Func<float, float> clawClickingFunction = EasingFunctions.JoinedCurves(clickEasings, progressValues, lerpEndPoints);

                            if (AITimer >= preClicksTime && AITimer <= PreparationTime - postClicksTime)
                            {
                                int clicksDuration = PreparationTime - postClicksTime - preClicksTime;
                                int clicksTime = AITimer - preClicksTime;
                                float clicksProgress = clicksTime / (float)clicksDuration;

                                Vector2 clawDimensions = Arm(i).LowerClaw.GetSize();
                                //clawDimensions.X *= -1;
                                Vector2 clawEndPoint = Arm(i).LowerClaw.CalculateEnd();// + clawDimensions.Rotate(Owner.Rotation);
                                //BoxDrawer.DrawBox(clawEndPoint);

                                if (clicksTime % clickFrequency == 0)
                                { 
                                    Vector2 spawnPos = clawEndPoint;
                                    Projectile p = SpawnProjectile(spawnPos, 15f * Utilities.SafeNormalise(spawnPos.ToPlayer()), 1f, 1, "box", Vector2.One, Owner, true, false, Color.Red, true, false);
                                    p.AddTrail(14);
                                    p.Rotation = p.Position.ToPlayer().ToAngle();

                                    p.SetExtraAI(new Action(() =>
                                    {
                                        p.ExponentialAccelerate(1.05f);
                                    }));
                                }

                                float clickInterpolant = clawClickingFunction(clicksProgress);
                                Arm(i).LowerClaw.RotationToAdd = MathHelper.Lerp(expandedi * additionalAngleSoClawsTouch, -expandedi * lowerClawOpenAngle, clickInterpolant);
                                Arm(i).UpperClaw.RotationToAdd = MathHelper.Lerp(-expandedi * additionalAngleSoClawsTouch, expandedi * upperClawOpenAngle, clickInterpolant);
                            }
                            else if (AITimer >= PreparationTime - postClicksTime)
                            {
                                rotateUpperClaw(upperClawOpenAngle + additionalAngleSoClawsTouch, postClicksTime);
                                rotateLowerClaw(lowerClawOpenAngle + additionalAngleSoClawsTouch, postClicksTime);
                            }
                        }
                    }

                    CrabOwner.FacePlayer();                    
                }

                if (AITimer == PreparationTime)
                {
                    // might be awesome to put a sound effect or glint to show a lock on here?
                    SlamTargetPosition = player.Position;
                    PreSlamArmPositions[i] = Arm(i).WristPosition();

                    float initialAngle = Owner.Rotation - Arm(i).WristPosition().ToPlayer().ToAngle();
                    SlamArmPaths[i] = ChooseSlamPath(PreSlamArmPositions[i], SlamTargetPosition, initialAngle, EasingFunctions.EaseInCubic, EasingFunctions.EaseInExpo);
                }

                if (AITimer >= PreparationTime && AITimer <= PreparationTime + slamDuration)
                {
                    float progress = (AITimer - PreparationTime) / (float)slamDuration;

                    Arm(i).LerpToPoint(SlamArmPaths[i](progress), 1f, false);

                    float lowerClawAfterSlamAngle = CrabBoss.LowerClawAfterSlamAngle;
                    float upperClawAfterSlamAngle = CrabBoss.UpperClawAfterSlamAngle;

                    float lowerClawRotationThisFrame = expandedi * (lowerClawAfterSlamAngle - lowerClawOpenAngle) / slamDuration;
                    float upperClawRotationThisFrame = expandedi * (upperClawAfterSlamAngle - upperClawOpenAngle) / slamDuration;
                    Arm(i).LowerClaw.Rotate(-lowerClawRotationThisFrame);
                    Arm(i).UpperClaw.Rotate(upperClawRotationThisFrame);

                    //Arm(i).UpperClaw.Opacity = MathHelper.Lerp(1f, 0f, progress);
                }
            }

            if (AITimer == PreparationTime + slamDuration)
            {
                Drawing.ScreenShake(10, 30);
            }

            if (AITimer == PreparationTime + slamDuration + idleTimeAtEnd)
            {
                End();
            }
        }

        public Func<float, Vector2> ChooseSlamPath(Vector2 initialPosition, Vector2 targetPosition, float initialAngle, Func<float, float> parallelEasing, Func<float, float> lateralEasing)
        {
            return MathsUtils.PathBetweenPoints(initialPosition, targetPosition, initialAngle, parallelEasing, lateralEasing);
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabDoubleArmSmashRepeat(CrabOwner, 0);
        }

        public override bool SelectionCondition()
        {
            return Owner.Position.Distance(player.Position) < 1000f;
        }
        public override void ExtraDraw(SpriteBatch s, int AITimer)
        {
            Drawing.BetterDraw("box", SlamTargetPosition, null, Color.Red, 0f, Vector2.One * 3f, SpriteEffects.None, 0f);
        }
    }
}
