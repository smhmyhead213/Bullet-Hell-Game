using bullethellwhatever.AssetManagement;
using bullethellwhatever.DrawCode;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabScuttler
    {
        public enum ScuttlerState
        {
            Idle,
            MovingBackwards,
            MovingForwards
        }

        public Vector2 Position;
        public Vector2 EndPosition
        {
            get;
            set;
        }

        public Vector2 TargetPosition;
        public float UpperLength;
        public float LowerLength;
        public Texture2D Texture;
        public int BendSign;
        public bool TryingToReachTarget;
        public int MoveForwardTimer;
        public int MoveBackwardTimer;
        public Color Colour;
        public CrabScuttler(Vector2 position, Vector2 endPosition, float upperLength, float lowerLength, string texture, int bendSign)
        {
            Position = position;
            EndPosition = endPosition;
            UpperLength = upperLength;
            LowerLength = lowerLength;
            Texture = AssetRegistry.GetTexture2D(texture);
            BendSign = bendSign;
            TryingToReachTarget = false;
            MoveForwardTimer = 0;
            MoveBackwardTimer = 0;
            Colour = Color.White;
        }

        public void Update(Vector2 rootVelocity)
        {
            float distanceToBeginMovingForth = Length();
            int timeToReachTarget = 200;
            float progress = (float)MoveForwardTimer / timeToReachTarget;

            if (Position.Distance(EndPosition) > distanceToBeginMovingForth && !TryingToReachTarget)
            {
                TryingToReachTarget = true;
                MoveForwardTimer = 0;
            }

            if (TryingToReachTarget)
            {
                TargetPosition = Position + Utilities.SafeNormalise(rootVelocity) * (1 - progress) * Length() + Utilities.SafeNormalise(rootVelocity.Rotate(BendSign * PI / 2)) * Length() / 2f;
                EndPosition = Vector2.Lerp(EndPosition, TargetPosition, progress);

                MoveForwardTimer++;

                if (MoveForwardTimer == timeToReachTarget)
                {
                    TryingToReachTarget = false;
                }
            }

            Colour = TryingToReachTarget ? Color.Red : Color.Green;
        }

        public float Length()
        {
            return UpperLength + LowerLength;
        }
        public void Draw(SpriteBatch s)
        {
            Vector2 elbowPosition;

            float[] rotations = MathsUtils.SolveTwoPartIK(Position, EndPosition, UpperLength, LowerLength, out elbowPosition, BendSign);

            Texture2D texture = AssetRegistry.GetTexture2D("box");
            float scuttlerWidth = 10f;
            Vector2 upperPartScale = new Vector2(scuttlerWidth / texture.Width, UpperLength / texture.Height);
            Vector2 lowerPartScale = new Vector2(scuttlerWidth / texture.Width, LowerLength / texture.Height);

            Drawing.BetterDraw(texture, Position, null, Colour, rotations[0] + PI, upperPartScale, SpriteEffects.None, 0f, new Vector2(texture.Width / 2f, 0f));
            Drawing.BetterDraw(texture, elbowPosition, null, Colour, rotations[1] + PI, lowerPartScale, SpriteEffects.None, 0f, new Vector2(texture.Width / 2f, 0f));

            //Drawing.DrawBox(Position, Color.Red, 1f);
            //Drawing.DrawBox(EndPosition, Color.Yellow, 1f);
            //Drawing.DrawBox(TargetPosition, Color.White, 1f);
            //Drawing.DrawBox(elbowPosition, Color.Orange, 1f);
        }
    }
}
