using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using bullethellwhatever.Projectiles;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class ProjectileFan : EyeBossAttack
    {
        public float FanStartAngle;
        public ProjectileFan(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
            FanStartAngle = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int shootTime = 100;
            int shootDuration = 450;
            int shootSlowDownTime = 60;
            float fanAngleCoverage = PI * 58.8f;
            float anglePerFrame = fanAngleCoverage / shootDuration;
            int distanceFromEyeCentre = 30;

            int time = AITimer % (shootTime + shootDuration + shootSlowDownTime);

            if (time < shootTime)
            {
                Pupil.Size = Pupil.InitialSize;

                Pupil.LookAtPlayer(distanceFromEyeCentre);
            }

            if (time == shootTime)
            {
                FanStartAngle = Utilities.AngleToPlayerFrom(Pupil.Position) - (fanAngleCoverage / 2); // calculate the angle to shoot the first projectile from
            }

            if (time > shootTime && time < shootTime + shootDuration)
            {
                int localTime = time - shootTime;

                float pupilRotation = FanStartAngle + localTime * anglePerFrame; // add an angle based on the passed time

                Pupil.GoTo(distanceFromEyeCentre, pupilRotation);
                
                Projectile p = SpawnProjectile(Pupil.Position, 2f * Utilities.AngleToVector(pupilRotation), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);

                p.AddTrail(11);

                p.Rotation = pupilRotation;

                p.SetExtraAI(new Action(() =>
                {
                    p.HomeAtTarget(player.Position, 0f);
                    p.ExponentialAccelerate(1.03f);
                }));
            }

            if (time > shootTime + shootDuration && time < shootTime + shootDuration + shootSlowDownTime)
            {
                int localTime = time - (shootTime + shootDuration);

                Pupil.RotationWithinEye = MathHelper.Lerp(Pupil.RotationWithinEye, Utilities.AngleToPlayerFrom(Pupil.Position), (float)localTime / shootSlowDownTime);
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
