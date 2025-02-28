using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using bullethellwhatever.Projectiles.Base;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.DrawCode;
using Microsoft.VisualBasic.Logging;
using System.Security.Policy;
using SharpDX.Direct2D1;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabProjectilePunches : CrabBossAttack
    {

        public CrabProjectilePunches(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            //return;

            int pullBackArmTime = 30;
            int punchSwingTime = 7;
            int resetTime = 27;
            int delayAfterPunchToCloseClaw = 20; // should be less than reset time
            int totalPunchTime = pullBackArmTime + punchSwingTime + resetTime;
            int attackDuration = 200;
            int endAttackTime = 40;
            float homingStrength = 0.05f;

            float armLength = Arm(0).WristLength();
            int armZeroTimer = AITimer;
            int armOneTimer = AITimer - (totalPunchTime / 2);
            float pullBackAngle = PI / 2.4f;
            float sidestepInitialSpeed = 15f;
            ref float sidestepDir = ref ExtraData[1];

            for (int i = 0; i < 2; i++)
            {
                // decide which timer to use
                int usedTimer = (i == 0 ? armZeroTimer : armOneTimer);

                // only loop if we are still to continue attacking

                if (AITimer < attackDuration)
                {
                    usedTimer = usedTimer % totalPunchTime;
                }

                int expandedi = Utilities.ExpandedIndex(i);

                if (usedTimer == 0)
                {
                    sidestepDir = Utilities.RandomSign();
                    Owner.Velocity = sidestepInitialSpeed * sidestepDir * (Utilities.AngleToPlayerFrom(Owner.Position) + PI / 2).ToVector();
                }

                if (usedTimer < pullBackArmTime && usedTimer >= 0)
                {
                    // move arm in rough circle around its start

                    float holdOutDistEnd = armLength * 0.5f;
                    float progress = (float)usedTimer / pullBackArmTime;
                    float interpolant = EasingFunctions.EaseOutExpo(progress);
                    float holdOutDistance = MathHelper.Lerp(armLength, holdOutDistEnd, interpolant);
                    Vector2 rootToEnd = holdOutDistance * Utilities.SafeNormalise(RestingPosition(i) - Arm(i).Position);
                    float armRotation = MathHelper.Lerp(0f, pullBackAngle, interpolant);
                    Vector2 targetPosition = Arm(i).Position + rootToEnd.Rotate(-expandedi * armRotation);

                    Arm(i).TouchPoint(targetPosition, false);

                    // open up the claw to shoot laser at player
                    Arm(i).LowerClaw.LerpRotation(0f, -expandedi * PI / 2, interpolant);

                    CrabOwner.FacePlayer();

                    Owner.Velocity = sidestepDir * (Utilities.AngleToPlayerFrom(Owner.Position) + PI / 2).ToVector() * MathHelper.Lerp(sidestepInitialSpeed, 0f, EasingFunctions.EaseOutQuad(1f - progress));
                }

                if (usedTimer > pullBackArmTime && usedTimer <= pullBackArmTime + punchSwingTime)
                {
                    int localTime = usedTimer - pullBackArmTime;
                    Vector2 punchTarget = Arm(i).Position + new Vector2(0f, Arm(i).WristLength()).Rotate(Owner.Rotation);
                    float interpolant = (float)localTime / punchSwingTime;

                    Arm(i).LerpToPoint(punchTarget, interpolant, false);
                }

                if (usedTimer > pullBackArmTime + punchSwingTime && usedTimer <= pullBackArmTime + punchSwingTime + resetTime)
                {
                    int localTime = usedTimer - (pullBackArmTime + punchSwingTime);
                    int rayDuration = resetTime - delayAfterPunchToCloseClaw;

                    float interpolant = (float)localTime / resetTime;
                    Arm(i).LerpToRestPosition(interpolant, false);

                    if (usedTimer == pullBackArmTime + punchSwingTime + 1)
                    {
                        float projectileSpeed = 5f;
                        float angle = Utilities.AngleToPlayerFrom(Arm(i).WristPosition());
                        Projectile p = SpawnProjectile(Arm(i).WristPosition(), angle.ToVector() * projectileSpeed, 1f, 1, "box", Vector2.One, Owner, true, false, Color.Red, true, false);
                        p.AddTrail(14);

                        p.SetExtraAI(new Action(() =>
                        {
                            p.ExponentialAccelerate(1.1f);
                            p.Velocity = Utilities.ConserveLengthLerp(p.Velocity, p.Position.ToPlayer(), homingStrength);
                            p.Rotation = p.Velocity.ToAngle();
                        }));

                        //Deathray d = SpawnDeathray(Arm(i).WristPosition(), angle, 1f, rayDuration, "box", Arm(i).UpperArm.Width(), GameWidth, 0f, false, false, Color.Red, "DeathrayShader2", Owner);
                        //d.SetThinOut(true, 5);

                        //int locali = i;

                        //d.SetExtraAI(new Action(() =>
                        //{
                        //    d.Position = Arm(locali).WristPosition();

                        //    if (d.AITimer > 3)
                        //    {
                        //        d.HarmfulToPlayer = true;
                        //    }
                        //}));
                    }

                    if (usedTimer > pullBackArmTime + punchSwingTime + delayAfterPunchToCloseClaw)
                    {
                        int localerTime = localTime - delayAfterPunchToCloseClaw;
                        int clawCloseTime = 15; // close the claws faster than the arm resets and MAKE SURE THIS IS LESS THAN RESETTIME
                        float clawCloseInterpolant = MathHelper.Clamp((float)localerTime / clawCloseTime, 0f, 1f);

                        Arm(i).LowerClaw.LerpRotation(-expandedi * PI / 2, 0f, EasingFunctions.EaseOutExpo(clawCloseInterpolant));
                    }
                }

                if (usedTimer > attackDuration)
                {
                    Arm(i).LerpToRestPosition(0.1f);
                    Arm(i).LowerClaw.LerpRotation(Arm(i).LowerClaw.CalculateFinalRotation(), 0f, 0.5f);
                }
            }

            if (AITimer == attackDuration + endAttackTime)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabNeutralState(CrabOwner);
        }
    }
}
