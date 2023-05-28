using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.UtilitySystems.Dialogue;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class Desperation : BossAttack
    {
        public int DespBeamRotation;
        public int BlenderBeams;
        public Desperation(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    BlenderBeams = 0;
                    break;
                case GameState.Difficulties.Normal:
                    BlenderBeams = 0;
                    break;
                case GameState.Difficulties.Hard:
                    BlenderBeams = 2;
                    break;
                case GameState.Difficulties.Insane:
                    BlenderBeams = 3;
                    break;

            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            if (GameState.Difficulty == GameState.Difficulties.Easy || GameState.Difficulty == GameState.Difficulties.Normal)
            {
                Owner.IsDesperationOver = true;
            }

            int despStartTime = 400;

            if (AITimer == 0)
            {
                Owner.Velocity = Vector2.Zero; //amke it sit in the middle
                Owner.Rotation = 0;
                Owner.dialogueSystem.Dialogue(Owner.Position, "It's not over yet!", 4, despStartTime);
                Drawing.ScreenShake(4, EndTime - despStartTime);

                for (int i = 0; i < BlenderBeams; i++)
                {
                    TelegraphLine telegraphLine = new TelegraphLine(MathHelper.TwoPi / BlenderBeams * i, 0, 0, 15, 1500f, despStartTime, Owner.Position, Color.White, "box", Owner);
                }
            }

            if (AITimer < despStartTime)
            {
                MoveToCentre(AITimer, despStartTime);
            }

            if (AITimer == 400)
            {
                DespBeamRotation = Owner.Position.X > Main.player.Position.X ? 1 : -1;
                Main.activeProjectiles.Clear();

                for (int i = 0; i < 2; i++)
                {
                    Deathray ray = new Deathray();
                    ray.SpawnDeathray(Owner.Position, MathHelper.TwoPi / 2 * i, 1f, 2600, "box", 40f, 1500f, DespBeamRotation * 40f, 0f, true, Color.Red, "DeathrayShader2", Owner);
                }

                Owner.dialogueSystem.ClearDialogue();
            }

            if (AITimer == EndTime)
            {
                Main.musicSystem.StopMusic();
                Owner.IsDesperationOver = true; //die
            }
        }
    }
}
