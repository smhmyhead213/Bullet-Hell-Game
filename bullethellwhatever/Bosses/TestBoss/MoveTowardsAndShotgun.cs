﻿using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems.Dialogue;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class MoveTowardsAndShotgun : BossAttack
    {
        public int NumberOfProjectiles;
        public float ProjectileSpeed;
        public int ShotgunFrequency;

        public MoveTowardsAndShotgun(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }
        public override void InitialiseAttackValues()
        {
            ShotgunFrequency = 10;

            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    NumberOfProjectiles = 1;
                    ProjectileSpeed = 9f;
                    break;
                case GameState.Difficulties.Normal:
                    NumberOfProjectiles = 3;
                    ProjectileSpeed = 11f;
                    break;
                case GameState.Difficulties.Hard:
                    NumberOfProjectiles = 3;
                    ProjectileSpeed = 13f;
                    break;
                case GameState.Difficulties.Insane:
                    NumberOfProjectiles = 5;
                    ProjectileSpeed = 15f;
                    break;
            }
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            float angleBetweenProjectiles = MathF.PI / 12;

            if (AITimer == 0) //this could be optimised by giving every entity a rotation from vertical field
            {
                Owner.dialogueSystem.Dialogue("Test dialogue", 4, 800);
            }

            if (AITimer == 510)
            {
                ShotgunFrequency = 10;
                Owner.dialogueSystem.ClearDialogue();
            }

            if (AITimer == 60)
            {
                foreach (TelegraphLine telegraphLine in Owner.activeTelegraphs)
                {
                    telegraphLine.RotationalVelocity = 0;
                }
            }

            if (AITimer % ShotgunFrequency == 0 && (AITimer < Owner.BarDuration * 4 || AITimer > Owner.BarDuration * 5) && AITimer > Owner.BarDuration)
            {

                BasicProjectile singleShot = new BasicProjectile();


                Owner.Velocity = 1.1f * Utilities.Normalise(Owner.Position - Main.player.Position);

                singleShot.Spawn(Owner.Position, ProjectileSpeed * Utilities.Normalise(Main.player.Position - Owner.Position), 1f, 1, "box", 1.01f, Vector2.One, Owner, true, Color.Red, true, false);

                for (int i = 1; i < NumberOfProjectiles / 2 + 0.5f; i++) // loop for each pair of projectiles an angle away from the middle
                {
                    BasicProjectile shotgunBlast = new BasicProjectile();
                    BasicProjectile shotgunBlast2 = new BasicProjectile(); //one for each side of middle
                    shotgunBlast.Spawn(Owner.Position, ProjectileSpeed * Utilities.Normalise(Utilities.RotateVectorClockwise(Main.player.Position - Owner.Position, i * angleBetweenProjectiles)),
                        1f, 1, "box", 1.01f, Vector2.One, Owner, true, Color.Red, true, false);
                    shotgunBlast2.Spawn(Owner.Position, ProjectileSpeed * Utilities.Normalise(Utilities.RotateVectorCounterClockwise(Main.player.Position - Owner.Position, i * angleBetweenProjectiles)),
                        1f, 1, "box", 1.01f, Vector2.One, Owner, true, Color.Red, true, false);

                }


            }

            if (AITimer > Owner.BarDuration * 4 && AITimer < Owner.BarDuration * 5)
            {
                Owner.Velocity = MathHelper.Lerp(1f, 5f, Utilities.DistanceBetweenEntities(Main.player, Owner) / 1000f) * Utilities.Normalise(Main.player.Position - Owner.Position);
            }

            HandleBounces();
        }
    }
}
