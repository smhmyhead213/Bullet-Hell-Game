using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.BaseClasses.Hitboxes
{
    public static class HitboxUtils
    {
        public static Func<float, float> DefaultCentreOffset => (x) => 0;
        public static List<Circle> FillRectWithCircles(Vector2 centre, int width, int height, float rotation)
        {
            return FillRectWithCircles(centre, width, height, rotation, DefaultCentreOffset, 1f);
        }
        public static List<Circle> FillRectWithCircles(Vector2 centre, int width, int height, float rotation, Func<float, float> centreOffsetFunction, float circleScale, float distanceBetweenModifier = 1f)
        {
            // IMPORTANT IMPORTANT IMPORTANT IMPORTANT
            // centre offset function [0,1] -> signed distance from 0, the regular position of the hitbox
            // it was infeasible to make circle scale be a function of progress so its just a scalar, you can edit it afterwards 
            // use the final bool ONLY IF THE SCALE IS CONSTANT - untold horrors await otherwise
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
                    float radius = height >= 2 ? height / 2 * circleScale : 1;
                    float spaceBetweenCentres = radius * distanceBetweenModifier;

                    // for derivation of this formula see random piece of paper i scrawled it onto on 8/10/25 at 13:07 in IOOP2 in the joseph black building middle column fifth row from the back fourth seat from the left
                    int numCircles = (int)Floor((width / radius - 2) / distanceBetweenModifier + 1);
                    // figure out roughly how to divide up space between circles, then fill circles in and use the last one to ensure space is covered fully
                    // use all circles but last to fill as much space as possible

                    int upperLimit = numCircles;
                    float firstCircleOffsetFromEdge = radius;

                    for (int i = 0; i < upperLimit; i++)
                    {
                        float progress = (float)i / upperLimit;
                        float centreOffset = centreOffsetFunction(progress);
                        Vector2 circleCentre = centre - new Vector2(width / 2 - firstCircleOffsetFromEdge - i * spaceBetweenCentres, centreOffset).Rotate(rotation);
                        //BoxDrawer.DrawBox(circleCentre);
                        output.Add(new Circle(circleCentre, radius));
                    }

                    // lmao just stick the final circle on the end and call it a day

                    Vector2 finalCircleCentre = centre + new Vector2(width / 2 - radius, -centreOffsetFunction(1)).Rotate(rotation);
                    output.Add(new Circle(finalCircleCentre, radius));
                    //BoxDrawer.DrawBox(finalCircleCentre);
                }
                else
                {
                    // figure out how many circles to fit in
                    // the use of 1 when the radius is probably supposed to be less might cause an unfair hit, this is why
                    float radius = width >= 2 ? width / 2 * circleScale : 1;
                    float spaceBetweenCentres = radius * distanceBetweenModifier;
                    // for derivation of this formula see random piece of paper i scrawled it onto on 8/10/25 at 13:07 in IOOP2 in the joseph black building middle column fifth row from the back fourth seat from the left
                    int numCircles = (int)Floor((height / radius - 2) / distanceBetweenModifier + 1);
                    // figure out roughly how to divide up space between circles, then fill circles in and use the last one to ensure space is covered fully
                    // use all circles but last to fill as much space as possible

                    int upperLimit = numCircles;
                    for (int i = 0; i < upperLimit; i++)
                    {
                        float progress = (float)i / upperLimit;
                        Vector2 circleCentre = centre - new Vector2(centreOffsetFunction(progress), height / 2 - radius - i * spaceBetweenCentres).Rotate(rotation);
                        output.Add(new Circle(circleCentre, radius * circleScale));
                    }

                    // lmao just stick the final circle on the end and call it a day

                    Vector2 finalCircleCentre = centre + new Vector2(-centreOffsetFunction(1), height / 2 - radius).Rotate(rotation);
                    output.Add(new Circle(finalCircleCentre, radius));
                    //BoxDrawer.DrawBox(finalCircleCentre);
                }

                return output;
            }
        }
    }
}
