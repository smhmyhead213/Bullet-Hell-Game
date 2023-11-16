using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class ChainLink
    {
        public Vector2 Position;
        public Vector2 End;
        public float Rotation;
        public float RotationalVelocity;
        public float RotationalAccleration;
        public float Length;
        public float DampingFactor;
        public EyeBoss Owner;
        public Texture2D Texture;

        public int AITimer;
        public ChainLink(string texture, Vector2 position, float rotation, float length, EyeBoss owner)
        {
            Texture = Assets[texture];
            Position = position;
            Rotation = rotation; // these rotations should be angles from vertical
            Length = length;
            End = CalculateEnd();
            AITimer = 0;
            Owner = owner;
        }

        public void Update()
        {
            float gravitationalAcceleration = 0.3f;
            
            // the unbalanced force due to gravity on the pendulum is mgsin theta.
            // the objects acceleration is gsin(theta)
            // dividing both sides by r (the pendulums length) gives an angular accleration.

            RotationalAccleration = gravitationalAcceleration * Sin(Rotation) / Length;

            RotationalVelocity = RotationalVelocity + RotationalAccleration;

            RotationalVelocity = RotationalVelocity * DampingFactor; // damping;

            Rotation = Rotation + RotationalVelocity;

            End = CalculateEnd();

            AITimer++;
        }

        public void SetDampingFactor(float dampingFactor)
        {
            DampingFactor = dampingFactor;
        }
        public Vector2 CalculateEnd()
        {
            // counter clockwise because we are considering swinging right to give a positive rotation

            return Position + Length * Utilities.RotateVectorClockwise(-Vector2.UnitY, Rotation);
        }
        public void Draw(SpriteBatch s)
        {
            s.Draw(Texture, Position, null, Color.White, Rotation - PI / 2, new Vector2(0, Texture.Height / 2f), new Vector2(Length / Texture.Width, 1f), SpriteEffects.None, 1);
            //Drawing.BetterDraw(Texture, CalculateEnd(), null, Color.Yellow, 0, Vector2.One, SpriteEffects.None, 1);
            //Drawing.BetterDraw(Texture, Position, null, Color.Purple, 0, Vector2.One, SpriteEffects.None, 1);
        }
    }
}
