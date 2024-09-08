using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

 
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using bullethellwhatever.MainFiles;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class LaserSwingProjectileBurst : EyeBossAttack
    {
        public float AngleToSwing;
        public int SwingPassesComplete;
        public LaserSwingProjectileBurst(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
            AngleToSwing = 0;
            SwingPassesComplete = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int rayTelegraphTime = 60;
            int rayDuration = 10;
            int swingTime = 90;
            int eyeFocusTime = 15;
            int cycleTime = (rayTelegraphTime + swingTime + eyeFocusTime);
            int time = AITimer % cycleTime;

            ChainLink firstLink = EyeOwner.ChainLinks[0];

            float rayAdditionalAngle = SwingPassesComplete * PI; // if an odd number of passes have been completed, a half turn will be added to the ray angle.
            float beamDirection = firstLink.Rotation + PI / 2 + rayAdditionalAngle;

            float dampingFactorDuringRayCharge = 0.9f;

            if (time < rayTelegraphTime)
            {
                Pupil.LookAtPlayer(30f);

                // slow down swinging while charging the beam

                EyeOwner.SetChainDampingFactor(dampingFactorDuringRayCharge);

                if (time == 1)
                {
                    TelegraphLine t = SpawnTelegraphLine(beamDirection, 0, 50, Utilities.ScreenDiagonalLength(), rayTelegraphTime, Pupil.Position, Color.White, "box", Pupil, true);

                    t.ThickenIn = true;

                    t.SetExtraAI(new Action(() =>
                    {
                        t.Rotation = EyeOwner.ChainLinks[0].Rotation + PI / 2 + rayAdditionalAngle;
                    }));

                    t.SetOnDeath(new Action(() =>
                    {
                        SwingPassesComplete++;

                        Deathray ray = new Deathray();

                        ray.SetStayWithOwner(true);
                        ray.SetThinOut(true);
                        ray.SpawnDeathray(t.Origin, t.Rotation, 1f, rayDuration, "box", t.Width, t.Length, 0, true, Color.White, "DeathrayShader2", Pupil);

                        int projectiles = 50;
                        int distanceBetweenProjectiles = 150;

                        float additionalRotation = t.Rotation;

                        for (int i = 0; i < projectiles; i++)
                        {
                            Vector2 additionalDistance = Utilities.AngleToVector(t.Rotation) * distanceBetweenProjectiles;

                            for (int j = 0; j < 2; j++)
                            {
                                float projSpeed = 0.9f;

                                Projectile p = SpawnProjectile(t.Origin + i * additionalDistance, projSpeed * Utilities.AngleToVector(additionalRotation - PI / 2 + (j * PI)), 1f, 1, "box", Vector2.One * 0.6f, Owner, true, Color.White, true, false);

                                p.AddTrail(50);

                                p.Rotation = additionalRotation - PI / 2 + (j * PI);

                                p.SetExtraAI(new Action(() =>
                                {
                                    p.Rotation = Utilities.VectorToAngle(p.Velocity);
                                    p.ExponentialAccelerate(1.05f);
                                }));                               
                            }
                        }
                    }));
                }
            }

            if (time < rayTelegraphTime)
            {
                //float lookAngle = Utilities.VectorToAngle(Pupil.Position - Owner.Position);
                Pupil.GoTo(30, beamDirection);
            }

            if (time == rayTelegraphTime) // when the ray is fired
            {
                float angleToReachSwingingLeft = Utilities.ToRadians(250f); // im too lazy to convert this myself
                float angleToReachSwingingRight = Utilities.ToRadians(20f);

                if (SwingPassesComplete % 2 == 0) // if we are swinging left this time
                {
                    AngleToSwing = firstLink.Rotation - angleToReachSwingingLeft;
                }
                else
                {
                    AngleToSwing = angleToReachSwingingLeft - firstLink.Rotation;
                }
            }

            //if (time > rayTelegraphTime && time <= swingTime + rayTelegraphTime)
            //{
            //    float localTime = time - rayTelegraphTime;

            //    foreach (ChainLink c in EyeOwner.ChainLinks)
            //    {
            //        c.Rotate(40f * AngleToSwing / swingTime / localTime);
            //    }
            //}

            float projectileBurstChargeTime = 50;

            if (time >= rayTelegraphTime && time < rayTelegraphTime + projectileBurstChargeTime)
            {
                if (time == rayTelegraphTime) // on ray fire
                {
                    EyeOwner.SetChainDampingFactor(EyeOwner.InitialChainDampingFactor);

                    foreach (ChainLink c in EyeOwner.ChainLinks)
                    {
                        c.ApplyTorque(AngleToSwing / 10f);
                    }
                }

                int localTime = time - rayTelegraphTime;

                float progress = localTime / projectileBurstChargeTime;

                // make progress ease

                float easedProgress = 1 - Pow(1 - progress, 4); // easeOutExpo from easings.net

                Vector2 initialSize = Pupil.InitialSize;

                Pupil.Dilate(initialSize / 4f, easedProgress);
            }

            if (time == rayTelegraphTime + projectileBurstChargeTime)
            {
                int numberOfProjectiles = 50;

                for (int i = 0; i < numberOfProjectiles; i++)
                {
                    float xVelAmplitude = 20f;
                    float xVelocity = Utilities.RandomFloat(-xVelAmplitude, xVelAmplitude);

                    float yVelAmplitude = 20f;
                    float yVelocity = Utilities.RandomFloat(-yVelAmplitude, 0);

                    Projectile p = SpawnProjectile(Pupil.Position, new Vector2(xVelocity, yVelocity), 1f, 1, "box", Vector2.One * 0.85f, Pupil, true, Color.Red, true, false);

                    p.SetExtraAI(new Action(() =>
                    {
                        p.Velocity.Y = p.Velocity.Y + 0.3f; // gravity
                        p.Rotation = Utilities.VectorToAngle(p.Velocity);
                    }));

                    p.AddTrail(20);

                    p.SetEdgeTouchEffect(new Action(() =>
                    {
                        AttackUtilities.ParticleExplosion(15, 20, 10f, Vector2.One * 0.45f, p.Position, p.Colour);
                    }));
                }
            }

            int pupilExpandTime = 25;
            int expandStartTime = rayTelegraphTime + (int)projectileBurstChargeTime;

            if (time > expandStartTime && time <= expandStartTime + pupilExpandTime)
            {
                int localTime = time - expandStartTime;

                Pupil.Dilate(Pupil.InitialSize, localTime / (float)pupilExpandTime);
            }
        }

        public override void ExtraAttackEnd()
        {
            base.ExtraAttackEnd();

            Pupil.ResetSize();
        }
        public override void ExtraDraw(SpriteBatch s)
        {
            //Utilities.drawTextInDrawMethod(aitimer.ToString(), new Vector2(ScreenWidth / 4f * 3f, ScreenHeight / 4f * 3f), s, font, Color.White);
        }
    }
}
