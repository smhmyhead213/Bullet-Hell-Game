using bullethellwhatever.MainFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles.TelegraphLines;
using System.Security.Cryptography;
using System.Xml;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class Stomp : BossAttack
    {
        public int CompletedStomps;
        public int TargetingDuration;
        public int StompDuration;
        public int StompFrequency;
        public float DistanceToBottom;
        public int NumberOfProjectiles;

        public Stomp(int endTime) : base(endTime)
        {
            EndTime = endTime;
            CompletedStomps = 0;
        }

        public override void InitialiseAttackValues()
        {
            StompFrequency = Owner.BarDuration;

            if (GameState.Difficulty == GameState.Difficulties.Easy || GameState.Difficulty == GameState.Difficulties.Normal)
            {
                StompFrequency = Owner.BarDuration * 2;
            }

            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    NumberOfProjectiles = 4;
                    break;
                case GameState.Difficulties.Normal:
                    NumberOfProjectiles = 6;
                    break;
                case GameState.Difficulties.Hard:
                    NumberOfProjectiles = 8;
                    break;
                case GameState.Difficulties.Insane:
                    NumberOfProjectiles = 10;
                    break;
            }
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int halfStomp = StompFrequency / 2;
            int time = AITimer % StompFrequency;


            if (time == 0)
            {
                CompletedStomps = 0; //reset
            }

            if (time % StompFrequency <= halfStomp)
            {
                MoveToPoint(Main.player.Position + new Vector2(0, -600), time, halfStomp);
                Owner.ContactDamage = false;
            }

            if (time % StompFrequency > halfStomp)
            {
                Owner.ContactDamage = true;

                DistanceToBottom = Main.ScreenHeight - Owner.Position.Y - (Owner.Texture.Height * Owner.GetSize().Y);

                int timeSinceDrop = time - halfStomp;

                float accel = 2f * DistanceToBottom / MathF.Pow(halfStomp - timeSinceDrop, 2);

                Owner.Velocity = new Vector2(0f, accel * timeSinceDrop);
            }

            if (time % StompFrequency == StompFrequency - 1) //fix
            {
                CompletedStomps++;
                Drawing.ScreenShake(5, 20);

                for (int i = (int)-MathF.Floor(NumberOfProjectiles / 2); i < NumberOfProjectiles + (int)-MathF.Floor(NumberOfProjectiles / 2); i++)
                {
                    Random rng = new Random();

                    float angleVariation = (MathF.PI / NumberOfProjectiles) / 4;

                    Projectile p = new Projectile();

                    p.Spawn(Owner.Position, 5f * Utilities.AngleToVector((i + 1) * MathF.PI / NumberOfProjectiles + (float)rng.NextDouble() * angleVariation - angleVariation * 2), 1f, 1, "box", 1, Vector2.One, Owner, true, Color.Red, true, false);
                }
            }

        }
    }
}
