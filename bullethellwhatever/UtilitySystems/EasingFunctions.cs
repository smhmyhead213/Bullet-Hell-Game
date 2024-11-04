using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.UtilitySystems
{
    public static class EasingFunctions
    {
        // functions use the same name as on easings.net, add easings as you use them

        public static float Linear(float progress)
        {
            return progress;
        }

        public static float EaseOutElastic(float progress)
        {
            float c4 = 2f * PI / 3f;

            if (progress == 0f) return 0f;
            if (progress == 1f) return 1f;
            else return Pow(2, -10 * progress) * Sin((progress * 10 - 0.75f) * c4) + 1;
        }

        public static float EaseInQuint(float progress)
        {
            return Pow(progress, 5);
        }
        public static double EaseInQuint(double progress)
        {
            return Math.Pow(progress, 5);
        }
        public static float EaseOutExpo(float progress)
        {
            return progress == 1 ? 1 : 1 - Pow(2, -10 * progress);
        }

        public static float EaseOutQuad(float progress)
        {
            return 1 - (1 - progress) * (1 - progress);
        }
        public static double EaseOutQuad(double progress)
        {
            return 1 - (1 - progress) * (1 - progress);
        }
        public static float EaseInQuart(float progress)
        {
            return Pow(progress, 4);
        }
        public static float EaseInOutQuart(float progress)
        {
            return progress < 0.5 ? 8 * progress * progress * progress * progress : 1 - Pow(-2 * progress + 2, 4) / 2;
        }
    }
}
