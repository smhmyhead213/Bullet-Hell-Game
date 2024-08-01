using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.NPCs;
using bullethellwhatever.AssetManagement;

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
        public float Torque;
        public NPC Owner;
        public Texture2D Texture;
        public RotatedRectangle Hitbox;
        public float Opacity;
        public int AITimer;
        public Color Colour;
        public ChainLink(string texture, Vector2 position, float rotation, float length, NPC owner)
        {
            Texture = AssetRegistry.GetTexture2D(texture);
            Position = position;
            Rotation = rotation; // these rotations should be angles from vertical
            Length = length;
            End = CalculateEnd();
            AITimer = 0;
            Owner = owner;
            Opacity = 1f;
            Colour = Color.White;

            UpdateHitbox();
        }
        public void Update()
        {
            float gravitationalAcceleration = 0.3f;
            
            // the unbalanced force due to gravity on the pendulum is mgsin theta.
            // the objects acceleration is gsin(theta)
            // dividing both sides by r (the pendulums length) gives an angular accleration.

            RotationalAccleration = gravitationalAcceleration * Sin(Rotation) / Length + Torque;

            Torque = 0; // apply the torque once only, do for multiple frames

            RotationalVelocity = RotationalVelocity + RotationalAccleration;

            RotationalVelocity = RotationalVelocity * DampingFactor; // damping;

            Rotation = Rotation + RotationalVelocity;

            End = CalculateEnd();

            AITimer++;

            UpdateHitbox();
        }

        public void UpdateHitbox()
        {
            Hitbox = new RotatedRectangle(Rotation, Texture.Width, Length, Position + Length / 2f * Utilities.RotateVectorClockwise(Vector2.UnitY, Rotation), Owner);
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

        public void Rotate(float angle)
        {
            Rotation = Rotation + angle;
        }

        public void ApplyTorque(float torque)
        {
            Torque = torque;
        }
        public void Draw(SpriteBatch s)
        {
            s.Draw(Texture, Position, null, Colour * Opacity, Rotation - PI / 2, new Vector2(0, Texture.Height / 2f), new Vector2(Length / Texture.Width, 1f), SpriteEffects.None, 1);

            //Hitbox.DrawHitbox();
            //Drawing.BetterDraw(Texture, CalculateEnd(), null, Color.Yellow, 0, Vector2.One, SpriteEffects.None, 1);
            //Drawing.BetterDraw(Texture, Position, null, Color.Purple, 0, Vector2.One, SpriteEffects.None, 1);
        }
    }
}
