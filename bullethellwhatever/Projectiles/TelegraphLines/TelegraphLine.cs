using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;

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
        public string LineShader;
        public bool SpawnRayAfterFinish;
        public Deathray ToSpawn;
        public TelegraphLine(float rotation, float rotationalVelocity, float rotationalAcceleration, float width, float length, int duration, Vector2 origin, Color colour, string texture, Entity owner, bool stayWithOwner)
        {
            Rotation = rotation;

            SpawnRayAfterFinish = false;

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

            LineShader = "TelegraphLineShader";
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
                if (SpawnRayAfterFinish)
                {
                    ToSpawn.SpawnDeathray(ToSpawn.Position, ToSpawn.Rotation, ToSpawn.Damage, ToSpawn.Duration, ToSpawn.Texture, ToSpawn.Width, ToSpawn.Length, ToSpawn.AngularVelocity, ToSpawn.Acceleration, ToSpawn.IsHarmful, ToSpawn.Colour, ToSpawn.Shader, ToSpawn.Owner);
                }
            }

        }

        public void SpawnDeathrayOnDeath(float damage, int duration, float angularVelocity, float angularAcceleration, bool isHarmful, Color colour, string? shader, Entity owner)
        {
            SpawnRayAfterFinish = true;
            ToSpawn = new Deathray();
            ToSpawn.CreateDeathray(Origin, Rotation, damage, duration, "box", Width, Length, angularVelocity, angularAcceleration, isHarmful, colour, shader, owner);
        }
        public void ChangeShader(string shaderName)
        {
            LineShader = shaderName;
        }
        public void Draw(SpriteBatch spritebatch)
        {
            Shaders[LineShader].Parameters["uTime"]?.SetValue(TimeAlive);
            Shaders[LineShader].Parameters["AngularVelocity"]?.SetValue(RotationalVelocity);
            Shaders[LineShader].Parameters["duration"]?.SetValue(Duration);

            Shaders[LineShader].CurrentTechnique.Passes[0].Apply();

            Vector2 size = new Vector2(Width / Texture.Width, Length / Texture.Height); //Scale the beam up to the required width and length.

            Vector2 originOffset = new Vector2(5f, 0f); //i have no idea why the value 5 works everytime i have genuinely no clue

            spritebatch.Draw(Main.player.Texture, Origin, null, Colour, Rotation + MathF.PI, originOffset, size, SpriteEffects.None, 0);
        }
    }
}
