using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.Bosses.EyeBoss;
using bullethellwhatever.Projectiles;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode
{
    public class ShockwaveRing : Projectile
    {
        public float Radius;
        public int LifeTime;
        public float ExpansionRate;
        public int FadeOutTime;
        public ShockwaveRing(float radius, float expansionRate, int lifeTime, int fadeOutTime)
        {
            Radius = radius;
            ExpansionRate = expansionRate;
            Participating = false;
            IsEffect = true;
            LifeTime = lifeTime;
            FadeOutTime = fadeOutTime;
            //add a set shader call
        }

        public void Spawn(Vector2 position, Entity owner, Color colour)
        {
            Rotation = Utilities.RandomAngle();

            Colour = colour;

            SetShader("ShockwaveShader");

            SetOpacity(0.5f);

            this.SpawnProjectile(position, Vector2.Zero, 0f, 1, "box", Vector2.Zero, owner, false, Color.White, false, false);            
        }
        public override void Update()
        {
            base.Update();

            Radius = Radius + ExpansionRate;

            if (AITimer < LifeTime - FadeOutTime)
            {
                Opacity = MathHelper.Lerp(InitialOpacity, Opacity, (LifeTime - AITimer) / (float)FadeOutTime);
            }

            if (AITimer >= LifeTime)
            {
                Die();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Shader.SetParameter("radius", 0.5f);

            ApplyRandomNoise();

            Shader.Apply();

            Texture2D texture = AssetRegistry.GetTexture2D("box");

            Drawing.BetterDraw(texture, Position, null, Color.White, Rotation, Vector2.One * Radius / texture.Width * 2f, SpriteEffects.None, 1);
        }
    }
}
