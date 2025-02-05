using bullethellwhatever.AssetManagement;
using bullethellwhatever.DrawCode;
using bullethellwhatever.NPCs;
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
            Texture = AssetRegistry.GetTexture2D(texture);
            DistanceFromEyeCentre = distanceFromEyeCentre;
            RotationWithinEye = rotationWithinEye;
            Scale = size;
            InitialSize = Scale;
            TargetableByHoming = false;
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

        public void ResetSize()
        {
            Scale = InitialSize;
        }

        public void Dilate(Vector2 sizeToDilateTo, float progress)
        {
            Scale = Vector2.Lerp(InitialSize, sizeToDilateTo, progress);
        }
        public void Dilate(Vector2 initialSize, Vector2 sizeToDilateTo, float progress)
        {
            Scale = Vector2.Lerp(initialSize, sizeToDilateTo, progress);
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
            Drawing.BetterDraw(Texture, Position, null, Color.Black, 0, Scale, SpriteEffects.None, 1);
        }
    }
}
