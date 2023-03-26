using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bullethellwhatever
{
    public class Utilities
    {
        public static Vector2 Normalise(Vector2 vectorToNormalise)
        {
            float distance = MathF.Sqrt(vectorToNormalise.X * vectorToNormalise.X + vectorToNormalise.Y * vectorToNormalise.Y);
            return new Vector2(vectorToNormalise.X / distance, vectorToNormalise.Y / distance);
        }

        public static Vector2 SafeNormalise(Vector2 vectorToNormalise, Vector2 fallback)
        {
            if (vectorToNormalise == Vector2.Zero)
                return fallback;

            else
            {
                float distance = MathF.Sqrt(vectorToNormalise.X * vectorToNormalise.X + vectorToNormalise.Y * vectorToNormalise.Y);
                return new Vector2(vectorToNormalise.X / distance, vectorToNormalise.Y / distance);
            }
        }

        public static void drawTextInDrawMethod(string stringg, Vector2 position, SpriteBatch _spriteBatch, SpriteFont font)
        {
            //_spriteBatch.Begin();
            _spriteBatch.DrawString(font, stringg, position, Color.White);
            //_spriteBatch.End();
        }

        public static float ToDegrees(float angleRadians)
        {
            return angleRadians * (180 / MathF.PI);
        }

        public static float ToRadians(float angleDegrees)
        {
            return angleDegrees * (MathF.PI / 180);
        }

        public static float DistanceBetweenEntities(Entity entity1, Entity entity2)
        {
            return MathF.Sqrt(MathF.Pow((entity1.Position.X - entity2.Position.X), 2) + MathF.Pow((entity1.Position.Y - entity2.Position.Y), 2));
        }

        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            return MathF.Acos(Vector2.Dot(SafeNormalise(v1, Vector2.Zero), SafeNormalise(v2, Vector2.Zero)));
        }

        public static Vector2 RotateVectorCounterClockwise(Vector2 input, float angle) //angle counterclockwise
        {
            //for insight as to how this works, check https://discord.com/channels/770381661098606612/770382926545813515/1089248352073433168

            return  new Vector2(input.X * MathF.Cos(angle) - input.Y * MathF.Sin(angle),
                            input.X * MathF.Sin(angle) + input.Y * MathF.Cos(angle)); //perform rotation
        }

        public static Vector2 RotateVectorClockwise(Vector2 input, float angle) //angle counterclockwise
        {
            //for insight as to how this works, check https://discord.com/channels/770381661098606612/770382926545813515/1089248352073433168

            return new Vector2(input.X * MathF.Cos(2 * MathF.PI - angle) - input.Y * MathF.Sin(2 * MathF.PI - angle),
                            input.X * MathF.Sin(2 * MathF.PI - angle) + input.Y * MathF.Cos(2 * MathF.PI - angle)); //perform rotation
        }
    }

}
