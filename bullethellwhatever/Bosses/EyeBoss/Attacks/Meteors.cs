using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;


using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SharpDX.WIC;
using bullethellwhatever.Projectiles;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class Meteors : EyeBossAttack
    {
        public Meteors(EyeBoss owner) : base(owner)
        {

        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(int AITimer)
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
                Vector2 spawnPos = new Vector2(Utilities.RandomFloat(thickness / 2, GameWidth - thickness / 2), 0);

                float maxRotation = PI / 60;

                float additionalRotation = Utilities.RandomFloat(-maxRotation, maxRotation);

                TelegraphLine t = SpawnTelegraphLine(PI + additionalRotation, 0, thickness, GameHeight * 1.5f, 60, spawnPos, Color.White, "box", Owner, false);

                Pupil.RotationWithinEye = Utilities.VectorToAngle(spawnPos - Owner.Position);
                Pupil.DistanceFromEyeCentre = 35;

                t.ShouldThickenIn(true);
                t.SetOnDeath(new Action(() =>
                {
                    Texture2D texture = AssetRegistry.GetTexture2D("Circle");

                    Projectile orb = SpawnProjectile(spawnPos, Utilities.RotateVectorClockwise(-Vector2.UnitY * 66f, t.Rotation), 1f, 1, "Circle", thickness / texture.Width * Vector2.One / 2f, Owner, true, Color.White, true, false);
                    
                    orb.SetOnDeath(new Action(() =>
                    {
                        Drawing.ScreenShake(12, 12);
                    }));

                    int timeBetweenProjs = 5;
                    int projectileTimeOffset = Utilities.RandomInt(0, timeBetweenProjs - 1);

                    orb.SetExtraAI(new Action(() =>
                    {
                        orb.Velocity = orb.Velocity * 1.01f;

                        if (orb.AITimer % timeBetweenProjs == projectileTimeOffset)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                float projSpeed = 2.5f;

                                Projectile p = SpawnProjectile(orb.Position, projSpeed * Utilities.AngleToVector(additionalRotation - PI / 2 + (j * PI)), 1f, 1, "box", Vector2.One * 0.6f, Owner, true, Color.White, true, false);

                                p.AddTrail(50);

                                p.Rotation = additionalRotation - PI / 2 + (j * PI);

                                p.SetExtraAI(new Action(() =>
                                {
                                    p.Velocity *= 1.035f;

                                    p.HomeAtTarget(player.Position, 0.003f);
                                    p.Rotation = Utilities.VectorToAngle(p.Velocity);
                                }));
                            }
                        }
                    }));

                    orb.AddTrail(11);
                }));
            }
        }
        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
