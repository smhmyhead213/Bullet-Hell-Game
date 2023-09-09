using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Bosses.CrabBoss.Projectiles
{
    public class BigMassiveOrb : Projectile, IDrawsShader
    {
        public float ExpansionRate;
        public int Lifetime;
        public BigMassiveOrb(float expansionRate, int lifetime)
        {
            ExpansionRate = expansionRate;
            Lifetime = lifetime;
        }
        public override void AI()
        {
            base.AI();
            Size = Size + new Vector2(ExpansionRate, ExpansionRate);

            if (AITimer == Lifetime)
            {
                Die();
            }
        }

        public void DrawWithShader(SpriteBatch spriteBatch)
        {

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
