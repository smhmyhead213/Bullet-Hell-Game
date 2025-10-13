using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabArmKeyframe
    {
        public Vector2[][] ArmPartScales;
        public float[][] ArmPartRotations;
        public CrabArmKeyframe(CrabBoss owner)
        {
            // i mean i guess this doesnt work if you want to add a third arm for no reason
            ArmPartScales = new Vector2[][] { owner.Arms[0].GetPartScales(), owner.Arms[1].GetPartScales() };
            ArmPartRotations = new float[][] { owner.Arms[0].GetPartRotations(), owner.Arms[1].GetPartRotations() };
        }

        public CrabArmKeyframe(Vector2[][] armPartScales, float[][] armPartRotations)
        { 
            ArmPartScales = armPartScales;
            ArmPartRotations = armPartRotations;
        }
    }
}
