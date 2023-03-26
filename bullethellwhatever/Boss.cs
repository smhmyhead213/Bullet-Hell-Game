using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace bullethellwhatever
{
    public class Boss : NPC
    {
        public bool HasChosenChargeDirection;
        public Boss(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            isBoss = true;
            isPlayer = false;
            Health = 100;
            AITimer = 0f;
            IFrames = 5f;
            HasChosenChargeDirection = false;
        }

        public override bool ShouldRemoveOnEdgeTouch() => false;

        public override void Spawn(Vector2 position, Vector2 initialVelocity, float damage, Texture2D texture, float size)
        {
            base.Spawn(position, initialVelocity, damage, texture, size);

        }

        public override void HandleMovement()
        {

        }

        public override void AI()
        {
            if (Health < 0)
                DeleteNextFrame = true;

            //Every frame, add 1 to the timer.
            AITimer++;

            //Update the boss position based on its velocity.
            Position = Position + Velocity; 

            
            if (IFrames > 0)
            {
                IFrames--;
            }
            //If the timer reaches 10, execute the attack and reset timer.


            if (AITimer % 40 == 0 && AITimer < 550)
            {
                BasicShotgunBlast(Position, 5f, 24);
            }

            if (AITimer == 330)
                HasChosenChargeDirection = false;

            if (AITimer > 330 && AITimer < 500)
            {
                Charge();
            }

            //if (AITimer % 300 == 0)
            //    HasChosenChargeDirection = false;

            if (AITimer > 550)
                AITimer = 0;





            foreach (FriendlyProjectile projectile in Main.activeFriendlyProjectiles)
            {
                if (isCollidingWithPlayerProjectile(projectile) && IFrames == 0)
                {
                    IFrames = 5f;
                    Health = Health - projectile.Damage;
                    projectile.DeleteNextFrame = true;
                    
                }
            }
        }

        public void BasicShotgunBlast(Vector2 bossPosition, float projectileSpeed, int numberOfProjectiles)
        {

            

            BasicProjectile singleShot = new BasicProjectile();
            singleShot.Spawn(bossPosition, projectileSpeed * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, 1);

            for (int i = 1; i < (numberOfProjectiles / 2) + 0.5f ; i++) // loop for each pair of projectiles an angle away from the middle
            {
                BasicProjectile shotgunBlast = new BasicProjectile();
                BasicProjectile shotgunBlast2 = new BasicProjectile(); //one for each side of middle
                shotgunBlast.Spawn(bossPosition, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - bossPosition, i * MathF.PI / 12)), 1f, Texture, 1);
                shotgunBlast2.Spawn(bossPosition, projectileSpeed * Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - bossPosition, i * MathF.PI / 12)), 1f, Texture, 1);

            }

            HandleBounces();
            
        }

        public void Charge()
        {
            if (!HasChosenChargeDirection)
            {
                Velocity = 7.5f * Utilities.Normalise(Main.player.Position - Position);
                HasChosenChargeDirection = true;
            }

            HandleBounces();
        }

        public void HandleBounces()
        {
            if (touchingLeft(this))
            {
                if (Velocity.X < 0)
                    Velocity.X = Velocity.X * -1;
            }

            if (touchingRight(this, Main._graphics.PreferredBackBufferWidth))
            {
                if (Velocity.X > 0)
                    Velocity.X = Velocity.X  * -1;
            }

            if (touchingTop(this))
            {
                if (Velocity.Y < 0)
                    Velocity.Y = Velocity.Y * -1f;

            }

            if (touchingBottom(this, Main._graphics.PreferredBackBufferHeight))
            {
                if (Velocity.Y > 0)
                    Velocity.Y = Velocity.Y * -1f;
            }
        }

    }
}
