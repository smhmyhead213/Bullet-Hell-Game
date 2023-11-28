using bullethellwhatever.BaseClasses;
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
    public class Pupil : NPC
    {
        public float DistanceFromEyeCentre;
        public float RotationWithinEye;
        public Vector2 InitialSize;
        public Vector2 EyeCentre;

        //public Texture2D Texture;
        //public Vector2 Size;
        //public Vector2 Position;

        public Pupil(string texture, float distanceFromEyeCentre, float rotationWithinEye, Vector2 size)
        {
            Texture = Assets[texture];
            DistanceFromEyeCentre = distanceFromEyeCentre;
            RotationWithinEye = rotationWithinEye;
            Size = size;
            InitialSize = Size;
        }

        public void Update(Vector2 eyeCentre)
        {
            EyeCentre = eyeCentre;

            while (RotationWithinEye > Tau)
            {
                RotationWithinEye = RotationWithinEye - Tau;
            }

            while (RotationWithinEye < 0)
            {
                RotationWithinEye = RotationWithinEye + Tau;
            }
        }
        public void Rotate(float angle)
        {
            RotationWithinEye = RotationWithinEye + angle;
        }
        public void LookAtPlayer(float distanceFromCentre)
        {
            DistanceFromEyeCentre = distanceFromCentre;
            RotationWithinEye = Utilities.AngleToPlayerFrom(EyeCentre);
        }

        public void GoTo(float distanceFromEyeCentre, float rotation)
        {
            DistanceFromEyeCentre = distanceFromEyeCentre;
            RotationWithinEye = rotation;
        }
        public void CalculatePosition()
        {
            Position = EyeCentre + DistanceFromEyeCentre * Utilities.RotateVectorClockwise(-Vector2.UnitY, RotationWithinEye);
        }
        public void Draw()
        {
            Drawing.BetterDraw(Texture, Position, null, Color.Black, 0, Size, SpriteEffects.None, 1);
        }
    }
}
