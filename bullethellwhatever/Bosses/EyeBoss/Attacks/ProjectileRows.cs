using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

 
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using bullethellwhatever.Projectiles;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class ProjectileRows : EyeBossAttack
    {
        public ProjectileRows(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int animationTime = 20;
            int projectileBufferTime = 5; // time before eye looks back to player
            int waitTimeAfter = 10;
            int cycleTime = animationTime + waitTimeAfter;
            int time = AITimer % cycleTime;

            if (AITimer < EndTime - cycleTime * 2)
            {
                if (time < animationTime)
                {
                    float maxDistance = 30f;
                    Pupil.GoTo(time / (float)animationTime * maxDistance, Utilities.AngleToPlayerFrom(Pupil.Position) - PI);
                }

                if (time == animationTime)
                {
                    int projectilesInRow = 50;
                    float distanceBetweenProjectiles = 120;
                    float distanceBehindEye = 600f;

                    int timeToReachDestination = projectileBufferTime;
                    float angleToPlayer = Utilities.AngleToPlayerFrom(Pupil.Position);

                    for (int i = -projectilesInRow / 2; i < projectilesInRow / 2; i++)
                    {
                        Vector2 destination = Pupil.Position + Utilities.RotateVectorClockwise(new Vector2(i * distanceBetweenProjectiles, -distanceBehindEye), angleToPlayer - PI); // error is here

                        Vector2 velocity = (destination - Pupil.Position) / timeToReachDestination;

                        Projectile p = SpawnProjectile(Pupil.Position, velocity, 1f, 1, "box", Vector2.One, Pupil, true, Color.Red, true, false); ;

                        int projectileExtraUpdates = 2;

                        p.AddTrail(11 * projectileExtraUpdates);

                        p.MercyTimeBeforeRemoval = 240 * projectileExtraUpdates;

                        p.SetUpdates(projectileExtraUpdates);

                        p.SetExtraAI(new Action(() =>
                        {
                            if (p.AITimer == timeToReachDestination)
                            {
                                Vector2 toPlayer = player.Position - Pupil.Position;

                                //TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(toPlayer), 0, 0, 10, Utilities.ScreenDiagonalLength(), 40, p.Position, Color.White, "box", p, true);

                                //t.ThickenIn = true;

                                Vector2 newVelocity = 1.5f * Utilities.SafeNormalise(toPlayer);

                                p.Velocity = newVelocity;
                                p.Rotation = Utilities.VectorToAngle(newVelocity);
                                p.Velocity *= 1.013f;
                            }
                        }));
                    }
                }

                if (time >= animationTime && time < animationTime + projectileBufferTime)
                {
                    float maxDistance = 60f;
                    float interpolant = ((time - animationTime) / (float)projectileBufferTime) - 0.5f; // instead of 0 - 1, go -0.5 to 5
                    Pupil.GoTo(interpolant * maxDistance, Utilities.AngleToPlayerFrom(Pupil.Position));
                }
            }         
        }

        public override void ExtraAttackEnd()
        {
            base.ExtraAttackEnd();

            Pupil.ResetSize();
        }
        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}
