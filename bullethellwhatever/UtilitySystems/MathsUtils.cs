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
        public static Func<float, Vector2> PathBetweenPoints(Vector2 targetPosition, Vector2 initialPosition, float initialAngle, Func<float, float> parallelEasing, Func<float, float> lateralEasing)
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
    }
}
