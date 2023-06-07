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
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Enemies;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class TestBoss : Boss
    {
        public bool HasChosenChargeDirection;
        public float SpiralStartTime;
        
        public float ShotgunFrequency;
        public float DespBombFrequencyInitially;
        public float DespBombFrequency;
        public bool HasDesperationStarted;
        public float DespBombTimer;
        public int DespBombCounter;
        public int DespBeamRotation;

        public TestBoss(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            isBoss = true;
            isPlayer = false;
            Health = 200;
            AITimer = 0;
            MaxIFrames = 5;
            HasChosenChargeDirection = false;
            AttackNumber = 1;
            ShotgunFrequency = 15f;
            DespBombFrequencyInitially = 40f;
            DespBombFrequency = DespBombFrequencyInitially;
            DespBombTimer = 0f;
            DespBombCounter = 0;
            HasDesperationStarted = false;
            IsDesperationOver = false;

            Main.musicSystem.SetMusic("TestBossMusic", true, 0.01f);

            FramesPerMusicBeat = 24;
            BeatsPerBar = 4;
            BarDuration = FramesPerMusicBeat * BeatsPerBar;

            BossAttacks = new BossAttack[]
            { new Desperation(BarDuration * 30),
                new EnemySpam(BarDuration * 16),
                new BasicShotgunSpread(BarDuration * 18),
                new Charge(BarDuration * 16),
                new Spiral(BarDuration * 12),
                new SpawnEnemies(BarDuration),
                new LaserBarrages(BarDuration * 7),
                new MoveTowardsAndShotgun(BarDuration * 7),
                new ExplodingProjectiles(BarDuration * 10),             
                new MutantBulletHell(BarDuration * 12),                
                new LiterallyJustABulletHell(BarDuration * 35),
                new HorizontalChargesWithProjectiles(BarDuration * 13),
                new Circle(BarDuration * 17),
                new EnemySpam(BarDuration * 16),
            };

            for (int i = 0; i < BossAttacks.Length; i++)
            {
                BossAttacks[i].Owner = this;
                BossAttacks[i].InitialiseAttackValues();
            }
        }
        public override void Spawn(Vector2 position, Vector2 initialVelocity, float damage, string texture, Vector2 size, float MaxHealth, int pierceToTake, Color colour, bool shouldRemoveOnEdgeTouch, bool isHarmful)
        {
            base.Spawn(position, initialVelocity, damage, texture, size, MaxHealth, pierceToTake, colour, shouldRemoveOnEdgeTouch, isHarmful);
        }

        public override void AI()
        {
            CurrentBeat = (int)(((float)AITimer / BarDuration - MathF.Floor(AITimer / BarDuration)) * 4) + 1;

            JustStartedBeat = AITimer % FramesPerMusicBeat == 0 ? true : false;

            BossAttacks[AttackNumber].TryEndAttack(ref AITimer, ref AttackNumber);

            BossAttacks[AttackNumber].Execute(ref AITimer, ref AttackNumber);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Drawing.BetterDraw(Texture, Position, null, Colour, Rotation, Size, SpriteEffects.None, 0f);

            Utilities.drawTextInDrawMethod(MathF.Floor(AITimer / BarDuration).ToString(), new Vector2(Main.ScreenWidth / 6, Main.ScreenHeight / 6), spriteBatch, Main.font, Colour);
        }
    }
}

