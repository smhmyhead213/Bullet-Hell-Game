using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles;
using bullethellwhatever.UtilitySystems;
using FMOD.Studio;
using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            int flickOutTime = 3;
            int sprayTime = 120;
            int windDownTime = 30;

            float holdOutAngle = PI / 12f;
            float finalHoldAngle = PI / 2f;
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

                    //Arm(i).LowerArm.LerpRotation(Arm(i).LowerArm.RotationToAdd, Arm(i).UpperArm.RotationToAdd * 0.5f, EasingFunctions.EaseOutExpo(interpolant));
                    //Arm(i).UpperClaw.RotationToAdd = Arm(i).LowerArm.RotationToAdd;
                    //Arm(i).LowerClaw.RotationToAdd = Arm(i).LowerArm.RotationToAdd;
                }

                int timeHere = chargeUpTime + flickOutTime + waitTime;

                if (AITimer >= timeHere && AITimer < timeHere + sprayTime)
                {
                    int localTime = AITimer - timeHere;
                    float progress = localTime / (float)sprayTime;
                    float interpolant = EasingFunctions.EaseParabolic(progress);
                    float angleVariance = PI / 4 * interpolant;
                    float sizeVariance = 0.3f;
                    float centreAngle = Arm(i).LowerArm.RotationFromV();
                    float projectileSpeed = (interpolant + 0.2f) * 10f;
                    float projectileScale = 1f + Utilities.RandomFloat(-sizeVariance, sizeVariance);

                    Vector2 direction = Utilities.RandomFloat(-angleVariance, angleVariance).ToVector().Rotate(centreAngle);

                    Projectile p = SpawnProjectile(Arm(i).WristPosition(), projectileSpeed * direction, 1f, 1, "box", Vector2.One * projectileScale, Owner, true, false, Color.Red, true, false);
                    p.AddTrail(14);
                    p.Raycast = new BaseClasses.Hitboxes.RaycastData(p.GetVelocity, -1);
                    float acceleration = Utilities.RandomFloat(1.05f, 1.25f);

                    p.SetExtraAI(new Action(() =>
                    {
                        p.Rotation = p.Velocity.ToAngle();
                        p.ExponentialAccelerate(acceleration);
                        p.LightHomeToPlayer(0.01f);
                        
                    }));

                    float armInterpolant = EasingFunctions.EaseOutElastic(progress);
                    Arm(i).UpperArm.LerpTo(-expandedi * finalHoldAngle, armInterpolant);
                    Arm(i).LowerClaw.LerpTo(-expandedi * PI / 2f, armInterpolant);
                    Arm(i).LowerArm.LerpTo(0, armInterpolant);
                }

                if (AITimer >= timeHere + sprayTime && AITimer < timeHere + sprayTime + windDownTime)
                {
                    int localTime = AITimer - (timeHere + sprayTime);

                    Arm(i).LerpArmToRest(localTime / (float)windDownTime);
                }

                if (AITimer == timeHere + sprayTime + windDownTime)
                {
                    End();
                }
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            // try to make prim cones to show danger zone
            base.ExtraDraw(s);

            List<Vector2> conePoints = GenerateConePrimPoints(Arm(0).WristPosition(), Arm(0).LowerArm.RotationFromV(), PI / 2, 400);

            foreach (Vector2 point in conePoints)
            {
                Drawing.DrawBox(point, Color.Red, 1f);
            }
        }

        public List<Vector2> GenerateConePrimPoints(Vector2 startPoint, float rotation, float angleSubtended, float length, int points = 20)
        {
            List<Vector2> output = new List<Vector2>();

            for (int i = 0; i < points; i++) // not including points here might make it not reach max length
            {
                float progress = i / (float)points;
                float currentLength = length * progress;
                // calculate the half-width of the cone at this point
                float halfWidth = currentLength * Tan(angleSubtended / 2);

                output.Add(startPoint + new Vector2(halfWidth, currentLength).Rotate(rotation));
                output.Add(startPoint + new Vector2(-halfWidth, currentLength).Rotate(rotation));
            }

            return output;
        }
        public override BossAttack PickNextAttack()
        {
            return new CrabSpray(CrabOwner);
        }
    }
}
