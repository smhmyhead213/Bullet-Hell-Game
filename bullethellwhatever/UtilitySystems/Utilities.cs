using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode;
using bullethellwhatever.DrawCode.UI;
using System.Collections.Generic;
using System.Linq;
using bullethellwhatever.AssetManagement;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Diagnostics;
using bullethellwhatever.BaseClasses.Entities;
using bullethellwhatever.NPCs;
using bullethellwhatever.Projectiles;
using bullethellwhatever.BaseClasses.Hitboxes;
using Microsoft.Xna.Framework.Media;
using System.Windows.Forms;
using System.Windows.Markup;
using bullethellwhatever.UtilitySystems;

namespace bullethellwhatever
{
    public static class Utilities
    {
        public static void InitialiseGame()
        {
            EntityManager.activeNPCs.Clear();
            EntityManager.activeProjectiles.Clear();
        }
        /// <summary>
        /// Returns -1 if index is 0 and 1 if index is 1.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int ExpandedIndex(int index)
        {
            return Sign(index - 0.3f); // 0.3 can be anything positive between less than 1
        }
        public static Vector2 ClampWithinScreen(Vector2 vector)
        {
            vector.X = MathHelper.Clamp(vector.X, 0, GameWidth);

            vector.Y = MathHelper.Clamp(vector.Y, 0, GameHeight); // keep target within bounds

            return vector;
        }
        public static float RotationTowards(Vector2 from, Vector2 to)
        {
            return VectorToAngle(to - from);
        }
        public static float InverseLerp(float lower, float upper, float delta)
        {
            float output = upper - lower;
            output = delta / output;
            return MathHelper.Clamp(output, 0f, 1f);
        }
        public static Vector2 Normalise(Vector2 vectorToNormalise)
        {
            float distance = Sqrt(vectorToNormalise.X * vectorToNormalise.X + vectorToNormalise.Y * vectorToNormalise.Y);
            return new Vector2(vectorToNormalise.X / distance, vectorToNormalise.Y / distance);
        }

        //public static Vector2 LerpVectorDirection(Vector2 vec1, float angle, float interpolant)
        //{
        //    return SafeNormalise(Vector2.LerpPrecise(vec1, AngleToVector(angle) * vec1.Length(), interpolant)); 
        //}
        public static Vector2 SafeNormalise(Vector2 vectorToNormalise, Vector2 fallback = default)
        {
            if (vectorToNormalise == Vector2.Zero)
                return fallback;

            else
            {
                float distance = vectorToNormalise.Length();
                return new Vector2(vectorToNormalise.X / distance, vectorToNormalise.Y / distance);
            }
        }

        public static Vector2 VectorToPlayerFrom(Vector2 v)
        {
            return player.Position - v;
        }

        public static Vector2 UnitVectorToPlayerFrom(Vector2 v)
        {
            return SafeNormalise(VectorToPlayerFrom(v));
        }
        public static float AngleToPlayerFrom(Vector2 v)
        {
            return VectorToAngle(VectorToPlayerFrom(v));
        }
        public static void drawTextInDrawMethod(string stringg, Vector2 position, SpriteBatch _spriteBatch, SpriteFont font, Color colour, float scale = 1f)
        {
            //_spriteBatch
            //
            //
            //();
            _spriteBatch.DrawString(font, stringg, position, colour, 0f, Vector2.Zero, scale, SpriteEffects.None, 0); // fix later
            //_spriteBatch.End();
        }
        public static void drawTextOutDrawMethod(string stringg, Vector2 position, SpriteBatch _spriteBatch, SpriteFont font, Color colour, float scale = 1f)
        {

            _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointWrap, transformMatrix: MainCamera.Matrix);
            //
            //
            //();
            drawTextInDrawMethod(stringg, position, _spriteBatch, font, colour, scale);

            _spriteBatch.End();
            //_spriteBatch.End();
        }

        public static float ToDegrees(this float angleRadians)
        {
            return angleRadians * (180 / MathF.PI);
        }

        public static float ToRadians(float angleDegrees)
        {
            return angleDegrees * (MathF.PI / 180);
        }

        public static float DistanceBetweenEntities(Entity entity1, Entity entity2)
        {
            return Sqrt(Pow((entity1.Position.X - entity2.Position.X), 2) + Pow((entity1.Position.Y - entity2.Position.Y), 2));
        }

        public static float DistanceBetweenVectors(Vector2 v1, Vector2 v2)
        {
            return MathF.Sqrt(MathF.Pow(v1.X - v2.X, 2) + MathF.Pow(v1.Y - v2.Y, 2));
        }

