using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

 
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using bullethellwhatever.MainFiles;
using bullethellwhatever.UtilitySystems;
using System.Runtime.CompilerServices;
using bullethellwhatever.BaseClasses.Hitboxes;
using bullethellwhatever.Projectiles;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class PhaseTwoBulletHell : EyeBossAttack
    {
        public PhaseTwoBulletHell(EyeBoss owner) : base(owner)
        {

        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(int AITimer)
        {
            int time = AITimer;
            int attackStartTime = 200;

            EyeBossPhaseTwoMinion owner = (EyeBossPhaseTwoMinion)Owner;
            EyeBoss mainBoss = owner.Owner;

            if (EntityManager.activeNPCs.Count == 2) // boss and pupil are alive
            {
                Owner.TargetableByHoming = true;
                Owner.IsInvincible = false;

                AttackUtilities.ClearProjectiles();
                EyeOwner.Phase++;
            }

            if (time > attackStartTime)
            {
                bool attack = true; // true, shoot bullet rings, false, lasers

                if (owner.IsEntityWithinVulnerabilityRadius(player))
                {
                    attack = false;
                }

                if (attack)
                {
                    if (time % 48 == 0)
                    {
                        int projectilesPerRing = 9;

                        float randomOffset = Utilities.RandomFloat(0, Tau);

                        for (int i = 0; i < projectilesPerRing; i++)
                        {
                            if (!Utilities.RandomChance(20))
                            {
                                Projectile p = SpawnProjectile(Pupil.Position, 2f * Utilities.RotateVectorClockwise(Vector2.UnitY, i * Tau / projectilesPerRing + randomOffset), 1f, 1, "box", Vector2.One * 0.8f, Owner, true, false, Color.Red, true, false);

                                p.Rotation = Utilities.VectorToAngle(p.Velocity);
                            }
                            else // healing projectile
                            {
                                Projectile p = SpawnProjectile(Pupil.Position, 2f * Utilities.RotateVectorClockwise(Vector2.UnitY, i * Tau / projectilesPerRing + randomOffset), 0f, 1, "box", Vector2.One * 0.8f, Owner, true, false, Color.LimeGreen, true, true);

                                p.Rotation = Utilities.VectorToAngle(p.Velocity);

                                p.SetOnHit(new Action(() =>
                                {
                                    player.Heal(0.5f);
                                }));
                            }
                        }
                    }
                    //int localTime = time - attackStartTime;

                    //if (owner.Index == 1)
                    //{              
                    //    int eyeShrinkTime = 75;
                    //    int eyeExpandTime = 15;
                    //    int waitTime = 30;

                    //    int cycleTime = eyeShrinkTime + eyeExpandTime + waitTime;
                    //    int modulodTime = localTime % cycleTime;

                    //    if (modulodTime < eyeShrinkTime)
                    //    {
                    //        float progress = modulodTime / (float)eyeShrinkTime;
                    //        float easedProgress = 1 - Pow(1 - progress, 4);
                    //        owner.Pupil.Size = Vector2.Lerp(owner.Pupil.InitialSize, owner.Pupil.InitialSize / 5f, easedProgress);
                    //    }
                    //    else if (modulodTime == eyeShrinkTime)
                    //    {
                    //        int numberOfProjectiles = 25;

                    //        for (int i = 0; i < numberOfProjectiles; i++)
                    //        {
                    //            float xVelAmplitude = 20f;
                    //            float xVelocity = Utilities.RandomFloat(-xVelAmplitude, xVelAmplitude);

                    //            float yVelAmplitude = 20f;
                    //            float yVelocity = Utilities.RandomFloat(-yVelAmplitude, 0);

                    //            Projectile p = new Projectile();

                    //            p.SetExtraAI(new Action(() =>
                    //            {
                    //                p.Velocity.Y = p.Velocity.Y + 0.3f; // gravity
                    //            }));

                    //            p.SetDrawAfterimages(28, 3);

                    //            p.SetEdgeTouchEffect(new Action(() =>
                    //            {
                    //                AttackUtilities.ParticleExplosion(15, 20, 10f, Vector2.One * 0.45f, p.Position, p.Colour);
                    //            }));

                    //            p.Spawn(Pupil.Position, new Vector2(xVelocity, yVelocity), 1f, 1, "box", 1f, Vector2.One * 0.85f, Pupil, true, Color.Red, true, false);
                    //        }
                    //    }
                    //    else if (modulodTime > eyeShrinkTime && modulodTime <= eyeShrinkTime + eyeExpandTime)
                    //    {
                    //        float progress = (modulodTime - eyeShrinkTime) / (float)eyeExpandTime;

                    //        owner.Pupil.Size = Vector2.Lerp(owner.Pupil.InitialSize / 5f, owner.Pupil.InitialSize, progress);
                    //    }
                    //}
                    //else if (owner.Index == 2)
                    //{
                    //    if (localTime % 90 == 0)
                    //    {a
                    //        Projectile p = new Projectile();

                    //        p.SetExtraAI(new Action(() =>
                    //        {
                    //            p.HomeAtTarget(player.Position, 0.03f);
                    //        }));

                    //        p.SetDrawAfterimages(15, 2);

                    //        p.Spawn(owner.Pupil.Position, Utilities.UnitVectorToPlayerFrom(Pupil.Position), 1f, 1, "Circle", 1.02f, Vector2.One * 0.25f, Pupil, true, Color.Orange, true, false);
                    //    }
                    //}
                }
                else
                {
                    Pupil.LookAtPlayer(10);

                    if (time % 240 == 0)
                    {
                        int lasers = 5;
                        float initialRotation = Utilities.RandomAngle();

                        for (int i = 0; i < lasers; i++)
                        {
                            TelegraphLine t = SpawnTelegraphLine(Tau / lasers * i + initialRotation, 0, 20, Utilities.ScreenDiagonalLength(), 120, Pupil.Position, Color.White, "box", Pupil, true);

                            t.SetOnDeath(new Action(() =>
                            {
                                Deathray ray = SpawnDeathray(t.Origin, t.Rotation, 1f, 45, "box", t.Width, t.Length, 0, true, Color.Gold, "DeathrayShader", Pupil);
                                ray.SetStayWithOwner(true);
                            }));
                        }
                    }
                }
            }

            int expansionTime = 40;

            if (time > attackStartTime && time <= attackStartTime + expansionTime)
            {
                float maxVulnerabilityRadius = 270f;
                float progress = (time - attackStartTime) / (float)expansionTime;
                float easedProgress = 1 - Pow(1 - progress, 5);

                owner.BaseVulnerabilityRadius = MathHelper.Lerp(0f, maxVulnerabilityRadius, easedProgress);

                if (time == attackStartTime + expansionTime)
                {
                    owner.OscillateRadius = true;
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
            //Utilities.drawTextInDrawMethod(aitimer.ToString(), new Vector2(ScreenWidth / 4f * 3f, ScreenHeight / 4f * 3f), s, font, Color.White);
        }
    }
}
