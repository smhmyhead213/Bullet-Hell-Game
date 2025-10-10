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
using System.Runtime.CompilerServices;
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
            float additionalAngleSoClawsTouch = PI / 12;

            // make arms longer so they actually reach

            float distanceToPlayer = Utilities.DistanceBetweenVectors(Owner.Position, player.Position);
            float spaceOnSideOfBoss = 200f;

            // become longer so arms bend to look like crushing the player
            float additionalScale = 1.6f;

            //End();

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

                    //BoxDrawer.DrawBox(targetPosition);

                    Arm(i).LerpToPoint(targetPosition, interpolant, false);
                    Arm(i).SetScale(MathHelper.Lerp(Arm(i).Scale(), scaleFactor, interpolant));

                    if (AITimer != preparationTime)
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
                            rotateUpperClaw(upperClawOpenAngle, preparationTime);
                            rotateLowerClaw(lowerClawOpenAngle, preparationTime);
                        }

                        // cause other claw to click twice to shoot
                        else
                        {
                            int clicks = 3;

                            int preClicksTime = 12;
                            int postClicksTime = 10;
                            int clickFrequency = (preparationTime - postClicksTime - preClicksTime) / clicks;

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

                            if (AITimer >= preClicksTime && AITimer <= preparationTime - postClicksTime)
                            {
                                int clicksDuration = preparationTime - postClicksTime - preClicksTime;
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
                            else if (AITimer >= preparationTime - postClicksTime)
                            {
                                rotateUpperClaw(upperClawOpenAngle + additionalAngleSoClawsTouch, postClicksTime);
                                rotateLowerClaw(lowerClawOpenAngle + additionalAngleSoClawsTouch, postClicksTime);
                            }
                        }
                    }

                    CrabOwner.FacePlayer();

                    if (AITimer == preparationTime)
                    {   
                        // might be awesome to put a sound effect or glint to show a lock on here?
                        SlamTargetPosition = player.Position;
                        PreSlamArmPositions[i] = Arm(i).WristPosition();

                        float initialAngle = Owner.Rotation - Arm(i).WristPosition().ToPlayer().ToAngle();
                        SlamArmPaths[i] = ChooseSlamPath(PreSlamArmPositions[i], SlamTargetPosition, initialAngle);
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

        public Func<float, Vector2> ChooseSlamPath(Vector2 initialPosition, Vector2 targetPosition, float initialAngle)
        {
            Vector2 towardsPlayerFromWrist = targetPosition - initialPosition;
            float toPlayerDistance = towardsPlayerFromWrist.Length();
            towardsPlayerFromWrist = Utilities.SafeNormalise(towardsPlayerFromWrist);

            Vector2 towardsPlayerParallel = towardsPlayerFromWrist.Rotate(initialAngle);
            Vector2 towardsPlayerLateral = towardsPlayerParallel.Rotate(-PI / 2);

            towardsPlayerParallel *= toPlayerDistance * Cos(initialAngle);
            towardsPlayerLateral *= toPlayerDistance * Sin(initialAngle);

            Func<float, Vector2> path = (x) => initialPosition + EasingFunctions.EaseInCubic(x) * towardsPlayerParallel + EasingFunctions.EaseInExpo(x) * towardsPlayerLateral;
            return path;
        }
    }
}
