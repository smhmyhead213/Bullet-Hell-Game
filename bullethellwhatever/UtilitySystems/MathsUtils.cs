using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.UtilitySystems
{
    public static class MathsUtils
    {
        public static Func<float, Vector2> PathBetweenPoints(Vector2 initialPosition, Vector2 targetPosition, float initialAngle, Func<float, float> parallelEasing, Func<float, float> lateralEasing)
        {
            Vector2 towardsPlayerFromWrist = targetPosition - initialPosition;
            float toPlayerDistance = towardsPlayerFromWrist.Length();
            towardsPlayerFromWrist = Utilities.SafeNormalise(towardsPlayerFromWrist);

            Vector2 towardsPlayerParallel = towardsPlayerFromWrist.Rotate(initialAngle);
            Vector2 towardsPlayerLateral = towardsPlayerParallel.Rotate(-PI / 2);

            towardsPlayerParallel *= toPlayerDistance * Cos(initialAngle);
            towardsPlayerLateral *= toPlayerDistance * Sin(initialAngle);

            Func<float, Vector2> path = (x) => initialPosition + parallelEasing(x) * towardsPlayerParallel + lateralEasing(x) * towardsPlayerLateral;

            return path;
        }

        /// <summary>
        /// firstPartLength refers to the part of the system that connects to startPosition. bendSign serves to differentiate between the two posssible configurations.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="firstPartLength"></param>
        /// <param name="secondPartLength"></param>
        /// <returns></returns>
        public static float[] SolveTwoPartIK(Vector2 startPosition, Vector2 targetPosition, float firstPartLength, float secondPartLength, int bendSign = 1)
        {
            float lengthOfLeg = firstPartLength + secondPartLength;

            // decide on a target. if the target it out of reach, choose a new target in the same direction that's reachable

            // an inaccuracy arises from the subtraction taking place here
            Vector2 direction = Utilities.SafeNormalise(targetPosition - startPosition);

            if (Utilities.DistanceBetweenVectors(startPosition, targetPosition) > lengthOfLeg)
            {
                // get the direction vector               
                targetPosition = startPosition + lengthOfLeg * direction;
            }

            // the distance between the start of the arm and the target
            float distance = Utilities.DistanceBetweenVectors(startPosition, targetPosition);

            if (distance > lengthOfLeg)
            {
                distance = lengthOfLeg;
            }

            // check that triangle inequality is not violated

            if (firstPartLength - secondPartLength > distance)
            {
                distance = firstPartLength - secondPartLength;
            }

            // cosine rule
            float firstPartSquared = Pow(firstPartLength, 2);
            float secondPartSquared = Pow(secondPartLength, 2);
            float distanceSquared = Pow(distance, 2);

            float withinAcos = (firstPartSquared + distanceSquared - secondPartSquared) / (2 * firstPartLength * distance);
            float upperArmAngle = withinAcos < 1.0001f && withinAcos >= 1f ? 0 : Acos(withinAcos); // prevent NaN for exactly 1 and imprecisions

            Assert(!float.IsNaN(upperArmAngle));

            Vector2 elbowPos = startPosition + Utilities.RotateVectorClockwise(direction * firstPartLength, bendSign * upperArmAngle);

            float[] output = new float[2];

            output[0] = Utilities.VectorToAngle(elbowPos - startPosition);

            float secondPartRotation = Utilities.VectorToAngle(targetPosition - elbowPos);

            // the arm seems to be touching the correct point before executing this line, but afterwards seems to be slightly off
            output[1] = secondPartRotation;

            return output;
        }
    }
}
