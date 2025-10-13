using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public static class CrabKeyframes
    {
        public static CrabArmKeyframe Default;

        public static void SetKeyframes()
        {
            Vector2[][] scales = { new Vector2[] { Vector2.One, Vector2.One, Vector2.One, Vector2.One }, new Vector2[] { Vector2.One, Vector2.One, Vector2.One, Vector2.One } };
            float[][] rotations = { new float[] { 0f, 0f, 0f, 0f }, new float[] { 0f, 0f, 0f, 0f } };
            Default = new CrabArmKeyframe(scales, rotations);
        }

        public static void UseKeyframe(this CrabBoss crab, CrabArmKeyframe keyframe)
        {
            for (int i = 0; i < crab.Arms.Length; i++)
            {
                for (int j = 0; j < crab.Arms[i].ArmParts.Length; j++)
                {
                    crab.Arms[i].ArmParts[j].Scale = keyframe.ArmPartScales[i][j];
                    crab.Arms[i].ArmParts[j].RotationToAdd = keyframe.ArmPartRotations[i][j];
                }
            }
        }
    }
}
