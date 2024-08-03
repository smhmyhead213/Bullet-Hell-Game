using bullethellwhatever.MainFiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.AssetManagement;

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
        public float InitialWidth;

        public Deathray ToSpawn;
        public Action ExtraAI;
        public Action OnDeath;

        public bool ThickenIn;
        public TelegraphLine(float rotation, float rotationalVelocity, float width, float length, int duration, Vector2 origin, Color colour, string texture, Entity owner, bool stayWithOwner)
        {
            Rotation = rotation;

            RotationalVelocity = rotationalVelocity;
            Width = width;
            InitialWidth = Width;
            Length = length;
            Duration = duration;
            Origin = origin;
            Colour = colour;
            Texture = AssetRegistry.GetTexture2D(texture);
            Owner = owner;
            TimeAlive = 0;
            StayWithOwner = stayWithOwner;

            ThickenIn = false;

            LineShader = "TelegraphLineShader";
            Owner.activeTelegraphs.Add(this);
        }

        public void ShouldThickenIn(bool thicken)
        {
            ThickenIn = thicken;
        }

        public void SetExtraAI(Action action)
        {
            ExtraAI = action;
        }

        public void AI()
        {
            TimeAlive++;
            
            if (StayWithOwner)
                Origin = Owner.Position;

            Rotation = Rotation + RotationalVelocity;

            if (RotationalAcceleration != 0)
                RotationalVelocity = RotationalVelocity + RotationalAcceleration; //accel linearly

            Rotation = Rotation % (PI * 2);

            if (TimeAlive > Duration)
            {
                DeleteNextFrame = true;

                if (OnDeath is not null)
                {
                    OnDeath();
                }
            }

            float maxThicknessTime = 3 * Duration / 4;

            if (ThickenIn && TimeAlive < maxThicknessTime)
            {
                // ln(x) normally reaches 1 at x = e.
                // we want it reaching 1 at x = duration / 8
                // ln(x/2) reaches y = 1 at x = 2e.
                // so ln(x/a) reaches y = 1 at x = a * e.
                // so ln(x/a) reaches y = 1 at x = duration / 8 when a = (duration / 8) / e

                Width = InitialWidth * Log(TimeAlive / (maxThicknessTime / E));
            }

            if (ExtraAI is not null)
            {
                ExtraAI();
            }
        }

        public void SetOnDeath(Action action)
        {
            OnDeath = action;
        }

        public void ChangeShader(string shaderName)
        {
            LineShader = shaderName;
        }
        public void Draw(SpriteBatch spritebatch)
        {
            Effect lineShader = AssetRegistry.GetShader(LineShader);

            lineShader.Parameters["uTime"]?.SetValue(TimeAlive);
            lineShader.Parameters["AngularVelocity"]?.SetValue(RotationalVelocity);
            lineShader.Parameters["duration"]?.SetValue(Duration);

            lineShader.Parameters["colour"]?.SetValue(Colour.ToVector3());

            lineShader.CurrentTechnique.Passes[0].Apply();

            Vector2 size = new Vector2(Width / Texture.Width, Length / Texture.Height); //Scale the beam up to the required width and length.

            Vector2 originOffset = new Vector2(5f, 0f); //i have no idea why the value 5 works everytime i have genuinely no clue

            spritebatch.Draw(Texture, Origin, null, Colour, Rotation + PI, originOffset, size, SpriteEffects.None, 0);
        }
    }
}
