using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class RainingProjectileCharges : CrabBossAttack
    {
        public bool[] HasLegScreenShook;
        public bool InitialMovementStop;
        public Vector2 ToTarget;
        public RainingProjectileCharges(int endTime) : base(endTime)
        {

        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            HasLegScreenShook = new bool[2] { false, false };
            InitialMovementStop = false;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int armTime = AITimer;
            
            int waitBeforeMoving = 60;
            int SlamIntoCielingTime = 60;

            int crabChargeWaitTimeBefore = Utilities.ValueFromDifficulty(70, 60, 60, 45);
            int crabChargeWaitTimeAfter = Utilities.ValueFromDifficulty(70, 60, 60, 45);

            int bodyTime = AITimer % (crabChargeWaitTimeAfter + crabChargeWaitTimeBefore);

            if (armTime == 0)
            {
                HasLegScreenShook = new bool[2] { false, false };
            }

            if (armTime < waitBeforeMoving) // woah there slow down buddy, come back in
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).PointLegInDirection(Utilities.VectorToAngle((Owner.Position - new Vector2(0, -300)) - Leg(i).Position)); // move to a spot near the boss ish

                    Leg(i).Velocity = ((Owner.Position - new Vector2(0, 300)) - Leg(i).Position) / waitBeforeMoving;

                    Leg(i).ContactDamage(false);
                }
            }

            // --- arm code --- 
            int timeBeforeEndToExtractArms = 60;

            if (Owner.AITimer < EndTime - timeBeforeEndToExtractArms)
            {
                if (armTime == waitBeforeMoving)
                {
                    if (!InitialMovementStop)
                    {
                        Owner.Velocity = Vector2.Zero;
                        InitialMovementStop = true;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 posToMoveTo = new Vector2(Leg(i).Position.X, 0); // move upwards and root in ceiling

                        Vector2 direction = posToMoveTo - Leg(i).LowerClaw.Position;

                        Leg(i).PointLegInDirection(Utilities.VectorToAngle(direction));

                        Leg(i).Velocity = (direction) / SlamIntoCielingTime * 2f;
                    }
                }

                if (armTime > waitBeforeMoving && armTime < waitBeforeMoving + SlamIntoCielingTime + 30) // spend a bit longer ensuring the arms dont leave to be safe
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (Entity.touchingTop(Leg(i).LowerClaw) || Entity.touchingLeft(Leg(i).LowerClaw) || Entity.touchingRight(Leg(i).LowerClaw)) // dont get stuck to bottom
                        {
                            Leg(i).Velocity = Vector2.Zero; // stop if we've hit ceiling / wall

                            if (!HasLegScreenShook[i])
                            {
                                Drawing.ScreenShake(6, 8);
                                HasLegScreenShook[i] = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (Owner.AITimer == EndTime - timeBeforeEndToExtractArms)
                {
                    //Drawing.ScreenShake(8, 30);

                    for (int i = 0; i < 2; i++)
                    {
                        Leg(i).Velocity = Utilities.RotateVectorClockwise(new Vector2(0f, 0.3f), Leg(i).UpperArm.RotationFromV()); // start pulling out of ceiling slowly
                    }
                }
                else if (Owner.AITimer == EndTime - timeBeforeEndToExtractArms + 30)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Leg(i).Velocity = Leg(i).Velocity * 30f; // YANK
                    }
                }
                else if (Owner.AITimer > EndTime - timeBeforeEndToExtractArms + 30)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Leg(i).Velocity = Leg(i).Velocity * 0.9f; // slow down
                    }
                }
            }

            if (armTime > waitBeforeMoving + SlamIntoCielingTime)
            {
                Leg(0).ContactDamage(true);
                Leg(1).ContactDamage(true);

                Func<int, int> fireProjRows = x =>
                {
                    int numberOfProjsInBlast = Utilities.ValueFromDifficulty(6, 8, 10, 12);

                    int loweri = -numberOfProjsInBlast / 2;
                    int upperi = numberOfProjsInBlast / 2 + 1;

                    for (int j = loweri; j < upperi; j++)
                    {
                        Projectile projectile = new Projectile();

                        int scaledj = j * 3;

                        projectile.VelocityFunction = x => new Vector2(scaledj, x * 0.1f);

                        float yRange = Utilities.ValueFromDifficulty(20f, 30f, 40f, 70f);

                        float initialYVelocity = Utilities.RandomFloat(-yRange, yRange);

                        projectile.Spawn(Leg(x).LowerClaw.Position, new Vector2(scaledj, initialYVelocity), 1f, 1, "box", 0f, Vector2.One, Owner, true, Color.Red, true, false);
                    }

                    return 0; //dummy value
                };

                if (armTime % 120 == 0)
                {
                    fireProjRows(0);
                }

                else if (armTime % 120 == 60)
                {
                    fireProjRows(1);
                }
            }

            // ----- body code -----

            int timeBeforeDecel = 20;

            if (bodyTime == 0)
            {
                Owner.Velocity = Vector2.Zero;
            }

            if (bodyTime < crabChargeWaitTimeBefore)
            {
                CrabOwner.FacePlayer();
                
                ToTarget = player.Position - Owner.Position;

                TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(ToTarget), 0, 0, CrabOwner.Texture.Width, 3000, 1, Owner.Position, Color.White, "box", Owner, true);

                t.ChangeShader("OutlineTelegraphShader");
            }

            if (bodyTime == crabChargeWaitTimeBefore)
            {
                float chargeSpeed = Utilities.ValueFromDifficulty(20f, 30f, 40f, 45f);

                Owner.Velocity = chargeSpeed * Utilities.SafeNormalise(ToTarget);
            }

            if (bodyTime > crabChargeWaitTimeBefore && bodyTime % 10 == 0) // perpendicular projectiles
            {
                Projectile projectile = new Projectile();
                Projectile projectile2 = new Projectile();

                float initialProjSpeed = 1f;

                float projectileAccel = 1.03f;

                TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(Utilities.RotateVectorClockwise(Owner.Velocity, PI / 2)), 0, 0, 20, 3000, 15, Owner.Position, Color.White, "box", Owner, false);

                projectile.Spawn(Owner.Position, initialProjSpeed * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(Owner.Velocity, PI / 2)), 1f, 1, "box", projectileAccel, Vector2.One, Owner, true, Color.Red, true, false);

                // other side

                TelegraphLine tagain = new TelegraphLine(Utilities.VectorToAngle(Utilities.RotateVectorCounterClockwise(Owner.Velocity, PI / 2)), 0, 0, 20, 3000, 15, Owner.Position, Color.White, "box", Owner, false);

                projectile2.Spawn(Owner.Position, initialProjSpeed * Utilities.SafeNormalise(Utilities.RotateVectorCounterClockwise(Owner.Velocity, PI / 2)), 1f, 1, "box", projectileAccel, Vector2.One, Owner, true, Color.Red, true, false);
            }

            if (bodyTime > crabChargeWaitTimeBefore + timeBeforeDecel)
            {
                Owner.Velocity = Owner.Velocity * 0.93f;
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}

