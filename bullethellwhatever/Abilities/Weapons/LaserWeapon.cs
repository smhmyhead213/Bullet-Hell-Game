using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;
using bullethellwhatever.Projectiles;
using bullethellwhatever.Projectiles.Base;
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
    public class LaserWeapon : Weapon
    {
        public int FullChargeTime;
        public int ChargeTimer;
        public int LaserDuration;
        public int Cooldown;
        public int CooldownTimer;
        public LaserWeapon(Player player) : base(player)
        {
            
        }
        public override void WeaponInitialise()
        {
            PrimaryFireCoolDownDuration = 3;
            Cooldown = 30;
            CooldownTimer = 0;
            FullChargeTime = 45;
            LaserDuration = 20;
            ChargeTimer = 0;
        }
        public override bool CanSwitchWeapon()
        {
            return true;
        }

        public Vector2[] CreateLightningPoints()
        {
            // pick an endpoint for the lightning trail at a varying distance from the player
            float minDistance = 70f;
            float maxDistance = 100f;
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

                int startingIndex = i * 2;

                PrimitiveManager.MainVertices[startingIndex] = PrimitiveManager.CreateVertex(vertex1, Color.LightSkyBlue, new Vector2(0f , 0f));
                PrimitiveManager.MainVertices[startingIndex + 1] = PrimitiveManager.CreateVertex(vertex2, Color.LightSkyBlue, new Vector2(1f, 0f));
            }

            //now we assign the indice buffer

            int numberOfTriangles = 2 * (lightningBasePoints.Length - 1) - 2;
            int vertexCount = (lightningBasePoints.Length - 1) * 2;
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
            shader.Parameters["noiseMap"]?.SetValue(AssetRegistry.GetTexture2D("CrabScrollingBeamNoise"));
            PrimitiveSet primSet = new PrimitiveSet(vertexCount, indexCount, shader);

            primSet.Draw();
        }
        public override void AI()
        {
            if (OnCooldown())
            {
                CooldownTimer--;
            }          
        }

        public override void LeftClickReleasedBehaviour()
        {
            ChargeTimer = 0;

            if (Charged())
            {
                CooldownTimer = Cooldown;

                float damage = 0.1f;

                Deathray d = SpawnDeathray(Owner.Position, Utilities.VectorToAngle(MousePositionWithCamera() - Owner.Position), damage, LaserDuration, "box", 30, 2000, 0, false, Color.LightSkyBlue, "PlayerDeathrayShader", Owner);

                d.SetThinOut(true);

                d.SetStayWithOwner(true);

                d.SetNoiseMap("CrabScrollingBeamNoise", -0.06f);

                d.SetExtraAI(new Action(() =>
                {
                    d.Rotation = Utilities.VectorToAngle(MousePositionWithCamera() - Owner.Position);
                }));
            }
        }
        public override void LeftClickHeldBehaviour()
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
        }
        public bool Charging()
        {
            // if we are charging up and left click is held (chaneg to button when keybind changing is added) and we are not firing
            return CooldownTimer == 0;
        }
        public bool Charged()
        {
            return ChargeTimer == FullChargeTime;
        }
        public bool OnCooldown()
        {
            return CooldownTimer > 0;
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
        }
    }
}
