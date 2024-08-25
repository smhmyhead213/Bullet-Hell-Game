using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.UtilitySystems.Dialogue;
 
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using bullethellwhatever.Projectiles;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabDeathray : CrabBossAttack
    {
        public Deathray Ray;
        private Vector2 ProjOffset;
        private bool Obliterated;
        public CrabDeathray(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
            Ray = new Deathray();
            Obliterated = false;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int moveToPositionTime = 30;
            int time = AITimer;

            if (time < moveToPositionTime)
            {
                Owner.DealDamage = false;

                Vector2 moveTo = new Vector2(GameWidth / 6 * 5f, GameHeight / 6 * 5f);

                MoveToPoint(moveTo, time, moveToPositionTime);

                if (time == 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 legPos = moveTo + Utilities.RotateVectorClockwise(250f * Utilities.SafeNormalise(Owner.Position - Utilities.CentreOfScreen()), -PI / 8 + (i * PI / 4) + PI / 2);

                        Leg(i).Velocity = (legPos - Leg(i).Position) / moveToPositionTime;
                        Leg(i).PointLegInDirection(Utilities.VectorToAngle(moveTo - Leg(i).Position));
                    }
                }

                CrabOwner.FacePlayer();

                Owner.DealDamage = false;
                Leg(0).ContactDamage(false);
                Leg(1).ContactDamage(false);
            }

            float ownerRotateSpeed = 0;
            int teleTime = 120;
            int beamDuration = EndTime - moveToPositionTime - teleTime;

            float radiusOfRotation = Utilities.DistanceBetweenVectors(Owner.Position, Vector2.Zero);

            if (time == moveToPositionTime + 1)
            {
                float rotation = Utilities.VectorToAngle(Utilities.CentreOfScreen() - Owner.Position);

                Owner.DealDamage = true;

                for (int i = 0; i < 2; i++)
                {
                    Leg(i).ContactDamage(true);

                    Leg(i).Velocity = Vector2.Zero;

                    Leg(i).PointLegInDirection(rotation);
                }

                Owner.DealDamage = true;
                Owner.Velocity = Vector2.Zero;

                Owner.Rotation = rotation + PI;

                TelegraphLine t = SpawnTelegraphLine(rotation, 0, Owner.Texture.Width * Owner.GetSize().X, radiusOfRotation * 1.1f,
                    teleTime, Owner.Position, Color.White, "box", Owner, false);

                Ray = new Deathray().CreateDeathray(Owner.Position, rotation, 1f, beamDuration, "box", t.Width, t.Length, 0, true, Color.Red, "CrabScrollingBeamShader", Owner);

                Ray.SetNoiseMap("CrabScrollingBeamNoise", -0.03f);
                Ray.SetStayWithOwner(true);

                t.SetOnDeath(new Action(() =>
                {
                    Ray.AddDeathrayToActiveProjectiles();
                }));
            }

            if (time == moveToPositionTime + 1 + teleTime)
            {
                Drawing.ScreenShake(7, 10);
            }

            if (time > moveToPositionTime)
            {
                int localTime = time - moveToPositionTime - 1;

                for (int i = 0; i < 2; i++)
                {
                    Leg(i).RotateLeg(Cos(localTime / 30f) * PI / 50);

                    Leg(i).Velocity = Vector2.Zero;

                    int timeBetweenProjs = Utilities.ValueFromDifficulty(6, 5, 4, 2);

                    if (time % timeBetweenProjs == 0)
                    {
                        float angle = Leg(i).UpperArm.RotationFromV();

                        SpawnTelegraphLine(angle, 0, 10, 4000, 30, Leg(i).Position, Color.Red, "box", Owner, false);

                        Projectile p = SpawnProjectile(Leg(i).LowerClaw.Position, 15f * Utilities.AngleToVector(angle), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                    }
                }
            }

            if (time > moveToPositionTime + teleTime)
            {
                float rotation = Utilities.VectorToAngle(Utilities.CentreOfScreen() - Owner.Position);

                Owner.Rotation = rotation + PI;

                Owner.Velocity = (radiusOfRotation * -ownerRotateSpeed) * Utilities.RotateVectorCounterClockwise(Utilities.SafeNormalise(Utilities.CentreOfScreen() - Owner.Position), PI / 2);

                if (player.Position.X > Owner.Position.X && player.Position.Y > Owner.Position.Y) // you have made a fatal error
                {
                    if (!Obliterated)
                    {
                        DialogueSystem.Dialogue("I CAN SEE YOU OVER THERE", 3, Owner, 180);
                        Obliterated = true;
                    }

                    CrabOwner.FacePlayer();
                }
                else
                {
                    Owner.Rotation = rotation + PI;
                }

                Ray.Rotation = Owner.Rotation - PI;

                int localTime = time - (moveToPositionTime + 1 + teleTime) - 1;

                int timeBetweenShots = 90;

                int teleDuration = 30;

                int distanceBetweenShots = Utilities.ValueFromDifficulty(110, 90, 70, 50);

                float beamLength = Utilities.DistanceBetweenVectors(Owner.Position, Vector2.Zero);

                float numberOfShots = beamLength / distanceBetweenShots;

                Vector2 betweenEach = distanceBetweenShots * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);

                if (localTime % timeBetweenShots == 0)
                {
                    ProjOffset = Utilities.RotateVectorClockwise(Utilities.RandomFloat(0f, 30f) * -Vector2.UnitY, rotation);

                    for (int i = 1; i < numberOfShots + 1; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            TelegraphLine t = SpawnTelegraphLine(rotation - PI / 2 + (j * PI), 0, 10, 2000, teleDuration, Owner.Position + ProjOffset + (i * betweenEach), Color.Red, "box", Owner, false);
                        }
                    }
                }

                if (localTime % timeBetweenShots == teleDuration)
                {
                    for (int i = 1; i < numberOfShots + 1; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            float projSpeed = 2.5f;

                            Projectile p = SpawnProjectile(Owner.Position + ProjOffset + (i * betweenEach), projSpeed * Utilities.AngleToVector(rotation - PI / 2 + (j * PI)), 1f, 1, "box", Vector2.One * 0.6f, Owner, true, Color.Red, true, false);
                            p.SetDrawAfterimages(15, 3);

                            p.Rotation = rotation - PI / 2 + (j * PI);

                            p.SetEdgeTouchEffect(new Action(() =>
                            {
                                int numberOfParticles = Utilities.RandomInt(4, 8);

                                for (int i = 0; i < numberOfParticles; i++)
                                {
                                    float rotation = Utilities.RandomFloat(0, Tau);

                                    Particle particle = new Particle();

                                    Vector2 velocity = 10f * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);
                                    int lifetime = 20;

                                    particle.Spawn("box", p.Position, velocity, -velocity / 2f / lifetime, Vector2.One * 0.45f, rotation, p.Colour, 1f, 20);
                                }
                            }));

                            p.SetExtraAI(new Action(() =>
                            {
                                p.Velocity = p.Velocity * 1.035f;
                            }));
                        }
                    }
                }
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
