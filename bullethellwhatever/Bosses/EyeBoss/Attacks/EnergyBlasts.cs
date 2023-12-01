using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EnergyBlasts : EyeBossAttack
    {
        public Deathray Ray;
        public EnergyBlasts(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer;
            int startTime = 200;
            int chargeUpTime = 120;

            if (time > startTime && time < chargeUpTime + startTime)
            {
                Particle p = new Particle();
                float rotationAroundPupil = Utilities.RandomFloat(0, Tau);
                float spawnDistance = 300f;
                float speed = 10f;
                Vector2 spawnPos = Pupil.Position + spawnDistance * Utilities.AngleToVector(rotationAroundPupil);

                p.Spawn("box", spawnPos, speed * Utilities.SafeNormalise(Pupil.Position - spawnPos), new Vector2(0f, 0f), Vector2.One * 0.5f, rotationAroundPupil, Color.White, 1f, (int)spawnDistance / (int)speed);

                p.SetExtraAI(new Action(() =>
                {
                    float velocityLength = p.Velocity.Length();

                    p.Velocity = Utilities.SafeNormalise(Pupil.Position - p.Position) * velocityLength; // seek towards pupil

                }));
            }

            if (time > chargeUpTime + startTime && time % 60 == 0)
            {
                Projectile p = new Projectile();

                p.SetShader("FireballShader");
                p.SetNoiseMap("FireNoise1", 0.06f);
                p.ApplyRandomNoise();

                p.Rotation = Utilities.AngleToPlayerFrom(Pupil.Position) + PI / 2;

                p.SetExtraAI(new Action(() =>
                {
                    if (p.AITimer > 30)
                    {
                        p.HomeAtTarget(player.Position, 0.05f);
                    }
                }));

                //p.SetDrawAfterimages(15, 1);

                p.Spawn(Pupil.Position, 20f * Utilities.SafeNormalise(player.Position - Pupil.Position), 1f, 1, "box", 1f, Vector2.One * 10f, Pupil, true, Color.White, true, false);
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
