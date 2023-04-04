using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace bullethellwhatever
{
    public class OscillatingSpeedProjectile : BasicProjectile
    {
        public float OscillationFrequency;

        public OscillatingSpeedProjectile(float oscillationFrequency)
        {
            OscillationFrequency = oscillationFrequency;
        }

        public override void AI() //and drawing
        {
            TimeAlive++;

            if (Acceleration != 0)
                Velocity = Velocity * Acceleration; //acceleration values must be very very small

            float speed = (MathF.Sin(TimeAlive / 20f) + 1f);

            Velocity = speed * Utilities.SafeNormalise(Velocity, Vector2.Zero);

            Position = Position + Velocity;

            if (Updates > 1)
            {
                Main._spriteBatch.Begin();
                Main._spriteBatch.Draw(Texture, Position, null, Colour(), 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
                Main._spriteBatch.End();
            }


        }
    }
}
