using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode;
using bullethellwhatever.DrawCode.UI;
using System.Collections.Generic;
using System.Linq;

namespace bullethellwhatever
{
    public static class Utilities
    {
        public static void InitialiseGame()
        {
            EntityManager.activeNPCs.Clear();
            EntityManager.activeProjectiles.Clear();
            EntityManager.activeFriendlyProjectiles.Clear();
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

        public static float RandomFloat(float min, float max)
        {
            Random rng = new Random();

            double generated = rng.NextDouble() * (max - min) + min;

            return (float)generated;
        }

        public static bool RandomChance(int oneIn)
        {
            return RandomInt(1, oneIn) == 1;
        }
        public static float RandomAngle()
        {
            return RandomFloat(0, Tau);
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
        public static Vector2 CentreOfScreen() =>  new Vector2(GameWidth / 2, GameHeight / 2);
    }

}
