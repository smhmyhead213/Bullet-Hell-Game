using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;
using bullethellwhatever.DrawCode.Particles;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabBombThrow : CrabBossAttack
    {
        public CrabBombThrow(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int pullBackArmTime = 15;
            int throwTime = 15;

            float pullBackArmAngle = PI / 2;
            float throwAngle = 2 * PI / 3;
            int expandedi = Utilities.ExpandedIndex(ChosenArmIndex());
            float jumpBackSpeed = 30f;

            CrabOwner.FacePlayer();

            if (AITimer == 0)
            {
                Owner.Velocity = Utilities.SafeNormalise(Owner.Position - player.Position) * jumpBackSpeed;

                Projectile bomb = SpawnProjectile<Projectile>(ChosenArm().PositionAtDistanceFromWrist(20), Vector2.Zero, 1f, 1, "box", Vector2.One, Owner, true, false, Color.Red, false, false);

                bomb.AddTrail(7);

                Action bombExplode = new Action(() =>
                {
                    int numberOfProjectiles = 26;

                    for (int i = 0; i < numberOfProjectiles; i++)
                    {
                        float angle = Tau / numberOfProjectiles * i + bomb.Velocity.ToAngle();
                        Projectile p = SpawnProjectile<Projectile>(bomb.Position, 0.1f * Utilities.AngleToVector(angle), 1f, 1, "box", Vector2.One, Owner, true, false, Color.Red, true, false);

                        p.AddTrail(22);
                        p.Rotation = angle;

                        p.SetExtraAI(new Action(() =>
                        {
                            p.Velocity += 0.4f * Utilities.SafeNormalise(p.Velocity);
                        }));

                    }

                    int particles = 20;
                    float particleSpeed = 10f;
                    int particleLifetime = 30;

                    for (int i = 0; i < particles; i++)
                    {
                        float angle = Utilities.RandomAngle();
                        Particle p = new Particle();
                        p.Spawn("box", bomb.Position, particleSpeed * Utilities.RandomFloat(0.5f, 1.5f) * Utilities.AngleToVector(angle), Vector2.Zero, new Vector2(0.5f, 2.5f), angle, Color.Orange, 1f, particleLifetime);

                        p.SetExtraAI(new Action(() =>
                        {
                            float interpolant = p.AITimer / (float)particleLifetime;
                            p.Velocity *= 0.95f;
                            p.Opacity = MathHelper.Lerp(1f, 0f, interpolant);
                            p.Shader.SetColour(p.Colour * p.Opacity);
                        }));

                        p.SetShader("EnergyParticleShader");
                    }

                    bomb.InstantlyDie();
                });

                bomb.SetExtraAI(new Action(() =>
                {
                    int releaseTime = pullBackArmTime + throwTime + 3;
                    if (bomb.AITimer < releaseTime)
                        bomb.Position = ChosenArm().PositionAtDistanceFromWrist(20);
                    else if (bomb.AITimer == releaseTime)
                    {
                        float launchSpeed = 20f;
                        bomb.Velocity = Utilities.SafeNormalise(player.Position - bomb.Position) * launchSpeed;
                    }

                    int bombExplodeTime = 120;

                    if (bomb.AITimer > releaseTime && bomb.AITimer < bombExplodeTime)
                    {
                        int localTime = bomb.AITimer - releaseTime;
                        int slowDownDuration = bombExplodeTime - releaseTime;
                        float interpolant = localTime / (float)slowDownDuration;

                        bomb.Velocity = Vector2.LerpPrecise(bomb.Velocity, Vector2.Zero, interpolant);

                        float distanceFromBombToSpawnParticles = 200;
                        float angle = Utilities.RandomAngle();

                        int suckInTime = 15;

                        Particle p = new Particle();

                        Vector2 spawnLocation = bomb.Position + Utilities.AngleToVector(angle) * distanceFromBombToSpawnParticles;
                        Vector2 spawnVelocity = distanceFromBombToSpawnParticles / suckInTime * Utilities.AngleToVector(angle + PI);

                        p.SetShader("EnergyParticleShader");

                        p.Spawn("box", spawnLocation, spawnVelocity, Vector2.Zero, new Vector2(0.5f, 2.5f), angle, Color.White, 1f, suckInTime);
                    }

                    if (bomb.AITimer == bombExplodeTime)
                    {
                        bombExplode();
                    }
                }));

                bomb.SetEdgeTouchEffect(new Action(() =>
                {
                    int graceTimeBeforeCanExplode = 15;

                    if (bomb.AITimer > graceTimeBeforeCanExplode)
                    {
                        bombExplode();
                    }
                }));
            }

            if (AITimer < pullBackArmTime)
            {
                RotateArm(ChosenArmIndex(), -expandedi * pullBackArmAngle, AITimer, pullBackArmTime, EasingFunctions.EaseOutQuad);

                float interpolant = AITimer / (float)pullBackArmTime;

                Owner.Velocity = jumpBackSpeed * Utilities.SafeNormalise(Owner.Velocity) * EasingFunctions.EaseOutQuad(1 - interpolant);
            }

            if (AITimer >= pullBackArmTime && AITimer < pullBackArmTime + throwTime)
            {
                Owner.Velocity *= 0.97f;

                int localTime = AITimer - pullBackArmTime;

                RotateArm(ChosenArmIndex(), -expandedi * -throwAngle, localTime, throwTime, EasingFunctions.EaseOutQuad);
            }

            if (AITimer == pullBackArmTime + throwTime)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new BombThrowToNeutralTransition(CrabOwner);
        }
    }

    public class BombThrowToNeutralTransition : CrabBossAttack
    {
        public BombThrowToNeutralTransition(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int duration = 30;
            int expandedi = Utilities.ExpandedIndex(ChosenArmIndex());
            float pullBackArmAngle = PI / 2;
            float throwAngle = 2 * PI / 3;

            float difference = throwAngle - pullBackArmAngle;

            if (AITimer < duration)
            {
                //RotateArm(ChosenArmIndex(), -expandedi * difference, AITimer, duration, EasingFunctions.EaseOutQuad);
                ChosenArm().LerpToRestPosition(EasingFunctions.EaseOutQuad(AITimer / (float)duration));
            }
            if (AITimer == duration)
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
