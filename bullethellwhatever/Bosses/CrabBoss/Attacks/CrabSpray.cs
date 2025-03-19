using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.AssetManagement;
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
        public const int chargeUpTime = 70;
        public const int waitTime = 30;
        public const int sprayTime = 120;
        public const int windDownTime = 30;
        public float maxSprayAngle => PI / 4f;
        public int projectileReleaseTime => chargeUpTime + waitTime;
        public CrabSpray(CrabBoss owner) : base(owner)
        {

        }

        public override bool SelectionCondition()
        {
            return Owner.DistanceFromPlayer() < 900f;
        }

        public override void Execute(int AITimer)
        {
            float holdOutAngle = PI / 12f;
            float finalHoldAngle = PI / 3f;
            float armLength = Arm(0).WristLength();
            float moveTowardsPlayerSpeed = 2.7f;

            int slowDownTime = 10;

            if (AITimer < slowDownTime)
            {
                if (Owner.Velocity.Length() < 0.01f)
                {
                    Owner.Velocity = Vector2.Zero;
                }

                float interpolant = 1f - EasingFunctions.EaseOutExpo(AITimer / (float)slowDownTime);

                Vector2 velocity = Owner.Velocity;
                Owner.Velocity = velocity.SetLength(velocity.Length() * interpolant);
                Assert(!float.IsNaN(Owner.Velocity.Length()));
            }

            for (int i = 0; i < 2; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);
                float initialScale = 1f;
                float finalScale = 1.3f;

                CrabOwner.FacePlayer();

                if (AITimer < chargeUpTime)
                {                  
                    float interpolant = AITimer / (float)chargeUpTime;

                    Arm(i).SetScale(MathHelper.Lerp(initialScale, finalScale, interpolant));

                    float finalLength = armLength * 0.88f;
                    float usedLength = MathHelper.Lerp(armLength, finalLength, interpolant);
                    float usedAngle = MathHelper.Lerp(0f, holdOutAngle, interpolant);

                    Arm(i).TouchPoint(Arm(i).Position + new Vector2(0, usedLength).Rotate(usedAngle * -expandedi).Rotate(Owner.Rotation));
                }

                int timeHere = chargeUpTime + waitTime;

                if (AITimer >= timeHere && AITimer < timeHere + sprayTime)
                {
                    // idea: have the boss move slowly towards the player here to force them to move back instead of doing nothing

                    Owner.Velocity = moveTowardsPlayerSpeed * Owner.Position.DirectionToPlayer();

                    int localTime = AITimer - timeHere;
                    float progress = localTime / (float)sprayTime;

                    float armInterpolant = EasingFunctions.Linear(progress);
                    Arm(i).UpperArm.LerpTo(-expandedi * finalHoldAngle, armInterpolant);
                    //Arm(i).LowerArm.LerpTo(-expandedi * - PI / 2, armInterpolant);
                    Arm(i).LowerClaw.LerpTo(-expandedi * PI / 2f, armInterpolant);
                    //Arm(i).LowerArm.LerpTo(0, armInterpolant);

                    float interpolant = EasingFunctions.EaseParabolic(progress);
                    float angleVariance = maxSprayAngle * interpolant;
                    float sizeVariance = 0.3f;
                    float centreAngle = Arm(i).LowerArm.RotationFromV();
                    float projectileSpeed = (interpolant + 0.2f) * 10f;
                    float projectileScale = 1f + Utilities.RandomFloat(-sizeVariance, sizeVariance);

                    Vector2 direction = Utilities.RandomFloat(-angleVariance, angleVariance).ToVector().Rotate(centreAngle);

                    Projectile p = SpawnProjectile(Arm(i).WristPosition(), projectileSpeed * direction, 1f, 1, "box", Vector2.One * projectileScale, Owner, true, false, Color.Red, true, false);
                    p.AddTrail(14);
                    p.Raycast = new BaseClasses.Hitboxes.RaycastData(p.GetVelocity, -1);

                    float acceleration = Utilities.RandomFloat(1.05f, 1.25f);
                    float homingStrength = 0f;

                    p.SetExtraAI(new Action(() =>
                    {
                        p.Rotation = p.Velocity.ToAngle();
                        p.ExponentialAccelerate(acceleration);
                        p.LightHomeToPlayer(homingStrength);

                    }));
                }

                if (AITimer >= timeHere + sprayTime && AITimer < timeHere + sprayTime + windDownTime)
                {
                    int localTime = AITimer - (timeHere + sprayTime);
                    float progress = localTime / (float)windDownTime;
                    Arm(i).SetScale(MathHelper.Lerp(finalScale, initialScale, progress));
                    Arm(i).LerpArmToRest(localTime / (float)windDownTime);
                    Owner.Velocity = Utilities.SafeNormalise(Owner.Velocity) * MathHelper.Lerp(moveTowardsPlayerSpeed, 0f, progress);
                }

                if (AITimer == timeHere + sprayTime + windDownTime)
                {
                    End();
                }
            }
        }

        public override void ExtraDraw(SpriteBatch s, int AITimer)
        {
            // try to make prim cones to show danger zone
            base.ExtraDraw(s, AITimer);

            //foreach (Vector2 point in GenerateConePrimPoints(Arm(0).WristPosition(), Arm(0).LowerArm.RotationFromV(), PI / 2, 400))
            //{
            //    Drawing.DrawBox(point, Color.Red, 1f);
            //}

            float length = 800;

            Color colour = Color.Red;

            Shader coneShader = new("CrabConeShader", Color.Red);

            int lingerTime = 20;
            int endTime = projectileReleaseTime + lingerTime;

            if (AITimer < endTime)
            {
                float opacity = 1f;
                int fadeInTime = 15;
                int fadeOutTime = 8;

                if (AITimer < fadeInTime)
                {
                    opacity = AITimer / (float)fadeInTime;
                }

                if (AITimer > endTime - fadeOutTime)
                {
                    opacity = EasingFunctions.EaseOutExpo((endTime - AITimer) / (float)fadeInTime);
                }

                coneShader.SetParameter("colour", colour);
                coneShader.SetParameter("opacity", 1f);
                coneShader.SetNoiseMap("DangerTexture", 1f);
                
                float currentAngle = opacity * maxSprayAngle;
                float maxWidth = 2f * length * Tan(currentAngle / 2);

                coneShader.SetParameter("maxWidth", maxWidth);

                int points = 20;

                for (int i = 0; i < 2; i++)
                {
                    DrawCone(Arm(i).WristPosition(), Arm(i).LowerArm.RotationFromV(), currentAngle, length, colour * opacity, coneShader, points);
                }
            }
        }

        public void DrawCone(Vector2 startPoint, float rotation, float angleSubtended, float length, Color colour, Shader shader, int points = 20)
        {
            List<Vector2> inpPoints = GenerateConePrimPoints(startPoint, rotation, angleSubtended, length, points);
            int vertexCount = inpPoints.Count;

            if (vertexCount == 0)
            {
                // explodes pancake with mind
                return;
            }

            for (int i = 0; i < vertexCount; i += 2)
            {
                int startingIndex = i;
                float progress = i / (float)points; // this might be wrong
                Color colToUse = colour * (1f - progress);

                PrimitiveManager.AddPoint(startingIndex, inpPoints[i], colToUse, new Vector2(0f, progress));
                PrimitiveManager.AddPoint(startingIndex + 1, inpPoints[i + 1], colToUse, new Vector2(1f, progress));
            }

            int numberOfTriangles = vertexCount - 2;

            int indexCount = numberOfTriangles * 3;

            for (int i = 0; i < numberOfTriangles; i++)
            {
                int startingIndex = i * 3;
                PrimitiveManager.MainIndices[startingIndex] = (short)i;
                PrimitiveManager.MainIndices[startingIndex + 1] = (short)(i + 1);
                PrimitiveManager.MainIndices[startingIndex + 2] = (short)(i + 2);
            }

            PrimitiveSet primSet = new PrimitiveSet(vertexCount, indexCount, shader.Effect);

            primSet.Draw();
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

                output.Add(startPoint + new Vector2(halfWidth, currentLength).Rotate(rotation + PI));
                output.Add(startPoint + new Vector2(-halfWidth, currentLength).Rotate(rotation + PI));
            }

            return output;
        }
    }
}
