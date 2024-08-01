using bullethellwhatever.Projectiles.Base;

using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using bullethellwhatever.DrawCode;
using System.Threading;
using System.Drawing.Design;
using SharpDX.MediaFoundation;
using bullethellwhatever.Projectiles;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class BackgroundPunches : CrabBossAttack
    {
        public int MoveToPositionTime;
        public int PunchWindUpTime;
        public int ArmIndex;
        public int PunchTime;
        public int PunchDuration;
        public int FistRestTime;
        public int TimeAfterTargetChosen;
        public Vector2 TargetPosition;
        public float TargetSize;
        public float TargetRotation;
        public float TargetTransparency;
        public bool Targeting;
        public float UpperArmRotationAngle;
        public float LowerArmRotationAngle;
        public float ArmPullBackAngle;
        public BackgroundPunches(int endTime) : base(endTime)
        {
            EndTime = endTime;
            ArmIndex = 0;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            MoveToPositionTime = 30;
            PunchWindUpTime = 60;
            PunchTime = 210;
            TimeAfterTargetChosen = 60;
            //ArmIndex = 0;
            TargetSize = 2f;
            PunchDuration = 10;
            TargetRotation = 0;
            FistRestTime = 100;
            Targeting = false;
            TargetTransparency = 0.5f;
            //debugdepth = 0;
        }

        public void PullArmBack(int time)
        {
            int expandedi = ArmIndex * 2 - 1;
            
            float factorToMoveArms = MathHelper.Lerp(1f, 0.1f, Owner.Depth);

            float distanceToMoveX = ((Owner.Texture.Width * Owner.DepthFactor() / 2f) - Owner.Texture.Width * Owner.DepthFactor() / 1.4f) * factorToMoveArms * expandedi;
            float distanceToMoveY = Owner.Texture.Height * Owner.DepthFactor() / 2.54f * factorToMoveArms;

            CrabOwner.LockArmPositions = false;

            Leg(ArmIndex).Position = Leg(ArmIndex).Position - Utilities.RotateVectorClockwise(new Vector2(distanceToMoveX, distanceToMoveY), Owner.Rotation) / time;

            Leg(ArmIndex).UpperArm.Rotate(ArmPullBackAngle / time);
            Leg(ArmIndex).LowerArm.Rotate(ArmPullBackAngle / time);
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int totalAttackDuration = PunchTime + 2 * PunchDuration + FistRestTime + 60;
            int time = AITimer % (totalAttackDuration);

            CrabOwner.SetBoosters(false);

            float finalDepth = 0.5f;

            if (AITimer < EndTime - totalAttackDuration) // we want to fade back in near the end
            {
                if (time < MoveToPositionTime)
                {                   
                    MoveToPoint(new Vector2(IdealScreenWidth / 2f, IdealScreenHeight / 10f), time, MoveToPositionTime);

                    if (!Utilities.IsQuantityWithinARangeOfAValue(Owner.Depth, finalDepth, 0.02f)) //if looping the attack, dont bother doing the depth stuff again
                    {
                        float depth = time * (finalDepth / MoveToPositionTime);
                        CrabOwner.SetDepth(depth);
                        CrabOwner.SetArmDepth(0, depth);
                        CrabOwner.SetArmDepth(1, depth);
                    }

                    CrabOwner.LockArmPositions = true;
                }

                if (time == MoveToPositionTime)
                {
                    Owner.Velocity = Vector2.Zero;
                    CrabOwner.ResetArmRotations();
                    TargetSize = 2f;
                    ArmIndex = (int)Abs(ArmIndex - 1); //swap arms
                }

                int expandedi = ArmIndex * 2 - 1;

                ArmPullBackAngle = PI / 1.3f * -expandedi;

                if (time > MoveToPositionTime && time < MoveToPositionTime + PunchWindUpTime)
                {
                    Targeting = true;

                    TargetPosition = player.Position;
                    TargetTransparency = 0.5f;

                    PullArmBack(PunchWindUpTime + TimeAfterTargetChosen);
                }

                if (time < PunchTime - TimeAfterTargetChosen && time > MoveToPositionTime + PunchWindUpTime) // this if is redundant, merge with previous if performance becomes an issue but one extra if isnt gonna ruin everything so i cant be bothered
                {
                    TargetPosition = player.Position;

                    PullArmBack(PunchWindUpTime + TimeAfterTargetChosen);
                }

                if (time > PunchTime - TimeAfterTargetChosen && time < PunchTime)
                {
                    TargetSize = MathHelper.LerpPrecise(2f, 0.8f, (time - (PunchTime - TimeAfterTargetChosen)) / (float)TimeAfterTargetChosen);
                    TargetRotation = TargetRotation + (PI / 6f);
                    TargetTransparency = 1f;
                }

                if (time == PunchTime)
                {
                    Targeting = false;

                    CrabLeg leg = Leg(ArmIndex);
                   
                    UpperArmRotationAngle = Utilities.VectorToAngle(TargetPosition - leg.UpperArm.Position) - leg.UpperArm.RotationFromV();
                    LowerArmRotationAngle = Utilities.VectorToAngle(TargetPosition - leg.LowerArm.Position) - leg.LowerArm.RotationFromV();
                }

                if (time > PunchTime && time < PunchTime + PunchDuration)
                {
                    CrabLeg leg = Leg(ArmIndex);

                    float bendAngle = PI / 6f;

                    Leg(ArmIndex).UpperArm.Rotate((UpperArmRotationAngle - expandedi * bendAngle) / PunchDuration);
                    Leg(ArmIndex).LowerArm.Rotate((LowerArmRotationAngle + expandedi * bendAngle) / PunchDuration);

                    float punchDistance = Utilities.DistanceBetweenVectors(leg.Position, TargetPosition) / Owner.DepthFactor();
                    float halfPunchDist = punchDistance / 2f;

                    // i have reached a level of programming godhood where i can say "yeah this will probably work" and it actually works (neglecting the fact i have no idea why)

                    Vector2 maxUpperArmSize = Vector2.One * (halfPunchDist / leg.UpperArm.Texture.Height);
                    Vector2 maxLowerArmSize = Vector2.One * (halfPunchDist / leg.LowerArm.Texture.Height);

                    Leg(ArmIndex).UpperArm.Size = Vector2.LerpPrecise(Vector2.One, maxUpperArmSize, (time - PunchTime) / (float)PunchDuration); // GAHHH I HATE INTEGER DIVISION
                    Leg(ArmIndex).LowerArm.Size = Vector2.LerpPrecise(Vector2.One, maxLowerArmSize, (time - PunchTime) / (float)PunchDuration);
                    Leg(ArmIndex).UpperClaw.Size = Vector2.LerpPrecise(Vector2.One, maxLowerArmSize, (time - PunchTime) / (float)PunchDuration);
                    Leg(ArmIndex).LowerClaw.Size = Vector2.LerpPrecise(Vector2.One, maxLowerArmSize, (time - PunchTime) / (float)PunchDuration);
                }

                if (time == PunchTime + PunchDuration)
                {
                    Drawing.ScreenShake(10, 6);

                    int bombs = Utilities.ValueFromDifficulty(2, 4, 6, 6);

                    for (int i = 0; i < bombs; i++)
                    {
                        int fragments = Utilities.ValueFromDifficulty(12, 16, 20, 24);

                        Projectile explodingProjectile = SpawnProjectile(TargetPosition, 5f * Utilities.AngleToVector(i * Tau / bombs), 1f, 1, "box", Vector2.One * 3f, Owner, true, Color.Red, true, false);

                        explodingProjectile.SetExtraAI(new Action(() =>
                        {
                            if (explodingProjectile.AITimer == 120)
                            {
                                explodingProjectile.InstantlyDie();
                            }
                        }));
                        explodingProjectile.SetOnDeath(new Action(() =>
                        {
                            for (int i = 0; i < fragments; i++)
                            {
                                Projectile fragment = SpawnProjectile(explodingProjectile.Position, 3f * Utilities.RotateVectorClockwise(Utilities.SafeNormalise(Vector2.UnitY, Vector2.Zero), (MathF.PI * 2 / fragments * i)),
                                        1f, 1, "box", Vector2.One, Owner, true, Color.Red, false, false);
                            }
                        }));

                        explodingProjectile.DealDamage = false;
                    }
                }

                if (time > PunchTime + PunchDuration && time < PunchTime + PunchDuration + FistRestTime && time % 2 == 0) // projectile shooting stuff
                {
                    int spiralArms = Utilities.ValueFromDifficulty(2, 3, 3, 4);

                    for (int i = 0; i < spiralArms; i++)
                    {
                        Projectile proj = SpawnProjectile(TargetPosition, 4f * Utilities.AngleToVector(i * Tau / spiralArms + (time * PI / 45f)), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                    }
                }

                if (time <= PunchTime + 2 * PunchDuration + FistRestTime && time > PunchTime + PunchDuration + FistRestTime) // punch pullback
                {
                    CrabLeg leg = Leg(ArmIndex);

                    float bendAngle = PI / 6f;

                    // everything here happens in the same amount of time as the punch does, so we can reuse PunchDuration

                    Leg(ArmIndex).UpperArm.Rotate(-(UpperArmRotationAngle - (expandedi * bendAngle) + ArmPullBackAngle) / PunchDuration);
                    Leg(ArmIndex).LowerArm.Rotate(-(LowerArmRotationAngle + (expandedi * bendAngle) + ArmPullBackAngle) / PunchDuration);

                    float punchDistance = Utilities.DistanceBetweenVectors(leg.Position, TargetPosition) / Owner.DepthFactor();
                    float halfPunchDist = punchDistance / 2f;

                    Vector2 maxUpperArmSize = Vector2.One * (halfPunchDist / leg.UpperArm.Texture.Height);
                    Vector2 maxLowerArmSize = Vector2.One * (halfPunchDist / leg.LowerArm.Texture.Height);

                    Leg(ArmIndex).UpperArm.Size = Vector2.LerpPrecise(maxUpperArmSize, Vector2.One, (time - PunchTime - PunchDuration - FistRestTime) / (float)PunchDuration);
                    Leg(ArmIndex).LowerArm.Size = Vector2.LerpPrecise(maxLowerArmSize, Vector2.One, (time - PunchTime - PunchDuration - FistRestTime) / (float)PunchDuration);
                    Leg(ArmIndex).UpperClaw.Size = Vector2.LerpPrecise(maxLowerArmSize, Vector2.One, (time - PunchTime - PunchDuration - FistRestTime) / (float)PunchDuration);
                    Leg(ArmIndex).LowerClaw.Size = Vector2.LerpPrecise(maxLowerArmSize, Vector2.One, (time - PunchTime - PunchDuration - FistRestTime) / (float)PunchDuration);

                    //also undo the arm position adjustments

                    float factorToMoveArms = MathHelper.Lerp(1f, 0.1f, Owner.Depth);

                    //float distanceToMoveX = ((Owner.Texture.Width * Owner.DepthFactor() / 2f) - Owner.Texture.Width * Owner.DepthFactor() / 1.4f) * factorToMoveArms * expandedi;
                    //float distanceToMoveY = Owner.Texture.Height * Owner.DepthFactor() / 2.54f * factorToMoveArms;

                    float distanceToMoveX = -CrabOwner.CalculateArmPostionsRelativeToCentre(expandedi).X * factorToMoveArms / 2f;
                    float distanceToMoveY = CrabOwner.CalculateArmPostionsRelativeToCentre(expandedi).Y * factorToMoveArms;

                    CrabOwner.LockArmPositions = false;

                    // + instead of - t go backwards

                    Leg(ArmIndex).Position = Leg(ArmIndex).Position + new Vector2(distanceToMoveX, distanceToMoveY) / PunchDuration;
                }
            }
            else if (AITimer > EndTime - 30 && time != 0) // time being calculated before this means that for one tick, time will be 0 due to modulo division and AITimer will be at maximum, leading to an absurdly high depth value which messes things up
            {                
                float depth = (totalAttackDuration - time) / 30f * (finalDepth);
                CrabOwner.SetDepth(depth);
                CrabOwner.SetArmDepth(0, depth);
                CrabOwner.SetArmDepth(1, depth);

                CrabOwner.LockArmPositions = true;
            }

        }
        public override void ExtraDraw(SpriteBatch s)
        {
            if (Targeting)
            {
                Drawing.BetterDraw(AssetRegistry.GetTexture2D("TargetReticle"), TargetPosition, null, Color.White * TargetTransparency, TargetRotation, Vector2.One * TargetSize, SpriteEffects.None, 1);
            }
        }
    }
}
