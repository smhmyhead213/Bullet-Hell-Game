﻿using bullethellwhatever.Bosses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bullethellwhatever.UtilitySystems
{
    public static class EasingFunctions
    {
        // functions use the same name as on easings.net, add easings as you use them

        /// <summary>
        /// Takes any number of easing functions (must be 0 - 1), end values and progress values and joins them into one long easing curve. The progress values are the values of x at which the endValues occur.
        /// Returns a linear easing if the lengths of provided lists do not match.
        /// The first and last elements of progressValues should be 0 and 1.
        /// </summary>
        /// <param name="easingCurves"></param>
        /// <param name="endValues"></param>
        /// <param name="progressValues"></param>
        /// <returns></returns>
        public static Func<float, float> JoinedCurves(Func<float, float>[] easingCurves, float[] endValues, float[] progressValues, float start = 0)
        {
            if (!(easingCurves.Length == endValues.Length && endValues.Length == progressValues.Length))
            {
                return Linear;
            }
            
            List<Func<float, float>> output = new List<Func<float, float>>();

            for (int i = 0; i < easingCurves.Length; i++)
            {
                float startVal = i == 0 ? start : endValues[i - 1];
                float endVal = endValues[i];

                Func<float, float> section = x => startVal + easingCurves[i](x) * endValues[i];
                output.Add(section);
            }

            return x =>
            {
                int index = Utilities.IndexValueIsAtInList(x, progressValues.ToList());
                float localTime = x - progressValues[index];
                return output[index](localTime);
            };
        }

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

        public static float easeInExpo(float progress)
        {
            return progress == 0 ? 0 : Pow(2, 10 * progress - 10);
        }
        public static float EaseOutExpo(float progress)
        {
            return 1 - Pow(2, -10 * progress);
        }
        public static double EaseOutExpo(double progress)
        {
            return 1 - Math.Pow(2, -10 * progress);
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

        public static float EaseOutBack(float progress)
        {
            // idk what these constants are
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return 1 + c3 * Pow(progress - 1, 3) + c1 * Pow(progress - 1, 2);
        }

        public static float EaseInOutBack(float progress)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return progress < 0.5
              ? (Pow(2 * progress, 2) * ((c2 + 1) * 2 * progress - c2)) / 2
              : (Pow(2 * progress - 2, 2) * ((c2 + 1) * (progress * 2 - 2) + c2) + 2) / 2;
        }
        public static float EaseInOutCirc(float progress)
        {
            return progress < 0.5 ? (1 - Sqrt(1 - Pow(2 * progress, 2))) / 2 : (Sqrt(1 - Pow(-2 * progress + 2, 2)) + 1) / 2;
        }

        /// <summary>
        /// Eases from 0 to 1 to 0 parabolically.
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static float EaseParabolic(float progress, float end_progress = 1f)
        {
            // the parabola should be of the form y = kx(x-progress) and then normalised by dividing by its maximum value in terms of progress.
            return -4f * progress * (progress - end_progress) / Pow(end_progress, 2f);
        }
        /// <summary>
        /// "Flips" a number between 0 and 1 - 0.3 becomes 0.7, 1 becomes 0 etc
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static float Flip(float progress)
        {
            return -(progress - 0.5f) + 0.5f;
        }
    }
}
