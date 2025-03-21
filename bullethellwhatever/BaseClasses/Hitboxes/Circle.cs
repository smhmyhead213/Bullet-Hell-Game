﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.BaseClasses.Hitboxes
{
    public class Circle
    {
        public Vector2 Centre;
        public float Radius;

        public Circle(Vector2 centre, float radius)
        {
            Centre = centre;
            Radius = radius;
        }

        public bool PointInCircle(Vector2 point)
        {
            return (Centre - point).LengthSquared() < Pow(Radius, 2f);
        }

        public bool Intersects(Circle other)
        {
            return (Centre - other.Centre).LengthSquared() < Pow(Radius + other.Radius, 2f);
        }
        public static Circle operator *(Circle a, float b) => new Circle(a.Centre, b * a.Radius);
        public static Circle operator /(Circle a, float b) => new Circle(a.Centre, a.Radius / b);

    }
}
