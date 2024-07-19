using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class ExplodingProjectiles : BossAttack
    {
        public int NumberOfProjectiles;

        public ExplodingProjectiles(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    NumberOfProjectiles = 10;
                    break;
                case GameState.Difficulties.Normal:
                    NumberOfProjectiles = 15;
                    break;
                case GameState.Difficulties.Hard:
                    NumberOfProjectiles = 20;
                    break;
                case GameState.Difficulties.Insane:
                    NumberOfProjectiles = 25;
                    break;
            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            Owner.Velocity = 0.55f * Utilities.Normalise(Main.player.Position - Owner.Position);

            int startTime = 270;
            int endTime = 720;
            int timeBetween = Owner.BarDuration / 2;

            int explosionDelay = endTime - AITimer;

            if (AITimer % timeBetween == 0 && AITimer <= endTime && AITimer >= startTime)
            {
                Vector2 direction = Utilities.RotateVectorClockwise(Main.player.Position - Owner.Position, Utilities.ToRadians(AITimer - 250f));

                //Projectile explodingProjectile = new Projectile(NumberOfProjectiles, Owner.BarDuration, 0, true, true, false);

                Projectile explodingProjectile = SpawnProjectile(Owner.Position, 15f * Utilities.SafeNormalise(direction, Vector2.Zero), 1f, 1, "box", new Vector2(2, 2), Owner, true, Color.Red, false, false);
                
                explodingProjectile.SetExtraAI(new Action(() =>
                {
                    if (explodingProjectile.AITimer == Owner.BarDuration)
                    {
                        for (int i = 0; i < NumberOfProjectiles; i++)
                        {
                            Deathray ray = new Deathray();

                            //make it explode based on its velocity, see Supreme Calamitas gigablasts exploding based on orientation

                            ray.SpawnDeathray(explodingProjectile.Position, i * MathHelper.TwoPi / NumberOfProjectiles, 1f, 10, "box", 10, 2000, 0, true, Color.Red, "DeathrayShader2", Owner);

                            explodingProjectile.DeleteNextFrame = true;
                        }
                    }
                })); ;
            }

            Owner.Rotation = Utilities.ToRadians(AITimer - 250f);
        }
    }
}
