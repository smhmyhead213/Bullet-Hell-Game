using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base; //bad code
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.DrawCode;

using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using bullethellwhatever.AssetManagement;
using SharpDX.Direct3D9;
using bullethellwhatever.DrawCode.UI;

namespace bullethellwhatever.Bosses
{
    public class SecondBoss : Boss
    {
        public bool HasChosenChargeDirection;

        public float DeathrayAngularVelocity;

        public PrimitiveSet prims;

        public Deathray ray;
        public Deathray ray2;
        public SecondBoss(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;

            MaxHP = 200;

            BlockDeathrays = true;

            Health = 200;
            AITimer = 0;
            IFrames = 5f;
            HasChosenChargeDirection = false;
            CanDie = false;

            DeathrayAngularVelocity = 180f;
            IsHarmful = true;
            Rotation = PI / 6f;
            Size = new Vector2(3f, 3f);
            Opacity = 0.1f;

            Colour = Color.White;

            Texture = AssetRegistry.GetTexture2D("box");

            HealthBar hp = new HealthBar("box", new Vector2(300f, 60f), this, new Vector2(GameWidth / 2, 4 * GameHeight / 5));
            hp.Display();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Hitbox.Draw(3f);

            //for (int i = 0; i < Hitbox.Vertices.Length; i++)
            //{
            //    Drawing.BetterDraw(Texture, Hitbox.Vertices[i], null, Color.Red, 0, Vector2.One, SpriteEffects.None, 0);
            //    Utilities.drawTextInDrawMethod(i.ToString(), Hitbox.Vertices[i] + new Vector2(30f, 0f), spriteBatch, font, Colour);
            //}
            //prims.Draw();
        }
        public override void AI()
        { 
            if (Health < 0 && CanDie)
                Die();

            if (AITimer == 0)
            {
                //Deathray ray = new Deathray();

                //ray.SetNoiseMap("CrabScrollingBeamNoise");

                //TelegraphLine t = new TelegraphLine(0, 0, 0, 100, 2000, 1000, Position, Colour, "box", this, true);

                //t.ChangeShader("OutlineTelegraphShader");

                //ray.SpawnDeathray(Position, PI / 2f, 0f, 3000, "box", 150f, ScreenWidth, PI / 600, 0, true, Colour, "CrabScrollingBeamShader", this);

                //ChangeGraphicsDeviceTexture(1, "CrabScrollingBeamNoise");
            }

            Rotation = Rotation + PI / 90f;
            //Update the boss position based on its velocity.
            Position = Position + Velocity;

            if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.T))
            {
                Position = MousePositionWithCamera();
            }

            if (IFrames > 0)
            {
                IFrames--;
            }

            ExecuteEasyAttackPattern();
        }

        public void ExecuteEasyAttackPattern()
        {
            if (Health <= 0)
            {
                CanDie = true;
            }
        }

        public void Explosions(ref float AITimer, ref int AttackNumber, int numberOfProjectiles) // this needs fixed 
        {
            float offset = PI / 500f * AITimer;

            if (AITimer % 2 == 0)
            {
                for (int i = 0; i < numberOfProjectiles; i++)
                {
                    //ExplodingProjectile explodingProjectile = new ExplodingProjectile(4, 120, -0.5f * offset, false, false, true);

                    float angle = 2 * PI / numberOfProjectiles * i;

                    Projectile explodingProjectile = SpawnProjectile(Position, 3f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(Vector2.UnitY, angle + offset), Vector2.Zero), 1f, 1, "box", Vector2.One, this, true, Color.Red, false, false);

                    explodingProjectile.SetExtraAI(new Action(() =>
                    {
                        if (explodingProjectile.AITimer == 120)
                        {
                            for (int i = 0; i <= numberOfProjectiles; i++)
                            {
                                Projectile p = SpawnProjectile(explodingProjectile.Position, 3f * Utilities.SafeNormalise(Utilities.RotateVectorClockwise(Vector2.UnitY, Tau / numberOfProjectiles * i), Vector2.Zero), 1f, 1, "box", Vector2.One, this, true, Color.Red, false, false);
                            }
                        }
                    }));
                }
            }
        }
        
        public void DialogueTest(ref int AITimer, ref int AttackNumber)
        {
            if (AITimer == 1)
            {              
                DialogueSystem.Dialogue("This boss is in progress, ignore it.", 4, this, 400);

                //Drawing.ScreenShake(20, 500);
                //TelegraphLine t = new(0f, PI / 600f, 0f, 20f, 500f, 1000, Position, Color.Yellow, "box", this, true);
                
                //TelegraphLine t2 = new(PI, PI / 600f, 0f, 20f, 500f, 10000, Position, Color.Yellow, "box", this, true);
                //t.ChangeShader("OutlineTelegraphShader");

                //Cuboid cuboid = new Cuboid(500, 500, 500, this);

                //ray = new Deathray();
                //ray2 = new Deathray();

                //ray.SpawnDeathray(new Vector2(ScreenWidth / 4 + 50f, ScreenHeight / 2), PI / 2f, 0f, 9999, "box", 30, 3000, 0, 0, true, Colour, "DeathrayShader", this);
                //ray2.SpawnDeathray(new Vector2(ScreenWidth / 4 * 3 + 50f, ScreenHeight / 2), 0f, 0f, 9999, "box", 30, 3000, PI / 240f, 0, true, Colour, "DeathrayShader", this);
            }

            if (AITimer % 10 == 0)
            {
                ShockwaveRing sRing = new ShockwaveRing(20, 80, 200, 10);

                //sRing.Spawn(Position, this);
            }
            if (AITimer == 50)
            {
                Projectile p = SpawnProjectile(Position, Vector2.UnitX * 10f, 1f, 1, "box", Vector2.One, this, true, Color.Red, true, false);
                //ChargingEnemy enemy = new ChargingEnemy(60, 120);

                //enemy.Spawn(Position, 7f * Vector2.UnitY, 1f, "box", Vector2.One, 3f, 1, Color.White, false, true);
            }

            if (AITimer == 60)
            {
                AttackUtilities.ClearProjectiles();
            }
        }

        public void EndAttack(ref float AITimer, ref int AttackNumber)
        {
            AITimer = 0; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0
            Rotation = 0;
            AttackNumber++;
        }
    }
}

