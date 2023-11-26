using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles;
using bullethellwhatever.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Bosses.CrabBoss.Projectiles
{
    public class BigMassiveOrb : Projectile, IDrawsShader
    {
        public float ExpansionRate;
        public int Lifetime;
        public bool KeepExpanding;
        public int FadeOutTime;
        public float SizeAtDeath;
        public bool Bounce;
        public BigMassiveOrb(float expansionRate, int lifetime)
        {
            ExpansionRate = expansionRate;
            Lifetime = lifetime;
            KeepExpanding = true;
            FadeOutTime = 30;
        }

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, int pierce, string texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            base.Spawn(position, velocity, damage, pierce, texture, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            Bounce = true;
        }

        public override void Spawn(Vector2 position, Vector2 velocity, float damage, int pierce, Texture2D texture, float acceleration, Vector2 size, Entity owner, bool isHarmful, Color colour, bool shouldRemoveOnEdgeTouch, bool removeOnHit)
        {
            base.Spawn(position, velocity, damage, pierce, texture, acceleration, size, owner, isHarmful, colour, shouldRemoveOnEdgeTouch, removeOnHit);

            Bounce = true;
        }

        public override void AI()
        {
            base.AI();

            if (touchingAnEdge(this))
            {
                HandleBounces();

                if (Bounce)
                {
                    Drawing.ScreenShake(10, 6);

                    float offset = 0;

                    int projectilesPerRing = Utilities.ValueFromDifficulty(15, 25, 32, 40);

                    for (int i = 0; i < projectilesPerRing; i++)
                    {
                        ExplodingProjectileFragment projectile = new ExplodingProjectileFragment();

                        projectile.Spawn(Position, 3f * Utilities.RotateVectorClockwise(-Vector2.UnitY, (PI * 2 / projectilesPerRing * i) + offset),
                        1f, 1, "box", 1f, Vector2.One, Owner, true, Color.Red, false, false);
                    }
                }

                else if (AITimer > 10)
                {
                    Die();
                }
            }

            if (KeepExpanding && AITimer <= Lifetime - FadeOutTime) //dont expand if fading out
            {
                Size = Size + new Vector2(ExpansionRate, ExpansionRate);
                SizeAtDeath = Size.X + ExpansionRate;
            }

            if (AITimer > Lifetime - FadeOutTime)
            {
                Size = Size - (new Vector2(SizeAtDeath, SizeAtDeath) / FadeOutTime);
            }


            if (AITimer == Lifetime)
            {
                Die();
            }
        }

        public void DrawWithShader(SpriteBatch spriteBatch)
        {

        }

        public void SetExpanding(bool expand)
        {
            KeepExpanding = expand;
        }

        public override void Die()
        {
            DeleteNextFrame = true; // exempt from fading out as this has its own fade out specifically

            if (OnDeath is not null)
            {
                OnDeath();
            }
        }
        public override void Draw(SpriteBatch s)
        {
            //Shader.Parameters["uTime"]?.SetValue(AITimer);
            //Shader.Parameters["duration"]?.SetValue(Duration);
            Shader = Shaders["BigMassiveOrbShader"];

            Shader.CurrentTechnique.Passes[0].Apply();

            base.Draw(s);
        }
    }
}
