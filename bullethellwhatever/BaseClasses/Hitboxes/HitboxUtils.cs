using System;
using System.Collections.Generic;
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
        public static Func<float, float> DefaultCircleScale => (x) => 1;
        public static List<Circle> FillRectWithCircles(Vector2 centre, int width, int height, float rotation)
        {
            return FillRectWithCircles(centre, width, height, rotation, DefaultCentreOffset, DefaultCircleScale);
        }
        public static List<Circle> FillRectWithCircles(Vector2 centre, int width, int height, float rotation, Func<float, float> centreOffsetFunction, Func<float, float> circleScaleFunction, float distanceBetweenModifier = 1f)
        {
            // IMPORTANT IMPORTANT IMPORTANT IMPORTANT
            // centre offset function [0,1] -> signed distance from 0, the regular position of the hitbox
            // circle scale function [0,1] -> scale factor for circle at progress x. does not affect positioning of circles whatsoever
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
                    float spaceBetweenCentres = radius * distanceBetweenModifier;
                    int numCircles = (int)(width / spaceBetweenCentres) + 1;
                    // figure out roughly how to divide up space between circles, then fill circles in and use the last one to ensure space is covered fully
                    // use all circles but last to fill as much space as possible

                    int upperLimit = numCircles - 2;
                    for (int i = 0; i < upperLimit; i++)
                    {
                        float progress = (float)i / upperLimit;
                        Vector2 circleCentre = centre - new Vector2(width / 2 - radius - i * spaceBetweenCentres, centreOffsetFunction(progress)).Rotate(rotation);
                        output.Add(new Circle(circleCentre, radius * circleScaleFunction(progress)));
                    }

                    // lmao just stick the final circle on the end and call it a day

                    Vector2 finalCircleCentre = centre + new Vector2(width / 2 - radius, 0).Rotate(rotation);
                    output.Add(new Circle(finalCircleCentre, radius * circleScaleFunction(1)));
                    //BoxDrawer.DrawBox(finalCircleCentre);
                }
                else
                {
                    // figure out how many circles to fit in
                    // the use of 1 when the radius is probably supposed to be less might cause an unfair hit, this is why
                    int radius = width >= 2 ? width / 2 : 1;
                    float spaceBetweenCentres = radius * distanceBetweenModifier;
                    int numCircles = (int)(height / spaceBetweenCentres) + 1;
                    // figure out roughly how to divide up space between circles, then fill circles in and use the last one to ensure space is covered fully
                    // use all circles but last to fill as much space as possible

                    int upperLimit = numCircles - 2;
                    for (int i = 0; i < upperLimit; i++)
                    {
                        float progress = (float)i / upperLimit;
                        Vector2 circleCentre = centre - new Vector2(centreOffsetFunction(progress), height / 2 - radius - i * spaceBetweenCentres).Rotate(rotation);
                        output.Add(new Circle(circleCentre, radius * circleScaleFunction(progress)));
                    }

                    // lmao just stick the final circle on the end and call it a day

                    Vector2 finalCircleCentre = centre + new Vector2(0, height / 2 - radius).Rotate(rotation);
                    output.Add(new Circle(finalCircleCentre, radius * circleScaleFunction(1)));
                    BoxDrawer.DrawBox(finalCircleCentre);
                }

                return output;
            }
        }
    }
}
