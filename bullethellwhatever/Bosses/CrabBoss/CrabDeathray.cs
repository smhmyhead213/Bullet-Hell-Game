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

                Vector2 moveTo = new Vector2(ScreenWidth / 6 * 5f, ScreenHeight / 6 * 5f);

                MoveToPoint(moveTo, time, moveToPositionTime);

                if (time == 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Leg(i).ContactDamage(false);

                        Vector2 legPos = moveTo + Utilities.RotateVectorClockwise(250f * Utilities.SafeNormalise(Owner.Position - Utilities.CentreOfScreen()), -PI / 8 + (i * PI / 4) + PI / 2);

                        Leg(i).Velocity = (legPos - Leg(i).Position) / moveToPositionTime;
                        Leg(i).PointLegInDirection(Utilities.VectorToAngle(moveTo - Leg(i).Position));
                    }
                }

                CrabOwner.FacePlayer();
            }

            float ownerRotateSpeed = 0;
            int teleTime = 120;
            int beamDuration = EndTime - moveToPositionTime - teleTime;

            float radiusOfRotation = Utilities.DistanceBetweenVectors(Owner.Position, Vector2.Zero);

            if (time == moveToPositionTime)
            {
                float rotation = Utilities.VectorToAngle(Utilities.CentreOfScreen() - Owner.Position);

                for (int i = 0; i < 2; i++)
                {
                    Leg(i).ContactDamage(true);

                    Leg(i).Velocity = Vector2.Zero;

                    Leg(i).PointLegInDirection(rotation);
                }

                Owner.DealDamage = true;
                Owner.Velocity = Vector2.Zero;

                Owner.Rotation = rotation + PI;

                TelegraphLine t = new TelegraphLine(rotation, 0, 0, Owner.Texture.Width * Owner.GetSize().X, radiusOfRotation * 1.1f,
                    teleTime, Owner.Position, Color.Red, "box", Owner, false);

                Ray = new Deathray().CreateDeathray(Owner.Position, rotation, 1f, beamDuration, "box", t.Width, t.Length, 0, 0, true, Color.Red, "CrabScrollingBeamShader", Owner);

                Ray.SetNoiseMap("CrabScrollingBeamNoise");
                Ray.SetStayWithOwner(true);

                t.SpawnDeathrayOnDeath(Ray);
            }

            if (time > moveToPositionTime)
            {
                int localTime = time - moveToPositionTime;

                for (int i = 0; i < 2; i++)
                {
                    Leg(i).RotateLeg(Cos(localTime / 30f) * PI / 50);

                    Leg(i).Velocity = Vector2.Zero;

                    if (time % 2 == 0)
                    {
                        Projectile p = new Projectile();

                        float angle = Leg(i).UpperArm.RotationFromV();

                        TelegraphLine t = new TelegraphLine(angle, 0, 0, 10, 4000, 30, Leg(i).Position, Color.Red, "box", Owner, false);

                        p.Spawn(Leg(i).LowerClaw.Position, 15f * Utilities.AngleToVector(angle), 1f, 1, "box", 0f, Vector2.One, Owner, true, Color.Red, true, false);
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
                        Owner.dialogueSystem.Dialogue("I CAN SEE YOU OVER THERE", 3, 180);
                        Obliterated = true;
                    }

                    CrabOwner.FacePlayer();
                }
                else
                {
                    Owner.Rotation = rotation + PI;
                }

                Ray.Rotation = Owner.Rotation - PI;

                int localTime = time - (moveToPositionTime + teleTime) - 1;

                int timeBetweenShots = 90;

                int teleDuration = 30;

                int distanceBetweenShots = 50;

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
                            TelegraphLine t = new TelegraphLine(rotation - PI / 2 + (j * PI), 0, 0, 10, 2000, teleDuration, Owner.Position + ProjOffset + (i * betweenEach), Color.Red, "box", Owner, false);
                        }
                    }
                }

                if (localTime % timeBetweenShots == teleDuration)
                {
                    for (int i = 1; i < numberOfShots + 1; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            Projectile p = new Projectile();

                            float projSpeed = 2.5f;

                            p.SetDrawAfterimages(15);

                            p.Rotation = rotation - PI / 2 + (j * PI);

                            p.Spawn(Owner.Position + ProjOffset + (i * betweenEach), projSpeed * Utilities.AngleToVector(rotation - PI / 2 + (j * PI)), 1f, 1, "box", 1.035f, Vector2.One * 0.6f, Owner, true, Color.Red, true, false);
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
