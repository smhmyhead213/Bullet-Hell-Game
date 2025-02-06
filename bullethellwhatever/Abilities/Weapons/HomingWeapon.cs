using bullethellwhatever.BaseClasses;
using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;
using bullethellwhatever.Projectiles;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.DrawCode.Particles;

namespace bullethellwhatever.Abilities.Weapons
{
    public class HomingWeapon : Weapon
    {
        public HomingWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {
            
        }
        public override void WeaponInitialise()
        {
            PrimaryFireCoolDownDuration = 10;
        }
        public override bool CanSwitchWeapon()
        {
            return true;
        }

        public override void AI()
        {
            
        }

        public override void PrimaryFire()
        {
            float initialVelocity = 7f;
            float damage = 0.28f * 100f;

            Projectile projectile = SpawnProjectile<Projectile>(Owner.Position, initialVelocity * Utilities.Normalise(MousePositionWithCamera() - Owner.Position), damage, 1, "box", Vector2.One, Owner, false, true, Color.LimeGreen, true, true);

            projectile.SetExtraData(0, 0); // extra data 0 represents how long the projectile has gone without a target

            projectile.SetUpdates(2);

            projectile.AddTrail(22);

            projectile.SetOnHit(new Action(() =>
            {
                if (projectile.Owner == player)
                {
                    int numberOfParticles = Utilities.RandomInt(1, 4);

                    for (int i = 0; i < numberOfParticles; i++)
                    {
                        float rotation = Utilities.RandomFloat(0, Tau);

                        Particle p = new Particle();

                        Vector2 velocity = 10f * Utilities.RotateVectorClockwise(-Vector2.UnitY, rotation);
                        int lifetime = 20;

                        p.Spawn("box", projectile.Position, velocity, -velocity / 2f / lifetime, Vector2.One * 0.45f, rotation, projectile.Colour, 1f, 20);
                    }
                }
            }));
            projectile.SetExtraAI(new Action(() =>
            {
                ref float TimeWithNoTarget = ref projectile.ExtraData[0];

                int homingTime = 30 * projectile.Updates;

                NPC closestNPC = new NPC(); //the target
                float minDistance = float.MaxValue;

                if (projectile.AITimer > homingTime)
                {
                    bool validTarget = false;

                    if (EntityManager.activeNPCs.Count > 0)
                    {
                        projectile.Opacity = 1f; // come back to full opacity if a target is found while fading out

                        foreach (NPC npc in EntityManager.activeNPCs)
                        {
                            if (npc.TargetableByHoming && npc.Participating)
                            {
                                float distance = Utilities.DistanceBetweenEntities(projectile, npc);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    closestNPC = npc;
                                }

                                validTarget = true;
                            }
                        }

                        if (validTarget)
                            projectile.Velocity = 0.4f / projectile.Updates * Vector2.Normalize(closestNPC.Position - projectile.Position) * (projectile.AITimer - homingTime);
                    }

                    else
                    {
                        TimeWithNoTarget++;
                        projectile.Velocity = projectile.Velocity * 0.98f; // slow down if no target
                    }

                    if (!validTarget)
                    {
                        TimeWithNoTarget++;
                        projectile.Velocity = projectile.Velocity * 0.98f; // slow down if no target
                    }
                }

                int beginFading = 50;

                if (TimeWithNoTarget >= beginFading) //after a second of not finding a target
                {
                    // if almost expired, start fading out

                    int fadeOutTime = 60;

                    projectile.Opacity = MathHelper.Lerp(1f, 0f, ((float)TimeWithNoTarget - beginFading) / fadeOutTime);

                    if (TimeWithNoTarget > fadeOutTime + beginFading)
                    {
                        projectile.DeleteNextFrame = true;

                        projectile.OnHitEffect(projectile.Position);
                    }
                }

                projectile.Rotation = Utilities.VectorToAngle(projectile.Velocity);
            }));
        }
        public override void SecondaryFire()
        {

        }
    }
}
