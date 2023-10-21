using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using System.Runtime.CompilerServices;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class SpinningAtPlayer : CrabBossAttack
    {
        private float LeftArmInitialRotation;
        public SpinningAtPlayer(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            LeftArmInitialRotation = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer;

            int spinUpTime = 60;
            int armMoveTime = 20;
            int leftArmShoveTime = 20;

            // ---- left arm code ----

            if (time == 1)
            {
                Owner.Velocity = Vector2.Zero;

                LeftArmInitialRotation = Leg(0).UpperArm.RotationFromV();

                Vector2 offsetFromCentre = new Vector2(Owner.Texture.Width / 2f * Owner.GetSize().X, -Owner.Texture.Height / 2f * Owner.GetSize().Y);

                Vector2 positionToMoveTo = Owner.Position + offsetFromCentre;

                Leg(0).Velocity = (positionToMoveTo - Leg(0).Position) / armMoveTime;
            }

            if (time == armMoveTime + 1) // when it gets to boss
            {
                Leg(0).Velocity = Vector2.Zero;
            }

            int delayBeforeLeftArmShove = 10;

            if (time == armMoveTime + 1 + delayBeforeLeftArmShove) // pull back
            {
                Leg(0).Velocity = 5f * Vector2.UnitX;
            }

            if (time == armMoveTime + 1 + delayBeforeLeftArmShove + leftArmShoveTime)
            {
                Leg(0).Velocity = -10f * Vector2.UnitX;
            }

            if (time > armMoveTime + 1 + delayBeforeLeftArmShove + leftArmShoveTime)
            {
                Leg(0).Velocity = -10f * Vector2.UnitX;
            }

            if (time < spinUpTime)
            {
                Owner.Rotation = Owner.Rotation + (PI / 720);
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
