using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
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

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class PhaseTwoBulletHell : EyeBossAttack
    {
        public PhaseTwoBulletHell(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer;
      
            if (activeNPCs.Count == 2) // boss and pupil are alive
            {
                Owner.TargetableByHoming = true;
                Owner.IsInvincible = false;

                AttackUtilities.ClearProjectiles();
                EyeOwner.Phase++;
                EyeOwner.ReplaceAttackPattern(EyeOwner.OriginalAttacks);
                EyeOwner.RandomlyArrangeAttacks();
            }

            else if (AITimer > 200 && time % 20 == 0)
            {
                int projectilesPerRing;
                bool attack = true; // true, shoot bullet rings, false, lasers

                if (Owner is EyeBossPhaseTwoMinion)
                {
                    projectilesPerRing = 10;
                    if (((EyeBossPhaseTwoMinion)Owner).IsPlayerWithinVulnerabilityRadius())
                    {
                        attack = false;
                    }
                }
                else projectilesPerRing = 20;

                if (attack)
                {
                    float randomOffset = Utilities.RandomFloat(0, Tau);

                    for (int i = 0; i < projectilesPerRing; i++)
                    {
                        if (!Utilities.RandomChance(20))
                        {
                            Projectile p = new Projectile();

                            p.Spawn(Pupil.Position, 2f * Utilities.RotateVectorClockwise(Vector2.UnitY, i * Tau / projectilesPerRing + randomOffset), 1f, 1, "box", 1f, Vector2.One * 0.8f, Owner, true, Color.Red, true, false);
                        }
                        else
                        {
                            PlayerHealingProjectile p = new PlayerHealingProjectile(0.5f);

                            p.Spawn(Pupil.Position, 2f * Utilities.RotateVectorClockwise(Vector2.UnitY, i * Tau / projectilesPerRing + randomOffset), 1f, 1, "box", 1f, Vector2.One * 0.8f, Owner, true, Color.LimeGreen, true, false);
                        }
                    }
                }
                else
                {
                    if (time % 240 == 0)
                    {
                        int lasers = 5;

                        for (int i = 0; i < lasers; i++)
                        {
                            TelegraphLine t = new TelegraphLine(Tau / lasers * i, 0, 0, 20, Utilities.ScreenDiagonalLength(), 120, Pupil.Position, Color.White, "box", Pupil, true);

                            t.SetOnDeath(new Action(() =>
                            {
                                Deathray ray = new Deathray();

                                ray.SpawnDeathray(t.Origin, t.Rotation, 1f, 45, "box", t.Width, t.Length, 0, 0, true, Color.Gold, "DeathrayShader", Pupil);
                            }));
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
