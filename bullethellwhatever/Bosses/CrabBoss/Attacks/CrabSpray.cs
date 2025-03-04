using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Projectiles;
using bullethellwhatever.UtilitySystems;
using log4net.Util;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabSpray : CrabBossAttack
    {
        public CrabSpray(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int chargeUpTime = 70;
            int waitTime = 30;
            int flickOutTime = 10;
            int sprayTime = 120;
            int windDownTime = 30;

            float holdOutAngle = PI / 6f;
            float armLength = Arm(0).WristLength();

            for (int i = 0; i < 2; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);
                CrabOwner.FacePlayer();

                if (AITimer < chargeUpTime)
                {                  
                    float interpolant = AITimer / (float)chargeUpTime;
                    float finalLength = armLength * 0.7f;
                    float usedLength = MathHelper.Lerp(armLength, finalLength, interpolant);
                    float usedAngle = MathHelper.Lerp(0f, holdOutAngle, interpolant);

                    Arm(i).TouchPoint(Arm(i).Position + new Vector2(0, usedLength).Rotate(usedAngle * -expandedi).Rotate(Owner.Rotation));
                }

                if (AITimer >= chargeUpTime + waitTime && AITimer < chargeUpTime + flickOutTime + waitTime)
                {
                    int localTime = AITimer - chargeUpTime - waitTime;
                    float interpolant = localTime / (float)flickOutTime;

                    Arm(i).LowerArm.LerpRotation(Arm(i).LowerArm.RotationToAdd, Arm(i).UpperArm.RotationToAdd * 0.5f, interpolant);
                    Arm(i).UpperClaw.RotationToAdd = Arm(i).LowerArm.RotationToAdd;
                    Arm(i).LowerClaw.RotationToAdd = Arm(i).LowerArm.RotationToAdd;
                }

                int timeHere = chargeUpTime + flickOutTime + waitTime;

                if (AITimer >= timeHere && AITimer < timeHere + sprayTime)
                {
                    int localTime = AITimer - timeHere;
                    float interpolant = EasingFunctions.EaseParabolic(localTime / (float)sprayTime);
                    float angleVariance = PI / 3 * interpolant;
                    float centreAngle = Arm(i).LowerArm.RotationFromV();
                    float projectileSpeed = (interpolant + 0.2f) * 30f;
                    float projectileScale = 3.3f;

                    Vector2 direction = Utilities.RandomFloat(-angleVariance, angleVariance).ToVector().Rotate(centreAngle);

                    Projectile p = SpawnProjectile(Arm(i).WristPosition(), projectileSpeed * direction, 1f, 1, "box", Vector2.One * projectileScale, Owner, true, false, Color.Red, true, false);
                    p.AddTrail(14);
                    p.Raycast = new BaseClasses.Hitboxes.RaycastData(p.GetVelocity, -1);
                    p.SetExtraAI(new Action(() =>
                    {
                        p.Rotation = p.Velocity.ToAngle();
                        p.ExponentialAccelerate(1.1f);
                    }));
                }

                if (AITimer >= timeHere + sprayTime && AITimer < timeHere + sprayTime + windDownTime)
                {
                    int localTime = AITimer - (timeHere + sprayTime);

                    Arm(i).LerpArmToRest(localTime / (float)sprayTime);
                }

                if (AITimer == timeHere + sprayTime + windDownTime)
                {
                    End();
                }
            }
        }
    }
}
