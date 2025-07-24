using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.DrawCode;
using bullethellwhatever.DrawCode.Particles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabGrabPunish : CrabBossAttack
    {
        public const int ChargeUpTime = 60;
        public const int RayDuration = 90;
        public const int DelayTimeBeforePunish = 20;
        public const int ThrowAwayTime = 50;
        public const int ResetTime = 20;
        public CrabGrabPunish(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int armIndex = CrabBoss.GrabPunishArm;

            int moveToPositionTime = 20;
            int expandedi = Utilities.ExpandedIndex(0);
            
            int restTimeAfterBlast = 40;
            float distanceFromBodyToHold = 40f;

            if (AITimer > DelayTimeBeforePunish && AITimer < moveToPositionTime + DelayTimeBeforePunish)
            {
                int localTime = AITimer - DelayTimeBeforePunish;
                float progress = localTime / (float)(moveToPositionTime);
                Vector2 rootToTarget = player.Position - Arm(0).Position;
                float distanceRootToTarget = rootToTarget.Length();
                float theta = Acos(distanceFromBodyToHold / distanceRootToTarget);
                Vector2 target = Arm(armIndex).Position + rootToTarget.Rotate(-expandedi * theta).SetLength(distanceFromBodyToHold);

                Arm(armIndex).LerpToPoint(target, progress);
            }

            if (AITimer > DelayTimeBeforePunish && AITimer < ChargeUpTime + DelayTimeBeforePunish)
            {
                int localTime = AITimer - DelayTimeBeforePunish;
                float progress = localTime / (float)ChargeUpTime;
                int particleLifeTime = 20;
                float particleInitialSpeed = 5f;
                float spawnDistance = particleLifeTime * particleInitialSpeed;

                Particle p = new Particle();
                p.SetExtraAI(new Action(() =>
                {
                    float progressLifetime = p.AITimer / (float)particleLifeTime;
                    p.Opacity = progressLifetime;

                    if (p.AITimer == particleLifeTime)
                    {
                        p.Die();
                    }
                }));

                Vector2 wrist = Arm(armIndex).WristPosition();
                Vector2 spawnPos = wrist + spawnDistance * Utilities.RandomDirection();
                Vector2 velocity = particleInitialSpeed * Utilities.SafeNormalise(wrist - spawnPos);
                float scale = 0.4f;
                p.Spawn("box", spawnPos, velocity, Vector2.Zero, Vector2.One * scale, velocity.ToAngle(), Color.White, 1f, particleLifeTime);
                p.FadeOut = false;
            }

            if (AITimer == ChargeUpTime + DelayTimeBeforePunish)
            {
                Vector2 wristPos = Arm(armIndex).WristPosition();
                float angleToPlayer = wristPos.AngleToPlayer();
                float initialRayWidth = 200f;
                float rayThinTime = 10;
                float regularRayWidth = 60f;
                float rayPopOutMaxWidth = 90f;
                float totalRayDamage = 1f;
                float rayDamage = totalRayDamage / (RayDuration / player.MaxIFrames); // always do the same amount of damage regardless of iframes

                Deathray d = SpawnDeathray(wristPos, angleToPlayer, rayDamage, RayDuration, "box", initialRayWidth, GameWidth, 0f, true, false, Color.Red, "CrabGrabPunishLaser", Owner);
                d.SetNoiseMap("CrabScrollingBeamNoise", -0.06f);
                d.Shader.SetParameter("oscillationVariance", 0.6f);
                d.SetThinOut(true);
                d.Shader.SetParameter("opacity", 1f);

                d.SetExtraAI(new Action(() =>
                {
                    float progress = MathHelper.Clamp(d.AITimer / rayThinTime, 0f, 1f);
                    float interpolant = EasingFunctions.EaseOutExpo(progress);
                    d.Position = Arm(armIndex).WristPosition();
                    d.HitboxWidth = MathHelper.Lerp(initialRayWidth, regularRayWidth, interpolant);

                    int fadeOutTime = 25;

                    if (d.AITimer >= RayDuration - fadeOutTime)
                    {
                        int localTime = RayDuration - d.AITimer;
                        float opacityProgress = EasingFunctions.EaseOutExpo((float)localTime / fadeOutTime);
                        float opacityInterpolant = opacityProgress;
                        d.HitboxWidth = MathHelper.Lerp(regularRayWidth, 0f, 1f - opacityProgress);
                        //d.Shader.SetParameter("opacity", opacityInterpolant);
                    }

                    d.Shader.SetParameter("uTime", d.AITimer);
                    //d.Rotation = Arm(armIndex).LowerArm.RotationFromV();
                }));

                float oscillationsDownLaser = 5;
                float oscillationAmp = 0.15f;
                float waveSpeed = 50f;

                d.WidthFunction = (y, t) => {     
                    
                    float pinchFactor = MathHelper.Clamp(y * 10, 0f, 1f);
                    return EasingFunctions.Linear(pinchFactor) * d.HitboxWidth; // * (1 + oscillationAmp * Sin(oscillationsDownLaser * Tau * y - waveSpeed * t));
                };
            }

            if (AITimer > ChargeUpTime + DelayTimeBeforePunish && AITimer <= ChargeUpTime + DelayTimeBeforePunish + RayDuration)
            {
                int localTime = AITimer - (ChargeUpTime + DelayTimeBeforePunish);
                float progress = localTime / (float)(RayDuration);

                Vector2 rootToTarget = player.Position - Arm(0).Position;
                float distanceRootToTarget = rootToTarget.Length();
                float theta = Acos(distanceFromBodyToHold / distanceRootToTarget);
                Vector2 target = Arm(armIndex).Position + rootToTarget.Rotate(-expandedi * theta).SetLength(distanceFromBodyToHold);

                // push slightly away from player

                float pushAwayDistance = 10f;

                target -= Arm(armIndex).WristPosition().DirectionToPlayer() * (progress * pushAwayDistance);

                Arm(armIndex).LerpToPoint(target, progress);

                int particlesPerFrame = 1;

                for (int i = 0; i < particlesPerFrame; i++)
                {
                    Particle p = new Particle();
                    float baseSpeed = 18f;
                    float particleSpeed = baseSpeed + Utilities.RandomFloat(baseSpeed / 10f);
                    float angleVariance = PI;
                    Vector2 velocity = particleSpeed * Arm(armIndex).WristPosition().DirectionToPlayer().Rotate(Utilities.RandomAngle(angleVariance));
                    int lifetime = 20;
                    p.Spawn("box", Arm(armIndex).WristPosition(), velocity, Vector2.Zero, Vector2.One * 0.5f, velocity.ToAngle(), Color.Red, 1f, lifetime);

                    p.SetExtraAI(new Action(() =>
                    {
                        float progress = p.AITimer / (float)p.Lifetime;
                        p.Velocity = p.Velocity.SetLength(particleSpeed * EasingFunctions.EaseOutCubic(1f - progress));
                    }));
                }
            }

            //if (AITimer < ChargeUpTime + DelayTimeBeforePunish + RayDuration)
            //{
            //    // something funny happens if you swap the following two lines
            //    player.Position = PlayerPositionInGrab(CrabBoss.GrabbingArm);
            //    CrabOwner.LerpToFacePlayer();                
            //}

            if (AITimer > ChargeUpTime + DelayTimeBeforePunish + RayDuration && AITimer <= ChargeUpTime + DelayTimeBeforePunish + RayDuration + ThrowAwayTime)
            {
                int localTime = AITimer - (ChargeUpTime + DelayTimeBeforePunish + RayDuration);
                float progress = localTime / (float)ThrowAwayTime;
                float interpolant = EasingFunctions.EaseOutExpo(progress);
                Arm(CrabBoss.GrabbingArm).LerpRotation(Arm(CrabBoss.GrabbingArm).UpperArm.RotationToAdd, expandedi * PI / 2, interpolant);

                int throwTime = ThrowAwayTime / 20;

                if (localTime >= throwTime)
                {
                    float maxThrowSpeed = 20f;
                    float throwProgress = (localTime - throwTime) / (float)(ThrowAwayTime - throwTime);
                    float throwSpeedInterpolant = EasingFunctions.EaseOutExpo(1f - throwProgress);
                    player.Velocity = Utilities.SafeNormalise(Owner.Position.ToPlayer()) * maxThrowSpeed * throwSpeedInterpolant;
                }
            }

            if (AITimer == ChargeUpTime + DelayTimeBeforePunish + RayDuration + ThrowAwayTime)
            {
                player.UnlockMovement();
                Owner.ContactDamage = true;
            }

            if (AITimer > ChargeUpTime + DelayTimeBeforePunish + RayDuration + ThrowAwayTime && AITimer <= ChargeUpTime + DelayTimeBeforePunish + RayDuration + ThrowAwayTime + ResetTime)
            {
                int localTime = AITimer - (ChargeUpTime + DelayTimeBeforePunish + RayDuration + ThrowAwayTime);
                float progress = localTime / (float)ResetTime;
                float interpolant = progress;

                foreach (CrabArm arm in CrabOwner.Arms)
                {
                    arm.LerpArmToRest(interpolant);
                    float scale = MathHelper.Lerp(arm.UpperArm.Scale.X, 1f, interpolant);
                    arm.SetScale(scale);
                }
            }

            if (AITimer == ChargeUpTime + DelayTimeBeforePunish + RayDuration + ThrowAwayTime + ResetTime)
            {
                End();
            }
        }

        public override void ExtraDraw(SpriteBatch s, int AITimer)
        {
            int flashDuration = ChargeUpTime + RayDuration + DelayTimeBeforePunish;
            float maxRadius = 130f;

            if (AITimer < flashDuration)
            {
                float progress = AITimer / (float)flashDuration;

                //Func<float, float> easing = EasingFunctions.JoinedCurves([EasingFunctions.EaseInQuart, EasingFunctions.EaseOutExpo], [] )
                float opacity = EasingFunctions.Linear(progress);
                float radius = maxRadius * EasingFunctions.EaseOutQuad(progress);

                Drawing.RestartSB(s, true, true);

                Shader shader = new Shader("GlowShader", Color.White * opacity);
                shader.Apply();

                Drawing.DrawCircle(Arm(CrabBoss.GrabPunishArm).WristPosition(), radius, Color.White);

                Drawing.RestartSB(s, false, true);
            }
        }
    }
}
