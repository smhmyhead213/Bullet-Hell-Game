using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

 
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class HelixShots : EyeBossAttack
    {
        public float ShootAngle;
        public HelixShots(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
            ShootAngle = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int waitTime = 45;
            int telegraphTime = 60;
            int shootTime = 10;
            int totalAttackTime = waitTime + telegraphTime + shootTime;
            int time = AITimer % totalAttackTime;
            int telegraphLockTime = 10;

            if (time < waitTime && time > shootTime && AITimer > totalAttackTime) // dont shoot homing projs the first time round
            {
                float halfTime = waitTime / 2f;
                float totalSweepAngle = PI;
                float angularVelocity = totalSweepAngle / waitTime;

                float rotation = Utilities.AngleToPlayerFrom(Pupil.Position) - PI - (time - halfTime) * angularVelocity;

                Pupil.RotationWithinEye = rotation;

                if (time % 2 == 0)
                {
                    Projectile p = SpawnProjectile(Pupil.Position, 140f * Utilities.AngleToVector(rotation), 1f, 1, "box", Vector2.One * 0.8f, Pupil, true, Color.Red, true, false);

                    p.SetExtraAI(new Action(() =>
                    {
                        if (p.AITimer < 15)
                        {
                            p.ExponentialAccelerate(0.8f);
                        }
                        else
                        {
                            p.Velocity *= 1.06f;
                        }

                        if (p.AITimer > 15 && p.AITimer < 20)
                        {
                            p.HomeAtTarget(player.Position, 0.7f);
                        }

                        p.Rotation = Utilities.VectorToAngle(p.Velocity);
                    }));

                    p.AddTrail(11);
                }
            }

            if (time == waitTime)
            {
                Pupil.LookAtPlayer(20);

                TelegraphLine t = SpawnTelegraphLine(Utilities.AngleToPlayerFrom(Pupil.Position), 0, 25, Utilities.ScreenDiagonalLength() * 1.5f, telegraphTime, Pupil.Position, Color.White, "box", Pupil, true);

                t.ThickenIn = true;

                t.SetExtraAI(new Action(() =>
                {
                    if (t.TimeAlive < telegraphTime - telegraphLockTime)
                    {
                        t.Rotation = Utilities.AngleToPlayerFrom(t.Owner.Position);
                        ShootAngle = t.Rotation;
                    }
                }));

                Deathray ray = new Deathray();

                t.SetOnDeath(new Action(() =>
                {
                    ray.SetStayWithOwner(true);
                    ray.SetThinOut(true);
                    ray.SpawnDeathray(t.Origin, t.Rotation, 1f, shootTime, "box", t.Width, t.Length, 0, true, Color.White, "DeathrayShader2", Pupil);
                }));
            }

            float particleSpeed = 10f;
            float spawnDistance = 250f;

            float particleLifeTime = spawnDistance / particleSpeed;

            if (time > waitTime && time < waitTime + telegraphTime - particleLifeTime)
            {
                if (time < waitTime + telegraphTime - particleLifeTime - telegraphLockTime)
                {
                    Pupil.LookAtPlayer(20);
                }

                AttackUtilities.SuckInParticles(Pupil.Position, Color.White, particleSpeed, spawnDistance, 1);

                float interpolant = (time - waitTime) / (float)telegraphTime;

                Pupil.Size.X = MathHelper.Lerp(Pupil.InitialSize.X, Pupil.InitialSize.X * 1.3f, interpolant); // lengthen pupil slightly
                Pupil.Size.Y = MathHelper.Lerp(Pupil.InitialSize.Y, Pupil.InitialSize.X * 0.1f, interpolant); // narrow pupil
            }

            if (time > waitTime + telegraphTime && time <= waitTime + telegraphTime + shootTime)
            {
                Pupil.LookAtPlayer(20);

                float interpolant = (time - waitTime) / (float)telegraphTime;

                Pupil.Size.X = MathHelper.Lerp(Pupil.InitialSize.X, Pupil.InitialSize.X * 1.3f, 1f - interpolant); // thinnen pupil back
                Pupil.Size.Y = MathHelper.Lerp(Pupil.InitialSize.Y, Pupil.InitialSize.X * 0.5f, 1f - interpolant); // open pupil

                for (int i = 0; i < 2; i++)
                {
                    Projectile p = SpawnProjectile(Pupil.Position, Vector2.Zero, 1f, 1, "box", Vector2.One, Pupil, true, Color.Red, true, false);

                    float horizontalSpeed = 15f;
                    float verticalSpeedAmplitude = 30f;
                    float localShootAngle = ShootAngle;

                    int direction = i == 0 ? 1 : -1;

                    p.SetExtraAI(new Action(() =>
                    {
                        Vector2 horizontalVelocity = Utilities.RotateVectorClockwise(new Vector2(horizontalSpeed, 0f), localShootAngle - PI / 2);
                        Vector2 verticalVelocity = direction * Utilities.RotateVectorClockwise(new Vector2(0, verticalSpeedAmplitude * Cos(p.AITimer / 10f)), localShootAngle - PI / 2);

                        p.Velocity = horizontalVelocity + verticalVelocity;
                        p.Rotation = Utilities.VectorToAngle(p.Velocity);
                    }));

                    p.AddTrail(11);                   
                }
            }
        }
        public override void ExtraAttackEnd()
        {
            base.ExtraAttackEnd();

            Pupil.ResetSize();
        }
        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
