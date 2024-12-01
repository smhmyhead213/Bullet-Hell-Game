﻿using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Abilities.Weapons
{
    public class MachineWeapon : Weapon
    {
        public int FullChargeTime;
        public int ChargeTimer;
        public int BurstDuration;
        public int BurstTimeLeft;
        public MachineWeapon(Player player) : base(player)
        {
            
        }
        public override void WeaponInitialise()
        {
            PrimaryFireCoolDownDuration = 3;
            FullChargeTime = 45;
            BurstDuration = 20;
            BurstTimeLeft = 0;
            ChargeTimer = 0;
        }
        public override bool CanSwitchWeapon()
        {
            return true;
        }

        public Vector2[] CreateLightningPoints()
        {
            // pick an endpoint for the lightning trail at a varying distance from the player
            float minDistance = 200f;
            float maxDistance = 300f;
            int lightningPoints = 8;
            float lightningAngleVariance = PI / 4f;

            float endpointDistFromPlayer = Utilities.RandomFloat(minDistance, maxDistance);
            float endpointRotFromPlayer = Utilities.RandomAngle();

            // calculate endpoint position
            Vector2 endpointPosition = Owner.Position + endpointDistFromPlayer * Utilities.AngleToVector(endpointRotFromPlayer);

            Vector2[] lightningVertices = new Vector2[lightningPoints];

            // precalculate all lightning endpoints
            // the first one is just the player position (change this if it looks bad)

            lightningVertices[0] = Owner.Position;

            float lengthOfEach = endpointDistFromPlayer / lightningPoints;

            // do the last one separately as it is just the endpoint position
            for (int i = 1; i < lightningPoints - 1; i++)
            {
                // take a direction vector from the last decided endpoint to the end.
                Vector2 fromLastToEnd = Utilities.SafeNormalise(endpointPosition - lightningVertices[i-1]);
                // calculate an angle to offset the next line in the chain by.
                float angleOffset = Utilities.RandomAngle(-lightningAngleVariance, lightningAngleVariance);
                // calculate the next vertex by scaling its direction vector and rotating it.
                lightningVertices[i] = lightningVertices[i - 1] + Utilities.RotateVectorClockwise(lengthOfEach * fromLastToEnd, angleOffset);
            }

            lightningVertices[lightningPoints - 1] = endpointPosition;

            return lightningVertices;
        }

        public void CreateLightningPrims()
        {
            float lightningThickness = 10f;

            Vector2[] lightningBasePoints = CreateLightningPoints();

            for (int i = 0; i < lightningBasePoints.Length - 1; i++)
            {                
                float progress = (i + 1) / (float)lightningBasePoints.Length;
                float thickness = MathHelper.Lerp(lightningThickness, 0f, progress);

                // calculate the angle from one lightning point to the next.
                float angleToNext = Utilities.VectorToAngle(lightningBasePoints[i + 1] - lightningBasePoints[i]);
                // use this to make two points on either side of each lightning point that are perpendicular to the angle between the lightning points
                float distBetweenBasePointAndVertex = thickness / 2f;

                Vector2 vertex1 = lightningBasePoints[i] + distBetweenBasePointAndVertex * Utilities.AngleToVector(angleToNext + PI / 2);
                Vector2 vertex2 = lightningBasePoints[i] + distBetweenBasePointAndVertex * Utilities.AngleToVector(angleToNext - PI / 2);

                Vector2 vertex3 = lightningBasePoints[i + 1] + distBetweenBasePointAndVertex * Utilities.AngleToVector(angleToNext + PI / 2);
                Vector2 vertex4 = lightningBasePoints[i + 1] + distBetweenBasePointAndVertex * Utilities.AngleToVector(angleToNext - PI / 2);

                int startingIndex = i * 4;

                PrimitiveManager.MainVertices[startingIndex] = PrimitiveManager.CreateVertex(vertex1, Color.LightSkyBlue, new Vector2(0f , 0f));
                PrimitiveManager.MainVertices[startingIndex + 1] = PrimitiveManager.CreateVertex(vertex2, Color.LightSkyBlue, new Vector2(1f, 0f));
                PrimitiveManager.MainVertices[startingIndex + 2] = PrimitiveManager.CreateVertex(vertex3, Color.LightSkyBlue, new Vector2(0f, progress));
                PrimitiveManager.MainVertices[startingIndex + 3] = PrimitiveManager.CreateVertex(vertex4, Color.LightSkyBlue, new Vector2(1f, progress));
            }

            //now we assign the indice buffer

            int numberOfTriangles = 2 * lightningBasePoints.Length - 2;
            int vertexCount = 4 * lightningBasePoints.Length - 4;
            int indexCount = numberOfTriangles * 3;

            for (int i = 0; i < numberOfTriangles; i++)
            {
                int startingIndex = i * 3;
                PrimitiveManager.MainIndices[startingIndex] = (short)i;
                PrimitiveManager.MainIndices[startingIndex + 1] = (short)(i + 1);
                PrimitiveManager.MainIndices[startingIndex + 2] = (short)(i + 2);
            }

            Effect shader = AssetRegistry.GetShader("LightningShader");
            shader.Parameters["colour"]?.SetValue(new Vector3(0f, 0.59f, 1f));
            shader.Parameters["noiseMap"]?.SetValue(AssetRegistry.GetTexture2D("LightningNoise"));
            PrimitiveSet primSet = new PrimitiveSet(vertexCount, indexCount, shader);

            primSet.Draw();
        }
        public override void AI()
        {
            if (Charging())
            {
                if (ChargeTimer < FullChargeTime)
                {
                    ChargeTimer++;
                }

                if (!Charged())
                {
                    Particle p = new Particle();
                    p.FadeOutParticle(Utilities.RandomAngle(), 10f, 20, Owner.Position, Color.LightSkyBlue);
                }
            }

            if (Charged() && LeftClickReleased())
            {
                BurstTimeLeft = BurstDuration;
            }

            if (LeftClickReleased())
            {
                ChargeTimer = 0;
            }

            if (Firing())
            {
                if (BurstTimeLeft > 0)
                {
                    BurstTimeLeft--;

                    Random rnd = new Random();
                    float angleVariance = PI / 120;
                    float angle = Utilities.RandomAngle(-angleVariance, angleVariance);
                    
                    Projectile playerProjectile = SpawnProjectile(Owner.Position, 20f * Utilities.RotateVectorClockwise(Utilities.Normalise(MousePositionWithCamera() - Owner.Position), angle),
                        0.4f, 1, "MachineGunProjectile", Vector2.One, Owner, false, Color.LightBlue, true, true);

                    Vector2 toMouse = Utilities.SafeNormalise(MousePositionWithCamera() - Owner.Position);

                    playerProjectile.Rotation = Utilities.VectorToAngle(Utilities.RotateVectorClockwise(toMouse, angle));

                    Owner.Velocity += -BurstTimeLeft * 2f * toMouse;
                }
            }

        }

        public bool Charging()
        {
            // if we are charging up and left click is held (chaneg to button when keybind changing is added) and we are not firing
            return IsLeftClickDown() && BurstTimeLeft == 0;
        }
        public bool Charged()
        {
            return ChargeTimer == FullChargeTime;
        }
        public bool Firing()
        {
            return BurstTimeLeft > 0;
        }

        public override void PrimaryFire()
        {
            
        }
        public override void SecondaryFire()
        {

        }

        public override void Draw(SpriteBatch s)
        {
            if (Charged())
            {
                CreateLightningPrims();
            }

            //foreach (Vector2 vertex in CreateLightningVertices())
            //{
            //    Texture2D texture = AssetRegistry.GetTexture2D("box");
            //    Drawing.BetterDraw(texture, vertex, null, Color.Red, 0f, Vector2.One * 0.2f, SpriteEffects.None, 0f);
            //}

            //Utilities.drawTextInDrawMethod("charge timer " + ChargeTimer.ToString(), Owner.Position + new Vector2(0f, 100f), s, font, Color.White);
            //Utilities.drawTextInDrawMethod("charging " + Charging().ToString(), Owner.Position + new Vector2(0f, 150f), s, font, Color.White);
            //Utilities.drawTextInDrawMethod("charged " + Charged().ToString(), Owner.Position + new Vector2(0f, 200f), s, font, Color.White);
            //Utilities.drawTextInDrawMethod("firing " + Firing().ToString(), Owner.Position + new Vector2(0f, 250f), s, font, Color.White);
        }
    }
}