        public static float VectorToAngle(Vector2 vector)
        {
            float output = MathF.Atan2(vector.Y, vector.X) + MathF.PI / 2; //ANGLES ARE FROM THE vertical

            if (output > 0)
            {
                return output;
            }
            else return output + MathHelper.TwoPi;
        }

        public static Vector2 AngleToVector(float angle)
        {
            angle = angle - MathF.PI / 2; //adjust so that angle is from vertical

            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }
        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            return Acos(Vector2.Dot(SafeNormalise(v1, Vector2.Zero), SafeNormalise(v2, Vector2.Zero)));
        }

        public static float AngleFromVerticalOfVector(Vector2 vector, Vector2 originOfVector)
        {
            float angle = AngleBetween(-Vector2.UnitY, vector);
            
            //if (originOfVector.X > vector.X)
            //{
            //    angle = PI * 2f - angle;
            //}

            return angle;
        }

        public static bool IsQuantityWithinARangeOfAValue(float quantity, float value, float tolerance)
        {
            if (quantity >= value - tolerance && quantity <= value + tolerance)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates a vector clockwise.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            return RotateVectorClockwise(vector, angle);
        }

        public static Vector2 RotateVectorClockwise(Vector2 input, float angle) //angle counterclockwise
        {
            //for insight as to how this works, check https://discord.com/channels/770381661098606612/770382926545813515/1089248352073433168

            return new Vector2(input.X * MathF.Cos(angle) - input.Y * MathF.Sin(angle),
                            input.X * MathF.Sin(angle) + input.Y * MathF.Cos(angle)); //perform rotation
        }

        public static Vector2 RotateVectorCounterClockwise(Vector2 input, float angle) //angle counterclockwise
        {
            //for insight as to how this works, check https://discord.com/channels/770381661098606612/770382926545813515/1089248352073433168

            return new Vector2(input.X * MathF.Cos(2 * MathF.PI - angle) - input.Y * MathF.Sin(2 * MathF.PI - angle),
                            input.X * MathF.Sin(2 * MathF.PI - angle) + input.Y * MathF.Cos(2 * MathF.PI - angle)); //perform rotation
        }

        /// <summary>
        /// Returns a random float between -absMax and absMax
        /// </summary>
        /// <param name="absMax"></param>
        /// <returns></returns>
        public static float RandomFloat(float absMax)
        {
            return RandomFloat(-absMax, absMax);
        }

        public static float RandomFloat(float min, float max)
        {
            Random rng = new Random();

            double generated = rng.NextDouble() * (max - min) + min;

            return (float)generated;
        }

        public static float DerivativeOfFunctionAtTime(Func<double, double> function, double time)
        {
            double h = 1e-9f;

            return (float)((function(time + h) - function(time)) / h);
        }
        public static bool RandomChance(int oneIn)
        {
            return RandomInt(1, oneIn) == 1;
        }
        public static int RandomSign()
        {
            return RandomInt(-10, 10) > 0 ? 1 : -1;
        }
        public static float RandomAngle()
        {
            return RandomFloat(0, Tau);
        }

        public static float RandomAngle(float variance)
        {
            return RandomFloat(-variance, variance);
        }
        public static float RandomAngle(float min, float max)
        {
            return RandomFloat(min, max);
        }
        public static int RandomInt(int min, int max)
        {
            Random rng = new Random();

            return rng.Next(max - min + 1) + min;
        }
        public static T[] moveArrayElementsUpAndAddToStart<T>(T[] array, T toAdd)
        {
            T[] newArray = new T[array.Length];
            newArray[0] = toAdd;

            for (int i = array.Length - 1; i > 0; i--)
            {
                newArray[i] = array[i - 1];
            }

            return newArray;
        }

        public static T ValueFromDifficulty<T>(T easy, T normal, T hard, T insane)
        {
            switch (GetDifficulty())
            {
                case GameState.Difficulties.Easy: return easy;
                case GameState.Difficulties.Normal: return normal;
                case GameState.Difficulties.Hard: return hard;
                case GameState.Difficulties.Insane: return insane;

                default: return easy;
            }
        }

        public static T[] RandomlyRearrangeArray<T>(T[] array)
        {
            int length = array.Length;
            List<T> list = array.ToList();
            T[] output = new T[length];

            for (int i = 0; i < length; i++)
            {
                int randomIndex = RandomInt(0, list.Count - 1);

                T item = list[randomIndex];
                output[i] = item;
                list.Remove(item);
            }

            return output;
        }
        public static bool ImportantMenusPresent()
        {
            foreach (Menu menu in UIManager.GetListOfActiveMenus())
            { 
                if (menu.Important)
                {
                    return true;
                }
            }

            return false;
        }

        public static Vector2 TextureDimensionsToVector(this Texture2D texture)
        {
            return new Vector2(texture.Width, texture.Height);
        }
        public static float ScreenDiagonalLength()
        {
            return Sqrt(GameWidth * GameWidth + GameHeight * GameHeight);
        }
        public static Vector3 ColorToVec3(Color color)
        {
            return new Vector3(color.R, color.G, color.B);
        }
        public static bool IsVectorWithinScreen(Vector2 v)
        {
            return v.X >= 0 && v.X <= GameWidth && v.Y >= 0 && v.Y <= GameHeight;
        }
        public static Vector2 GameCoordsToVertexCoords(Vector2 coords)
        {
            // move 0,0 to centre of screen
            coords = coords - CentreOfScreen();
            // move negative Y direction to bottom
            coords.Y *= -1f;
            // squish X and Y coordinates to -1 to 1 range
            coords.X /= CentreOfScreen().X;
            coords.Y /= CentreOfScreen().Y;

            return new Vector2(coords.X, coords.Y);
        }

        public static Vector2 TopLeft(this Rectangle rect)
        {
            return new Vector2(rect.Left, rect.Top);
        }

        public static Vector2 BottomRight(this Rectangle rect)
        {
            return new Vector2(rect.Right, rect.Bottom);
        }

        public static Vector2 PlayerSpawnPosition()
        {
            return new Vector2(GameWidth / 2, GameHeight / 10 * 9);
        }

        public static Vector2 PlayerMenuPosition()
        {
            return CentreOfScreen();
        }

        /// <summary>
        /// Converts an angle to an equivalent angle as close to zero as possible
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float ClosestToZero(float angle)
        {
            float newAngle = BringAngleIntoRange(angle);

            if (Abs(newAngle) < Abs(newAngle - Tau))
            {
                return newAngle;
            }
            else return newAngle - Tau;
        }

        /// <summary>
        /// <summary>
        /// Takes an angle and returns an equivalent angle between 0 and 2PI.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float BringAngleIntoRange(float angle)
        {
            while (angle < 0)
            {
                angle += 2 * PI;
            }

            while (angle > 2 * PI)
            {
                angle -= 2 * PI;
            }

            return angle;
        }

        /// Returns the smallest angle clockwise between two angles as one clockwise from the vertical.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float SmallestAngleTo(float start, float end)
        {
            start = BringAngleIntoRange(start);
            end = BringAngleIntoRange(end);

            // figure out what sign the angle between has
            int sign = Sign(end - start);
            float result = BringAngleIntoRange(Abs(end - start));
            
            if (result > PI)
            {
                result -= 2 * PI;
            }

            return result * sign;
        }

        /// <summary>
        /// Lerps between start and end, returning a vector the same length as start
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="interpolant"></param>
        /// <returns></returns>
        public static Vector2 ConserveLengthLerp(Vector2 start, Vector2 end, float interpolant, float length = 0f)
        {
            if (length == 0f)
            {
                length = start.Length();
            }

            end = start.Length() * SafeNormalise(end);

            return start.Length() * SafeNormalise(Vector2.Lerp(start, end, interpolant));
        }

        public static Vector2 RandomDirection()
        {
            return AngleToVector(RandomAngle());
        }

        public static bool RandomBool()
        {
            return RandomInt(1, 100) % 2 == 0;
        }
        public static Color RandomColour()
        {
            return new Color(RandomFloat(0f, 1f), RandomFloat(0f, 1f), RandomFloat(0f, 1f));
        }

        public static List<Circle> FillRectWithCircles(Vector2 centre, int width, int height, float rotation)
        {
            List<Circle> output = new List<Circle>();

            if (width == height)
            {
                output.Add(new Circle(centre, width / 2f));
                return output; // use one circle for things of equal width and height
            }

            else
            {
                if (width > height)
                {
                    // figure out how many circles to fit in
                    // the use of 1 when the radius is probably supposed to be less might cause an unfair hit, this is why
                    int radius = height >= 2 ? height / 2 : 1;
                    int numCircles = width / radius - 1; // add a circle every radius

                    // try to figure out how much would be left on the end, and try to split it evenly between both sides

                    int totalRadiusCovered = radius * (numCircles + 1);
                    int leftOnEnd = (width - totalRadiusCovered) / 2;

                    Vector2 startPos = centre - Utilities.RotateVectorClockwise(new Vector2(width / 2 - radius - leftOnEnd, 0), rotation);

                    output.Add(new Circle(startPos, radius));

                    for (int i = 1; i < numCircles; i++)
                    {
                        output.Add(new Circle(startPos + Utilities.RotateVectorClockwise(new Vector2(i * radius, 0), rotation), radius));
                    }
                }
                else
                {
                    // figure out how many circles to fit in
                    int radius = width >= 2 ? width / 2 : 1;
                    int numCircles = height / radius - 1; // add a circle every radius
                    float adjustedRotation = rotation - PI / 2f;

                    // try to figure out how much would be left on the end, and try to split it evenly between both sides

                    int totalRadiusCovered = radius * (numCircles + 1);
                    int leftOnEnd = (height - totalRadiusCovered) / 2;

                    Vector2 startPos = centre - Utilities.RotateVectorClockwise(new Vector2(height / 2 - radius - leftOnEnd, 0), adjustedRotation);

                    output.Add(new Circle(startPos, radius));

                    for (int i = 1; i < numCircles; i++)
                    {
                        output.Add(new Circle(startPos + Utilities.RotateVectorClockwise(new Vector2(i * radius, 0), adjustedRotation), radius));
                    }
                }

                return output;
            }
        }

        public static float WithinTau(this float angle)
        {
            return BringAngleIntoRange(angle);
        }

        public static float Distance(this Vector2 v1, Vector2 v2)
        {
            return DistanceBetweenVectors(v1, v2);
        }

        public static float AngleToPlayer(this Vector2 position)
        {
            return position.ToPlayer().ToAngle();
        }
        public static Vector2 ToPlayer(this Vector2 position)
        {
            return player.Position - position;
        }

        public static Vector2 DirectionToPlayer(this Vector2 position)
        {
            return SafeNormalise(position.ToPlayer());
        }
        public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
        {
            return SafeNormalise(to - from);
        }

        public static float SinWithPeriod(float x, float period)
        {
            return Sin(Tau / period * x);
        }
        public static float CosWithPeriod(float x, float period)
        {
            return Cos(Tau / period * x);
        }

        public static float ToAngle(this Vector2 vec)
        {
            return VectorToAngle(vec);
        }
        public static Vector2 ToVector(this float angle)
        {
            return AngleToVector(angle);
        }

        public static Vector2 LerpLength(this Vector2 vector, float startLength, float endLength, float interpolant)
        {
            return SafeNormalise(vector) * MathHelper.Lerp(startLength, endLength, interpolant);
        }
        public static Vector2 SetLength(this Vector2 vector, float length)
        {
            return SafeNormalise(vector) * length;
        }

        public static float Sin01(float theta)
        {
            // f(0) = 0.5
            return (Sin(theta) + 1f) / 2f;
        }
        public static float Cos01(float theta)
        {
            // f(0) = 1
            return (Cos(theta) + 1f) / 2f;
        }
        /// <summary>
        /// Returns the sign of the rotation required to rotate from startAngle to endAngle.
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        public static int RotationDirection(float startAngle, float endAngle)
        {
            return Sign(SmallestAngleTo(startAngle, endAngle));
        }
        public static float LerpRotation(float startAngle, float endAngle, float interpolant)
        {
            endAngle = ClosestToZero(endAngle);
            float endAngleMinusTwoPi = endAngle - Tau;

            float angleToUse;

            // determine which direction to turn towards to minimise turn angle
            if (Abs(startAngle - endAngle) < Abs(startAngle - endAngleMinusTwoPi))
            {
                angleToUse = endAngle;
            }
            else
            {
                angleToUse = endAngleMinusTwoPi;
            }

            return MathHelper.Lerp(startAngle, angleToUse, interpolant);
        }
        public static void Add<T>(this List<T> addTo, List<T> toAdd)
        {
            foreach (T item in toAdd)
            {
                addTo.Add(item);
            }
        }

        /// <summary>
        /// Assumes list is sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int IndexValueIsAtInList(float value, List<float> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (value > list[i] && value < list[i + 1])
                { 
                    return i;
                }
            }

            return list.Count - 1;
        }

        public static bool CollidingWith(this List<Circle> me, List<Circle> other)
        {
            foreach (Circle mine in me)
            {
                foreach (Circle notmine in other)
                {
                    if (mine.Intersects(notmine))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static Vector2 CentreOfScreen() =>  new Vector2(GameWidth / 2, GameHeight / 2);
        public static Vector2 CentreWithCamera() => CentreOfScreen() + MainCamera.VisibleArea.TopLeft();
        public static void ApplyRandomNoise(this Effect shader)
        {
            shader.Parameters["randomNoiseMap"]?.SetValue(AssetRegistry.GetTexture2D("RandomNoise"));
        }
    }

}
