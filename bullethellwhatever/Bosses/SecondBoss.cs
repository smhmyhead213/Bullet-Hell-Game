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
            HarmfulToPlayer = true;
            Rotation = PI / 6f;
            Scale = new Vector2(6f, 6f);
            Opacity = 0.1f;

            Colour = Color.White;

            Texture = AssetRegistry.GetTexture2D("box");

            HealthBar hp = new HealthBar("box", new Vector2(300f, 60f), this, new Vector2(GameWidth / 2, 4 * GameHeight / 5));
            hp.Display();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            DrawHitbox();

            //Hitbox.Draw(3f);

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
        
        public void EndAttack(ref float AITimer, ref int AttackNumber)
        {
            AITimer = 0; //to prevent jank with EndAttack taking a frame, allows attacks to start on 0
            Rotation = 0;
            AttackNumber++;
        }
    }
}

