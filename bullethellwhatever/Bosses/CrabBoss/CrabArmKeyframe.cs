using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public struct CrabArmKeyframe
    {
        public Vector2[] Positions;
        public Vector2[] WristPositions;
        public Vector2[][] ArmPartScales;
        public float[][] ArmPartRotations;

        // lmao maybe one day youll make this actually loop through and assign to arrays
        
        // who am i kidding
        public CrabArmKeyframe(CrabBoss owner)
        {
            // i mean i guess this doesnt work if you want to add a third arm for no reason
            Positions = new Vector2[] { owner.Arms[0].Position, owner.Arms[1].Position };

            // i know this is calculable from the other data but i actually just cant be bothered writing the logic
            WristPositions = new Vector2[] { owner.Arms[0].WristPosition(), owner.Arms[1].WristPosition() };

            ArmPartScales = new Vector2[][] { owner.Arms[0].GetPartScales(), owner.Arms[1].GetPartScales() };
            ArmPartRotations = new float[][] { owner.Arms[0].GetPartRotations(), owner.Arms[1].GetPartRotations() };
        }

        public CrabArmKeyframe(Vector2[][] armPartScales, float[][] armPartRotations)
        { 
            ArmPartScales = armPartScales;
            ArmPartRotations = armPartRotations;
        }

        public float GetPartRotation(AppendageType appendageType, int index)
        {
            return ArmPartRotations[index][(int)appendageType];
        }
    }
}
