using bullethellwhatever.MainFiles;
using bullethellwhatever.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Projectiles
{
    public static class CommonProjectileAIs
    {
        public static Action Homing(Projectile projectile)
        {
            return () =>
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
            };
        }
    }
}
