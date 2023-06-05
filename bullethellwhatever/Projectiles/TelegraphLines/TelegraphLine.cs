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
        public bool DeleteNextFrame;
        public bool StayWithOwner;
        public TelegraphLine(float rotation, float rotationalVelocity, float rotationalAcceleration, float width, float length, int duration, Vector2 origin, Color colour, string texture, Entity owner, bool stayWithOwner)
        {
            Rotation = rotation;

            RotationalVelocity = rotationalVelocity;
            RotationalAcceleration = rotationalAcceleration;
            Width = width;
            Length = length;
            Duration = duration;
            Origin = origin;
            Colour = colour;
            Texture = Main.Assets[texture];
            Owner = owner;
            TimeAlive = 0;
            StayWithOwner = stayWithOwner;

            Owner.activeTelegraphs.Add(this);
        }

        public void AI()
        {
            TimeAlive++;
            
            if (StayWithOwner)
                Origin = Owner.Position;

            Rotation = Rotation + RotationalVelocity;

            if (RotationalAcceleration != 0)
                RotationalVelocity = RotationalVelocity + RotationalAcceleration; //accel linearly

            Rotation = Rotation % (MathF.PI * 2);

            if (TimeAlive > Duration)
            {
                DeleteNextFrame = true;
            }

        }
        public void Draw(SpriteBatch spritebatch)
        {
            Main.Shaders["TelegraphLineShader"].Parameters["uTime"]?.SetValue(TimeAlive);
            Main.Shaders["TelegraphLineShader"].Parameters["AngularVelocity"]?.SetValue(RotationalVelocity);
            Main.Shaders["TelegraphLineShader"].Parameters["duration"]?.SetValue(Duration);

            Main.Shaders["TelegraphLineShader"].CurrentTechnique.Passes[0].Apply();

            Vector2 size = new Vector2(Width / Texture.Width, Length / Texture.Height); //Scale the beam up to the required width and length.

            Vector2 originOffset = new Vector2(5f, 0f); //i have no idea why the value 5 works everytime i have genuinely no clue

            spritebatch.Draw(Main.player.Texture, Origin, null, Colour, Rotation + MathF.PI, originOffset, size, SpriteEffects.None, 0);
        }
    }
}
