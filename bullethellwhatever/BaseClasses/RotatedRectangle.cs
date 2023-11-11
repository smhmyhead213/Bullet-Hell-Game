using bullethellwhatever.Projectiles.Base;
using System;
using Microsoft.Xna.Framework;
using bullethellwhatever.MainFiles;
using bullethellwhatever.BaseClasses;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.Projectiles.TelegraphLines;

namespace bullethellwhatever.BaseClasses
{
    public class RotatedRectangle
    {
        public float Rotation;
        public float Width; //x
        public float Length; //y
        //public Vector2 AxisOfRotation;
        public Vector2 Centre;
        public Vector2[] Vertices;
        public Entity Owner;
        public RotatedRectangle(float rotation, float width, float length, Vector2 centre, Entity owner)
        {
            UpdateRectangle(rotation, width, length, centre);
            Vertices = new Vector2[4];
            Owner = owner;
            UpdateVertices();
        }
        public void UpdateRectangle(float rotation, float width, float length, Vector2 centre)
        {
            Rotation = rotation;
            Width = width;
            Length = length;
            Centre = centre;
        }

        public float DiagonalLength()
        {
            return Sqrt(Length * Length + Width * Width);
        }
        public void UpdateVertices()
        {
            // 0 and 3 are opposite each other, 1 and 2 likewise 
            Vertices[0] = Centre + Utilities.RotateVectorClockwise(new Vector2(-Width / 2f, -Length / 2f), Rotation);
            Vertices[1] = Centre + Utilities.RotateVectorClockwise(new Vector2(Width / 2f, -Length / 2f), Rotation);
            Vertices[2] = Centre + Utilities.RotateVectorClockwise(new Vector2(-Width / 2f, Length / 2f), Rotation);
            Vertices[3] = Centre + Utilities.RotateVectorClockwise(new Vector2(Width / 2f, Length / 2f), Rotation);
        }

        public bool Intersects(RotatedRectangle other)
        {
            if (Utilities.DistanceBetweenVectors(Centre, other.Centre) > (DiagonalLength() + other.DiagonalLength()) / 2f)
            {
                return false;
            }

            //Vector2 pointOfIntersection = PointOfIntersection(other);

            //float toleranceAngle = Atan(Width / 2f * Utilities.DistanceBetweenVectors(pointOfIntersection, AxisOfRotation));
            //float otherToleranceAngle = Atan(other.Width / 2f * Utilities.DistanceBetweenVectors(pointOfIntersection, other.AxisOfRotation));

            //if (Utilities.IsQuantityWithinARangeOfAValue(Utilities.VectorToAngle(pointOfIntersection - AxisOfRotation), Rotation, toleranceAngle)) //ensure that the PoI actually aligns with the rectangle and isnt in ohio
            //{
            //    if (Utilities.IsQuantityWithinARangeOfAValue(Utilities.VectorToAngle(pointOfIntersection - other.AxisOfRotation), other.Rotation, otherToleranceAngle) || Utilities.IsQuantityWithinARangeOfAValue(Utilities.VectorToAngle(pointOfIntersection - other.AxisOfRotation) - MathF.PI * 2f, other.Rotation, otherToleranceAngle))
            //    {
            //        return true;
            //    }
            //}

            //return false;           

            foreach (Vector2 point in other.Vertices)
            {
                if (IsVec2WithinMyRectangle(point))
                {
                    return true;
                }
            }

            foreach (Vector2 point in Vertices)
            {
                if (other.IsVec2WithinMyRectangle(point))
                {
                    return true;
                }
            }
            // if the vertices check fails, check if the point of intersection is within BOTH rectangles. rigorous mental gymnastics which are probably wrong say that doing both checks cover each others errors, idk man, check your notebook.


            Vector2 intersectionPoint = PointOfIntersection(other);

            if (IsVec2WithinMyRectangle(intersectionPoint))
            {
                return other.IsVec2WithinMyRectangle(intersectionPoint);
            }
            else return false;
        }

        public void DrawHitbox()
        {
            TelegraphLine zerotone = new TelegraphLine(Utilities.VectorToAngle(Vertices[1] - Vertices[0]), 0, 0, 5, Utilities.DistanceBetweenVectors(Vertices[0], Vertices[1]), 2, Vertices[0], Color.White, "box", Owner, false);
            TelegraphLine onetothree = new TelegraphLine(Utilities.VectorToAngle(Vertices[3] - Vertices[1]), 0, 0, 5, Utilities.DistanceBetweenVectors(Vertices[3], Vertices[1]), 2, Vertices[1], Color.White, "box", Owner, false);
            TelegraphLine threetotwo = new TelegraphLine(Utilities.VectorToAngle(Vertices[2] - Vertices[3]), 0, 0, 5, Utilities.DistanceBetweenVectors(Vertices[2], Vertices[3]), 2, Vertices[3], Color.White, "box", Owner, false);
            TelegraphLine twotozero = new TelegraphLine(Utilities.VectorToAngle(Vertices[0] - Vertices[2]), 0, 0, 5, Utilities.DistanceBetweenVectors(Vertices[2], Vertices[0]), 2, Vertices[2], Color.White, "box", Owner, false);
        }

        public void DrawVertices(SpriteBatch s, Color colour)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Texture2D texture = Assets["box"];

                s.Draw(texture, Vertices[i], null, colour, 0, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 1);
            }
        }
        public bool IsVec2WithinMyRectangle(Vector2 v2)
        {
            // https://math.stackexchange.com/questions/190111/how-to-check-if-a-point-is-inside-a-rectangle#:~:text=To%20be%20inside%20the%20rectangle,%7C%3D%7CAB%7C. source

            // 0 is A, 1 is B and 2 is D, M is input

            Vector2 AM = v2 - Vertices[0];
            Vector2 AB = Vertices[1] - Vertices[0];
            Vector2 AD = Vertices[2] - Vertices[0];

            float ABselfdot = Vector2.Dot(AB, AB);
            float ADselfdot = Vector2.Dot(AD, AD);

            return (Vector2.Dot(AB, AM) > 0) && (ABselfdot > Vector2.Dot(AB, AM)) && (Vector2.Dot(AD, AM) > 0) && (ADselfdot > Vector2.Dot(AD, AM));
        }

        public Vector2 PointOfIntersection(RotatedRectangle other)
        {
            // rearrangement of simultaneous equation ax + b = cx + d 

            float xOfIntercept = -1f * (CalculateC() - other.CalculateC()) / (CalculateGradient() - other.CalculateGradient());

            float yOfIntercept = CalculateGradient() * xOfIntercept + CalculateC();

            return new Vector2(xOfIntercept, yOfIntercept);
        }
        public float CalculateGradient()
        {
            return -Tan(PI / 2 - Rotation);
        }

        public float CalculateC()
        {
            return Centre.Y - CalculateGradient() * Centre.X;
        }
    }
}
