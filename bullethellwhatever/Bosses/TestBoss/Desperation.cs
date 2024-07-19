using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.DrawCode;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Projectiles;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class Desperation : BossAttack
    {
        public int DespBeamRotation;
        public int ProjectilesPerBomb;
        public Desperation(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    ProjectilesPerBomb = 0;
                    break;
                case GameState.Difficulties.Normal:
                    ProjectilesPerBomb = 0;
                    break;
                case GameState.Difficulties.Hard:
                    ProjectilesPerBomb = 24;
                    break;
                case GameState.Difficulties.Insane:
                    ProjectilesPerBomb = 30;
                    break;

            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            if (GameState.Difficulty == GameState.Difficulties.Easy || GameState.Difficulty == GameState.Difficulties.Normal)
            {
                Owner.IsDesperationOver = true;
            }

            int despStartTime = 300;

            if (AITimer == 0)
            {
                Owner.DealDamage = false;
                Owner.Velocity = Vector2.Zero; //amke it sit in the middle
                Owner.Rotation = 0;
                Owner.dialogueSystem.Dialogue("It's not over yet!", 4, despStartTime);
                Drawing.ScreenShake(4, EndTime - despStartTime);

                Owner.TargetableByHoming = false;

                foreach (NPC npc in activeNPCs)
                {
                    npc.Die();
                }

                foreach (Projectile projectile in activeProjectiles)
                {
                    projectile.Die();
                }
            }

            if (AITimer < despStartTime)
            {
                MoveToPoint(new Vector2(ScreenWidth / 10, ScreenHeight / 10), AITimer, despStartTime);
            }

            if (AITimer == despStartTime)
            {
                Owner.dialogueSystem.ClearDialogue();
            }

            if (AITimer > despStartTime && AITimer % 30 == 0)
            {
                Projectile bomb = SpawnProjectile(Owner.Position, 8f * Utilities.SafeNormalise(player.Position - Owner.Position), 1f, 1, "box", new Vector2(2,2), Owner, true, Color.Red, true, false);

                bomb.SetExtraAI(new Action(() =>
                {
                    if (bomb.AITimer == 180)
                    {
                        Projectile fragment = new Projectile(bomb.Position, 3f * Vector2.One, 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                        RadialProjectileBurst(fragment, ProjectilesPerBomb, 0f, 3f);
                    }
                }));
            }

            if (AITimer == EndTime)
            {
                musicSystem.StopMusic();
                Owner.IsDesperationOver = true; //die
            }
        }
    }
}
