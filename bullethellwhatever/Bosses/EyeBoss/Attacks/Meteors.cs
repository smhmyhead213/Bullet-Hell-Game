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
using System.Runtime.CompilerServices;
using SharpDX.WIC;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class Meteors : EyeBossAttack
    {
        public Meteors(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int timeBetweenMeteors = 30; // set this to 10 for insane mode, trust me
            int time = AITimer % timeBetweenMeteors;
            int thickness = 300;

            int pupilExpansionTime = timeBetweenMeteors / 3 * 2;

            Pupil.DistanceFromEyeCentre = MathHelper.Lerp(Pupil.DistanceFromEyeCentre, 0, 0.1f);

            if (time < pupilExpansionTime)
            {
                Pupil.Size = Vector2.Lerp(Pupil.InitialSize, Pupil.InitialSize * 2f, (float)time / pupilExpansionTime);
            }

            else
            {
                Pupil.Size = Vector2.Lerp(Pupil.InitialSize, Pupil.InitialSize * 2f, (float)(time - pupilExpansionTime) / (timeBetweenMeteors - pupilExpansionTime));
            }

            if (time == 0)
            {
                Vector2 spawnPos = new Vector2(Utilities.RandomFloat(thickness / 2, ScreenWidth - thickness / 2), 0);

                float maxRotation = PI / 60;

                float additionalRotation = Utilities.RandomFloat(-maxRotation, maxRotation);

                TelegraphLine t = new TelegraphLine(PI + additionalRotation, 0, 0, thickness, ScreenHeight * 1.5f, 60, spawnPos, Color.White, "box", Owner, false);

                Pupil.RotationWithinEye = Utilities.VectorToAngle(spawnPos - Owner.Position);
                Pupil.DistanceFromEyeCentre = 35;

                t.ShouldThickenIn(true);
                t.SetOnDeath(new Action(() =>
                {
                    BigMassiveOrb orb = new BigMassiveOrb(0, 300);
                    Texture2D texture = Assets["box"];

                    orb.SetOnDeath(new Action(() =>
                    {
                        Drawing.ScreenShake(12, 12);
                    }));

                    orb.SetDrawAfterimages(11);

                    orb.Spawn(spawnPos, Utilities.RotateVectorClockwise(-Vector2.UnitY * 80f, t.Rotation), 1f, 1, texture, 1.01f, thickness / texture.Width * Vector2.One / 2f, Owner, true, Color.White, true, false);

                    orb.Bounce = false;
                }));
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
