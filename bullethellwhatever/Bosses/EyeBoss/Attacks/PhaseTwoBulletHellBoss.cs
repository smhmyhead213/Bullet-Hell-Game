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
using SharpDX.MediaFoundation;
using bullethellwhatever.Projectiles;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class PhaseTwoBulletHellBoss : EyeBossAttack
    {
        public int TimerAfterAllMinionsDie;
        public float BossInitialHP; // hp before the boss starts to heal
        public float BossHPRunningTotal; // to detemine the amount of HP the boss has after the current projectiles heal it
        public PhaseTwoBulletHellBoss(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
            TimerAfterAllMinionsDie = 0;
            BossHPRunningTotal = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer;
            int startTime = 100;
            int minionDeployTime = 240;

            if (EntityManager.activeNPCs.Count == 2 && AITimer > minionDeployTime) // only boss and pupil are alive
            {
                Owner.TargetableByHoming = true;
                Owner.IsInvincible = false;

                AttackUtilities.ClearProjectiles();

                float eyeRecentreTime = 30;
                float healTime = 60;
                int waitTimeBefore = 180;
                int waitTimeAfterSpawningHealing = 90;

                int timeAfterWait = TimerAfterAllMinionsDie - waitTimeBefore; // time elapsed since the wait ended

                if (TimerAfterAllMinionsDie > waitTimeBefore)
                {
                    if (timeAfterWait == eyeRecentreTime)
                    {
                        BossInitialHP = Owner.Health;
                        BossHPRunningTotal = BossInitialHP;
                    }

                    if (timeAfterWait <= eyeRecentreTime)
                    {
                        float progress = timeAfterWait / eyeRecentreTime;

                        Pupil.DistanceFromEyeCentre = MathHelper.Lerp(Pupil.DistanceFromEyeCentre, 0f, progress);
                    }
                    else if (timeAfterWait > eyeRecentreTime && timeAfterWait < healTime + eyeRecentreTime)
                    {
                        float progress = (timeAfterWait - eyeRecentreTime) / healTime;

                        float easedProgress = 1 - Pow(1 - progress, 5);

                        float currentHP = BossHPRunningTotal;
                        float HPafterThisFrame = MathHelper.Lerp(BossInitialHP, Owner.MaxHP, easedProgress);
                        float totalHeal = HPafterThisFrame - currentHP;

                        BossHPRunningTotal = BossHPRunningTotal + totalHeal;

                        int numberOfProjectiles = (int)MathHelper.Lerp(10, 1, progress);

                        float healEach = totalHeal / numberOfProjectiles;

                        for (int i = 0; i < numberOfProjectiles; i++)
                        {
                            float speed = Utilities.RandomFloat(10f, 20f);

                            float randomAngle = Utilities.RandomAngle();
                            float spawnDistance = 1500f;

                            // spawn each healing projecile at a random point on a circle around the boss
                            Vector2 spawnPos = Pupil.Position + Utilities.RotateVectorClockwise(spawnDistance * Vector2.UnitX, randomAngle);
                            Vector2 velocity = speed * Utilities.SafeNormalise(Pupil.Position - spawnPos);

                            Projectile p = SpawnProjectile(spawnPos, velocity, 0f, 1, "Circle", Vector2.One * 0.1f, Owner, false, Color.LightBlue, false, true);

                            p.SetExtraAI(new Action(() =>
                            {
                                if (p.IsCollidingWith(Owner))
                                {
                                    p.DeleteNextFrame = true;
                                    Owner.Heal(healEach);
                                }

                                p.Velocity *= 1.05f;
                            }));

                            p.SetDrawAfterimages(11, 7);

                            p.SetParticipating(false);
                        }
                    }
                    else if (timeAfterWait == healTime + eyeRecentreTime + waitTimeAfterSpawningHealing)
                    {
                        EyeOwner.Phase++;
                        EyeOwner.ReplaceAttackPattern(EyeOwner.OriginalAttacks);
                        EyeOwner.RandomlyArrangeAttacks();
                    }
                }

                TimerAfterAllMinionsDie++;
            }
            else
            {
                if (time == 0)
                {
                    AttackUtilities.ClearProjectiles();

                    Owner.activeTelegraphs.Clear();
                }

                if (time == 12) // for good measure
                {
                    AttackUtilities.ClearProjectiles();
                    Owner.activeTelegraphs.Clear();
                }
                if (time == startTime)
                {
                    Drawing.ScreenShake(16, minionDeployTime - startTime);
                }

                if (time >= startTime && time < minionDeployTime)
                {
                    int pupilBeAtCentreTime = minionDeployTime - 20;

                    if (time < pupilBeAtCentreTime)
                    {
                        float progress = time - startTime;

                        progress = progress / (pupilBeAtCentreTime - startTime);

                        Pupil.DistanceFromEyeCentre = MathHelper.Lerp(Pupil.DistanceFromEyeCentre, 0f, progress);
                        Pupil.Size = Vector2.Lerp(Pupil.Size, Pupil.InitialSize, progress);
                    }
                    if (time % 10 == 0)
                    {
                        ShockwaveRing sRing = new ShockwaveRing(20, 80, 200, 10);

                        sRing.Spawn(Pupil.Position, Owner, Color.LightGray);
                    }
                }

                if (time == minionDeployTime)
                {
                    EyeBossPhaseTwoMinion[] minions = new EyeBossPhaseTwoMinion[4];

                    minions[0] = new EyeBossPhaseTwoMinion(EyeOwner, (int)GameHeight / 4, new Vector2(GameWidth / 3, 0), 1);
                    minions[1] = new EyeBossPhaseTwoMinion(EyeOwner, (int)GameHeight / 4 * 3, new Vector2(GameWidth / 5, 0), 2);
                    minions[2] = new EyeBossPhaseTwoMinion(EyeOwner, (int)GameHeight / 4, new Vector2(GameWidth - GameWidth / 3, 0), 3);
                    minions[3] = new EyeBossPhaseTwoMinion(EyeOwner, (int)GameHeight / 4 * 3, new Vector2(GameWidth - GameWidth / 5, 0), 4);

                    foreach (EyeBossPhaseTwoMinion minion in minions)
                    {
                        minion.Spawn(minion.ChainStartPosition, Vector2.Zero, 1f, "Circle", Owner.Size / 2f, minion.MaxHP, 1, Color.White, false, true);
                    }
                }

                else if (time > minionDeployTime)
                {
                    Pupil.LookAtPlayer(20);

                    if (time % 20 == 0)
                    {
                        int projectilesPerRing = 20;
                        bool attack = true; // true, shoot bullet rings, false, lasers

                        if (attack)
                        {
                            float randomOffset = Utilities.RandomFloat(0, Tau);

                            for (int i = 0; i < projectilesPerRing; i++)
                            {
                                if (!Utilities.RandomChance(20))
                                {
                                    Projectile p = SpawnProjectile(Pupil.Position, 2f * Utilities.RotateVectorClockwise(Vector2.UnitY, i * Tau / projectilesPerRing + randomOffset), 1f, 1, "box", Vector2.One * 0.8f, Owner, true, Color.Red, true, false);

                                    p.Rotation = Utilities.VectorToAngle(p.Velocity);
                                }
                                else
                                {
                                    Projectile p = SpawnProjectile(Pupil.Position, 2f * Utilities.RotateVectorClockwise(Vector2.UnitY, i * Tau / projectilesPerRing + randomOffset), 0f, 1, "box", Vector2.One * 0.8f, Owner, true, Color.LimeGreen, true, true);

                                    p.Rotation = Utilities.VectorToAngle(p.Velocity);

                                    p.SetOnHit(new Action(() =>
                                    {
                                        player.Heal(0.5f);
                                    }));
                                }
                            }
                        }
                        else
                        {
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
