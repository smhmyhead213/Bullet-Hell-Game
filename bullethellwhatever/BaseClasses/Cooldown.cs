using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.BaseClasses
{
    public class Cooldown
    {
        public int Timer;
        public int Duration;

        public Cooldown(int duration) 
        {
            Duration = duration;
        }

        public void TickDownCooldown()
        {
            if (Timer > 0)
            {
                Timer--;
            }
        }

    }
}
