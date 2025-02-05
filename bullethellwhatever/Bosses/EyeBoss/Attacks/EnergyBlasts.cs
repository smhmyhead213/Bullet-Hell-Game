using bullethellwhatever.Projectiles.Base;


using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using bullethellwhatever.Projectiles;
using bullethellwhatever.DrawCode.Particles;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EnergyBlasts : EyeBossAttack
    {
        public EnergyBlasts(EyeBoss owner) : base(owner)
        {

        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(int AITimer)
        {
            int time = AITimer;

            int cycleTime = 250;
            int burstTime = 120;
            float spawnDistance = 300f;
            float speed = 10f;
            int particeDuration = (int)spawnDistance / (int)speed;
            int localTime = time % cycleTime;

            Pupil.LookAtPlayer(20);

            if (localTime < burstTime - particeDuration)
            {
                Particle p = new Particle();
                float rotationAroundPupil = Utilities.RandomFloat(0, Tau);

                Vector2 spawnPos = Pupil.Position + spawnDistance * Utilities.AngleToVector(rotationAroundPupil);

                p.Spawn("box", spawnPos, speed * Utilities.SafeNormalise(Pupil.Position - spawnPos), new Vector2(0f, 0f), Vector2.One * 0.5f, rotationAroundPupil, Color.Orange, 1f, particeDuration);

                p.SetExtraAI(new Action(() =>
                {
                    float velocityLength = p.Velocity.Length();

                    p.Velocity = Utilities.SafeNormalise(Pupil.Position - p.Position) * velocityLength; // seek towards pupil

                }));

                Pupil.Scale = Vector2.Lerp(Pupil.InitialSize, Pupil.InitialSize * 2f, (float)localTime / (burstTime - particeDuration));
            }

            int dischargeEnergyTime = 4;
            int dischargeStartTime = burstTime;

            if (localTime > dischargeStartTime && localTime <= dischargeStartTime + dischargeEnergyTime)
            {
                Pupil.Scale = Vector2.Lerp(Pupil.InitialSize * 2f, Pupil.InitialSize, (float)(localTime - dischargeStartTime) / dischargeEnergyTime);
            }

            if (localTime == burstTime)
            {
                int fireballs = 8;

                for (int i = 0; i < fireballs; i++)
                {
                    Projectile p = new Projectile();

                    float mapScrollSpeed = 0.06f;
                    float projectileSpeed = 10f;

                    p.SetShader("FireballShader");
                    p.SetNoiseMap("FireNoise2", mapScrollSpeed);
                    p.ApplyRandomNoise();

                    float rotation = i * Tau / fireballs;

                    p.Rotation = rotation + PI / 2;

                    p.SetExtraAI(new Action(() =>
                    {
                        if (p.AITimer > 15)
                        {
                            p.Velocity = p.Velocity * 0.99f;
                        }

                        if (p.AITimer == 180)
                        {
                            p.Die();
                        }

                        p.Shader.Map.ScrollSpeed = p.Velocity.Length() / projectileSpeed * mapScrollSpeed;
                    }));

                    p.SetOnDeath(new Action(() =>
                    {
                        int projs = 10;
                        float randomOffset = Utilities.RandomFloat(0, Tau);

                        for (int i = 0; i < projs; i++)
                        {
                            float rotation = i * Tau / projs + randomOffset;

                            Projectile proj = SpawnProjectile(p.Position, 2f * Utilities.AngleToVector(rotation), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);

                            proj.Rotation = rotation;

                            proj.SetExtraAI(new Action(() =>
                            {
                                proj.Velocity = Utilities.RotateVectorClockwise(proj.Velocity, PI / 180) * 1.03f;
                                proj.Rotation = Utilities.VectorToAngle(proj.Velocity);
                                //TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(proj.Velocity), 0, 0, proj.Texture.Width * proj.GetSize().X, 2500, 1, proj.Position, Color.Red, "box", proj, true);
                            }));

                            proj.AddTrail(11);
                        }

                    }));

                    p.SetEdgeTouchEffect(new Action(() =>
                    {
                        p.Die();
                    }));

                    p.SpawnProjectile(Pupil.Position, projectileSpeed * Utilities.AngleToVector(rotation), 1f, 1, "box", Vector2.One * 10f, Pupil, true, Color.White, true, false);
                }
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
