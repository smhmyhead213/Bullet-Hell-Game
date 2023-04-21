using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.UtilitySystems
{
    public class ScreenShakeObject
    {
        public Vector2 Magnitude;
        public Vector2 MaxMagnitude;
        public int Duration;
        public int Timer;

        public ScreenShakeObject(int magnitude, int duration)
        {
            Magnitude = new Vector2(magnitude, magnitude);
            MaxMagnitude = Magnitude;
            Duration = duration;
            Timer = duration;
        }
        public void TickDownDuration()
        {
            if (Timer > 0)
                Timer--;

            if (Timer == 0)
            {
                Magnitude = Vector2.Zero;
                MaxMagnitude = Vector2.Zero;
            }
        }

    }
}
