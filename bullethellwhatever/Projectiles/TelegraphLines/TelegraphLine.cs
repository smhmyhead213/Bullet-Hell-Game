using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Projectiles.TelegraphLines
{
    public class TelegraphLine
    {
        public float Rotation; // From the vertical.
        public float RotationalVelocity;
        public float RotationalAcceleration;
        public int Duration;
        public Vector2 Origin;
        public Entity Owner;
        public Color Colour;
        public Texture2D Texture;
        public float Width;
        public float Length;
        public int TimeAlive;
        public TelegraphLine(float rotation, float rotationalVelocity, float rotationalAcceleration, float width, float length, int duration, Vector2 origin, Color colour, Texture2D texture, Entity owner)
        {
            Rotation = rotation;
            RotationalVelocity = rotationalVelocity;
            RotationalAcceleration = rotationalAcceleration;
            Width = width;
            Length = length;
            Duration = duration;
            Origin = origin;
            Colour = colour;
            Texture = texture;
            Owner = owner;
            TimeAlive = 0;

            Owner.activeTelegraphs.Add(this);
        }

        public void AI()
        {
            TimeAlive++;

            Origin = Owner.Position;

            Rotation = Rotation + RotationalVelocity;

            if (RotationalAcceleration != 0)
                Rotation = Rotation * RotationalAcceleration;

            Rotation = (Rotation + MathF.PI * RotationalVelocity / 21600f) % (MathF.PI * 2);

        }
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.End();

            spritebatch.Begin(SpriteSortMode.Immediate);

            Main.telegraphLineShader.Parameters["uTime"]?.SetValue(TimeAlive);
            Main.telegraphLineShader.Parameters["AngularVelocity"]?.SetValue(RotationalVelocity);
            Main.telegraphLineShader.Parameters["noiseMap"]?.SetValue(Main.deathrayNoiseMap);

            Main.telegraphLineShader.CurrentTechnique.Passes[0].Apply();

            Vector2 size = new Vector2(Width / Texture.Width, Length / Texture.Height); //Scale the beam up to the required width and length.

            Vector2 originOffset = new Vector2(5f, 0f); //i have no idea why the value 5 works everytime i have genuinely no clue

            spritebatch.Draw(Main.player.Texture, Origin, null, Colour, Rotation, originOffset, size, SpriteEffects.None, 0);

            spritebatch.End();

            spritebatch.Begin(SpriteSortMode.Deferred);


        }
    }
}
