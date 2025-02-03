using bullethellwhatever.Projectiles.Base;
using System;
using Microsoft.Xna.Framework;
using bullethellwhatever.MainFiles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;

using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses.Entities;
using bullethellwhatever.DrawCode;

namespace bullethellwhatever.BaseClasses.Hitboxes
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

        public RotatedRectangle()
        {

        }

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

        /// <summary>
        /// Backwards raycast by default.
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public RotatedRectangle GenerateRaycast(Vector2 velocity, int direction = -1)
        {
            float velocityLength = velocity.Length();
            // hitbox checks apply after update, so we raycast backwards to check the space the projectile just crossed
            return new RotatedRectangle(Utilities.VectorToAngle(velocity), Width, velocityLength, Centre + direction * 0.5f * velocity, Owner);
        }

        public RotatedRectangle GenerateRaycast(RaycastData raycastData)
        {
            return GenerateRaycast(raycastData.DescribingVector, raycastData.Direction);
        }

        public Collision IntersectsWithRaycast(RotatedRectangle other, Vector2 velocityThis, int direction = -1)
        {
            RotatedRectangle ahead = GenerateRaycast(velocityThis, direction);

            Collision aheadColl = other.Intersects(ahead);

            if (aheadColl.Collided)
            {
                return aheadColl;
            }

            return Intersects(other);
        }

        public Collision Intersects(RotatedRectangle other)
        {
            if (Utilities.DistanceBetweenVectors(Centre, other.Centre) > (DiagonalLength() + other.DiagonalLength()) / 2f)
            {
                return new Collision(Vector2.Zero, false);
            }

            foreach (Vector2 point in other.Vertices)
            {
                if (IsVec2WithinMyRectangle(point))
                {
                    return new Collision(point, true);
                }
            }

            foreach (Vector2 point in Vertices)
            {
                if (other.IsVec2WithinMyRectangle(point))
                {
                    return new Collision(point, true);
                }
            }

            // if the vertices check fails, check if the point of intersection is within BOTH rectangles. rigorous mental gymnastics which are probably wrong say that doing both checks cover each others errors, idk man, check your notebook.

            Vector2 intersectionPoint = PointOfIntersection(other);

            if (IsVec2WithinMyRectangle(intersectionPoint))
            {
                return new Collision(intersectionPoint, other.IsVec2WithinMyRectangle(intersectionPoint)); // this second check is so that we can ensure that the PoI is within both
            }

            intersectionPoint = other.PointOfIntersection(this); // double check

            if (other.IsVec2WithinMyRectangle(intersectionPoint))
            {
                return new Collision(intersectionPoint, IsVec2WithinMyRectangle(intersectionPoint));
            }

            else return new Collision(Vector2.Zero, false);
        }

        //public void DrawHitbox()
        //{
        //    SpawnTelegraphLine(Utilities.VectorToAngle(Vertices[1] - Vertices[0]), 0, 5, Utilities.DistanceBetweenVectors(Vertices[0], Vertices[1]), 2, Vertices[0], Color.White, "box", Owner, false);
        //    SpawnTelegraphLine(Utilities.VectorToAngle(Vertices[3] - Vertices[1]), 0, 5, Utilities.DistanceBetweenVectors(Vertices[3], Vertices[1]), 2, Vertices[1], Color.White, "box", Owner, false);
        //    SpawnTelegraphLine(Utilities.VectorToAngle(Vertices[2] - Vertices[3]), 0, 5, Utilities.DistanceBetweenVectors(Vertices[2], Vertices[3]), 2, Vertices[3], Color.White, "box", Owner, false);
        //    SpawnTelegraphLine(Utilities.VectorToAngle(Vertices[0] - Vertices[2]), 0, 5, Utilities.DistanceBetweenVectors(Vertices[2], Vertices[0]), 2, Vertices[2], Color.White, "box", Owner, false);
        //}

        public void DrawVertices(SpriteBatch s, Color colour)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Texture2D texture = AssetRegistry.GetTexture2D("box");

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

            return Vector2.Dot(AB, AM) > 0 && ABselfdot > Vector2.Dot(AB, AM) && Vector2.Dot(AD, AM) > 0 && ADselfdot > Vector2.Dot(AD, AM);
        }

        public Vector2 PointOfIntersection(RotatedRectangle other)
        {
            // rearrangement of simultaneous equation ax + b = cx + d

            // if theres an issue, make sure you have brackets in the right places if you change this again

            float xOfIntercept = (other.CalculateC() - CalculateC()) / (CalculateGradient() - other.CalculateGradient());

            float yOfIntercept = CalculateGradient() * xOfIntercept + CalculateC();

            Vector2 poi = new Vector2(xOfIntercept, yOfIntercept);

            return poi;
        }
        public float CalculateGradient()
        {
            return -Tan(PI / 2 - Rotation);
        }

        public float CalculateC()
        {
            //return CalculateGradient() * Centre.X - Centre.Y;

            return Centre.Y - Centre.X * CalculateGradient();
        }

        public void Draw(float width, Vector2 velocity = default, int raycastDir = 0)
        {
            int[] order = [0, 1, 3, 2];

            for (int i = 0; i < order.Length; i++)
            {
                int next = i < order.Length - 1 ? order[i + 1] : order[0];

                DrawUtils.DrawLine(Vertices[order[i]], Vertices[next], width, Color.Gray);
            }
        }
    }

    public class Collision
    {
        public Vector2 CollisionPoint;
        public bool Collided;

        public Collision(Vector2 point, bool collided) // vector2.zero is a flag for no collision as it is cosmically unlikely that a collision will happen there
        {
            CollisionPoint = point;
            Collided = collided;
        }
    }
}
