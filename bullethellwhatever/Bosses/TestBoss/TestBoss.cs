using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems.Dialogue;

using bullethellwhatever.DrawCode;
using bullethellwhatever.Enemies;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class TestBoss : Boss
    {
        public bool HasChosenChargeDirection;
        public float SpiralStartTime;

        public bool HasDesperationStarted;

        public TestBoss(Vector2 position, Vector2 velocity, Texture2D texture)
        {
            Position = position;
            Velocity = velocity;

            MaxHP = 200;
            AITimer = 0;
            MaxIFrames = 5;
            HasChosenChargeDirection = false;
            AttackNumber = 1;
            HasDesperationStarted = false;
            IsDesperationOver = false;

            Colour = Color.White;

            Texture = texture;

            Size = new Vector2(5f, 5f);

            musicSystem.SetMusic("TestBossMusic", true, 0.01f);

            FramesPerMusicBeat = 24; //dividing by 2 prevents me having to fix the great double > single update bug of the 3rd of spetember 2023
            BeatsPerBar = 4;
            BarDuration = FramesPerMusicBeat * BeatsPerBar;

            BossAttacks = new BossAttack[]
            { 
                new BasicShotgunSpread(BarDuration * 18),
                new Charge(BarDuration * 16),
                new Spiral(BarDuration * 12),
                new SpawnEnemies(BarDuration),
                new LaserBarrages(BarDuration * 7),
                new MoveTowardsAndShotgun(BarDuration * 7),
                new ExplodingProjectiles(BarDuration * 10),             
                new MutantBulletHell(BarDuration * 12),                
                new HorizontalChargesWithProjectiles(BarDuration * 13),
                new Stomp(BarDuration * 17),
            };

        }
        public override void Spawn(Vector2 position, Vector2 initialVelocity, float damage, string texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool isHarmful)
        {
            base.Spawn(position, initialVelocity, damage, texture, size, MaxHealth, pierceToTake, colour, shouldRemoveOnEdgeTouch, isHarmful);

            
        }

        public override void AI()
        {
            SetUpdates(2);

            CurrentBeat = (int)(((float)AITimer / BarDuration - MathF.Floor(AITimer / BarDuration)) * 4) + 1;

            JustStartedBeat = AITimer % FramesPerMusicBeat == 0 ? true : false;

            base.AI();

            if (Health <= 0 && !HasDesperationStarted)
            {
                HasDesperationStarted = true;
                ReplaceAttackPattern(new BossAttack[] { new Desperation(BarDuration * 30) });
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Drawing.BetterDraw(Texture, Position, null, Colour, Rotation, GetSize(), SpriteEffects.None, 0f);

            Utilities.drawTextInDrawMethod(activeProjectiles.Count.ToString(), Utilities.CentreOfScreen() * 1.5f, spriteBatch, font, Color.White);
        }
    }
}

