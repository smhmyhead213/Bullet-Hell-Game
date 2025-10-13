using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public partial class CrabBoss
    {
        // attack timings and stuff need to shared across both the boss and arm behaviours, so stick them all in here (or maybe not? could make them static on each attack

        // angles to open claws that look good
        public static readonly float UpperClawOpenAngle = PI / 5f;
        public static readonly float LowerClawOpenAngle = PI / 5f;

        public static readonly float AngleToCloseClaw = PI / 12;
    }
}
