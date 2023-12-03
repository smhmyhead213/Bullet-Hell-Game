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
    public class EyeRay : EyeBossAttack
    {
        public Deathray Ray;
        public EyeRay(int endTime) : base(endTime)
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
            int startTime = 60;
            int teleDuration = 60;
            int rayFireTime = teleDuration + startTime;
            int rayDuration = 1400;
            float teleRotationalVelocity = PI / 360;
            float teleRotationalAccel = -teleRotationalVelocity / rayFireTime;
            float beamRotationalVelocity = PI / 200;

            if (time == startTime)
            {
                // start a little before the player so the player cant just skip past

                TelegraphLine t = new TelegraphLine(Utilities.AngleToPlayerFrom(Pupil.Position) - (Sign(beamRotationalVelocity) * PI) , teleRotationalVelocity, teleRotationalAccel, 100, Utilities.ScreenDiagonalLength() / 1.5f, teleDuration,
                    Pupil.Position, Color.White, "box", Owner, true);

                Pupil.GoTo(30, t.Rotation);

                Ray = new Deathray();

                Ray.CreateDeathray(t.Origin, t.Rotation, 1f, rayDuration, "box", t.Width, t.Length, beamRotationalVelocity, 0, true, Color.White, "DeathrayShader2", Pupil);

                Ray.SetThinOut(true);

                Ray.SetStayWithOwner(true);

                Ray.SetNoiseMap("CrabScrollingBeamNoise", -0.06f);

                Ray.SetEdgeTouchEffect(new Action(() =>
                {
                    Particle p = new Particle();

                    float sizeFactor = Ray.Width / Ray.InitialWidth; // get smaller as the ray thins out

                    p.Spawn("Circle", Ray.BeamEdgeTouch().CollisionPoint, Vector2.Zero, Vector2.Zero, Vector2.One * sizeFactor, 0, Ray.Colour, 1, 20);
                    p.SetShrink(true);

                }));

                t.SpawnDeathrayOnDeath(Ray);
            }

            if (time <= rayFireTime)
            {
                Pupil.Rotate(teleRotationalVelocity + (time * teleRotationalAccel));

                int eyeExpansionTime = 10;

                if (time > rayFireTime - eyeExpansionTime)
                {
                    Vector2 idealSize = Vector2.One * (Ray.Width / Pupil.Texture.Width / Pupil.Size.X) / 7f; // size to encompass ray and damped down a little
                    Pupil.Size = Vector2.LerpPrecise(idealSize, Pupil.InitialSize, (float)(rayFireTime - time) / eyeExpansionTime);
                }
            }

            if (time == rayFireTime)
            {
                Drawing.ScreenShake(5, rayDuration);
            }

            if (time > rayFireTime && time < rayFireTime + rayDuration) // beam is firing
            {
                Pupil.Rotate(beamRotationalVelocity);

                foreach (ChainLink c in EyeOwner.ChainLinks)
                {
                    c.Rotate(-Cos(Ray.Rotation) / 10f); // recoil proportional to the horizontal component (cos) of the beams rotation
                }

                int telegraphTime = 60;
                int rayRingDuration = 30;
                int coolDownTime = 30;
                int calmDownBeforeRingsTime = 180;
                int rayCircleFrequency = telegraphTime + rayRingDuration + coolDownTime;
                int localTime = time % rayCircleFrequency;

                if (localTime % rayCircleFrequency == 0 && time > rayFireTime + calmDownBeforeRingsTime) // wait a bit so the boss stops swinging
                {
                    int numberOfRays = 30;

                    float offsetLimit = PI / 12;
                    float randomOffset = Utilities.RandomFloat(-offsetLimit, offsetLimit);

                    for (int i = 0; i < numberOfRays; i++)
                    {
                        TelegraphLine t = new TelegraphLine(i * Tau / numberOfRays + randomOffset, 0, 0, 40, Utilities.ScreenDiagonalLength(), telegraphTime, Pupil.Position, Color.White, "box", Pupil, true);

                        Deathray ray = new Deathray().CreateDeathray(t.Origin, t.Rotation, 1f, rayRingDuration, "box", t.Width, t.Length, 0, 0, true, Color.Gold, "DeathrayShader2", t.Owner);

                        ray.SetStayWithOwner(t.StayWithOwner);
                        ray.SetThinOut(true);

                        t.SpawnDeathrayOnDeath(ray);
                    }
                }
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
