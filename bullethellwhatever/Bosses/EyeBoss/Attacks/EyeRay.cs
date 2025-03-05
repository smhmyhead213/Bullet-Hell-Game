using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Runtime.CompilerServices;
using bullethellwhatever.DrawCode.Particles;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EyeRay : EyeBossAttack
    {
        public Deathray ray; // lowercase because Ray is pre defined or something idk
        public EyeRay(EyeBoss owner) : base(owner)
        {

        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            ray = new Deathray();
        }
        public override void Execute(int AITimer)
        {
            int time = AITimer;
            int startTime = 60;
            int teleDuration = 60;
            int rayFireTime = teleDuration + startTime;
            int rayDuration = 1400;
            float teleRotationalVelocity = PI / 360;
            
            float beamRotationalVelocity = PI / 200;

            float rayWidth = 100f;
            float rayLength = Utilities.ScreenDiagonalLength() / 1.5f;
            if (time == startTime)
            {
                EyeOwner.SetChainDampingFactor(EyeOwner.InitialChainDampingFactor);

                // start a little before the player so the player cant just skip past

                TelegraphLine telegraphLine = SpawnTelegraphLine(Utilities.AngleToPlayerFrom(Pupil.Position) - (Sign(beamRotationalVelocity) * PI) , teleRotationalVelocity, rayWidth, rayLength, teleDuration,
                    Pupil.Position, Color.White, "box", Owner, true);

                telegraphLine.SetExtraAI(new Action(() =>
                {
                    telegraphLine.RotationalVelocity += telegraphLine.RotationalAcceleration;
                }
                ));

                Pupil.GoTo(30, telegraphLine.Rotation);

                telegraphLine.SetOnDeath(new Action(() =>
                {
                    float width = telegraphLine.Width;
                    float length = telegraphLine.Length; // use local values to pass these by value

                    ray = SpawnDeathray(telegraphLine.Origin, telegraphLine.Rotation, 1f, rayDuration, "box", width, length, beamRotationalVelocity, true, false, Color.White, "DeathrayShader2", Pupil);

                    ray.SetThinOut(true);

                    ray.SetStayWithOwner(true);

                    ray.SetNoiseMap("CrabScrollingBeamNoise", -0.06f);

                    ray.SetEdgeTouchEffect(new Action(() =>
                    {
                        //Particle p = new Particle();

                        //float sizeFactor =  ray.Width / ray.InitialWidth; // get smaller as the ray thins out

                        //p.Spawn("Circle", ray.BeamEdgeTouch().CollisionPoint, Vector2.Zero, Vector2.Zero, Vector2.One * sizeFactor, 0, ray.Colour, 1, 20);
                        //p.SetShrink(true);
                    }));
                }));

            }

            if (time == rayFireTime)
            {
                float teleRotationalAccel = -teleRotationalVelocity / rayFireTime;

                Pupil.Rotate(teleRotationalVelocity + (time * teleRotationalAccel));

                int eyeExpansionTime = 10;

                if (time > rayFireTime - eyeExpansionTime)
                {
                    Vector2 idealSize = Vector2.One * (rayWidth / Pupil.Texture.Width / Pupil.Scale.X) / 7f; // size to encompass ray and damped down a little
                    Pupil.Scale = Vector2.LerpPrecise(idealSize, Pupil.InitialSize, (float)(rayFireTime - time) / eyeExpansionTime);
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
                    c.Rotate(-Cos(ray.Rotation) / 10f); // recoil proportional to the horizontal component (cos) of the beams rotation
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
                        TelegraphLine t = SpawnTelegraphLine(i * Tau / numberOfRays + randomOffset, 0, 40, Utilities.ScreenDiagonalLength(), telegraphTime, Pupil.Position, Color.White, "box", Pupil, true);

                        t.SetOnDeath(new Action(() =>
                        {
                            Deathray ray = SpawnDeathray(t.Origin, t.Rotation, 1f, rayRingDuration, "box", t.Width, t.Length, 0, true, false, Color.Gold, "DeathrayShader2", t.Owner);

                            ray.SetStayWithOwner(t.StayWithOwner);
                            ray.SetThinOut(true);
                        }));
                    }
                }
            }
        }
    }
}
