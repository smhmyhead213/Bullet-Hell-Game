using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

 
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using bullethellwhatever.MainFiles;
using bullethellwhatever.DrawCode.UI;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabDeathAnimation : CrabBossAttack
    {
        public CrabDeathAnimation(CrabBoss owner) : base(owner)
        {

        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(int AITimer)
        {
            int time = AITimer;

            int fallApartTime = 45;

            if (time == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Arm(i).ContactDamage(false);

                    Arm(i).Velocity = (player.Position - Arm(i).Position) / fallApartTime;

                    Arm(i).PointLegInDirection(Utilities.VectorToAngle(player.Position - Arm(i).Position));
                }

                Owner.DealDamage = false;

                Owner.activeTelegraphs.Clear();

                for (int i = 0; i < EntityManager.activeProjectiles.Count; i++)
                {
                    EntityManager.activeProjectiles[i].Die();
                }

                Owner.Velocity = (player.Position - Owner.Position) / fallApartTime;
            }

            if (time < fallApartTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Arm(i).Velocity = Arm(i).Velocity * 0.99f;
                    Arm(i).PointLegInDirection(Utilities.VectorToAngle(player.Position - Arm(i).Position));
                }

                Owner.Velocity = Owner.Velocity * 0.98f;

                CrabOwner.FacePlayer();
            }
            if (time == fallApartTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Arm(i).DeathAnimation = true;
                    Arm(i).UpperArm.Velocity = Arm(i).Velocity * 0.9f;
                    Arm(i).LowerArm.Velocity = Arm(i).Velocity;
                    Arm(i).UpperClaw.Velocity = Arm(i).Velocity * 1.1f;
                    Arm(i).LowerClaw.Velocity = Arm(i).Velocity * 1.2f;
                }

                Owner.RotationalVelocity = Sign(Owner.Velocity.X) * CrabOwner.SpinVelOnDeath;
            }

            if (time > fallApartTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Arm(i).UpperArm.Velocity = Arm(i).UpperArm.Velocity + new Vector2(0, Arm(i).UpperArm.Gravity);
                    Arm(i).UpperArm.HandleBounces();
                    Arm(i).UpperArm.Velocity.X = Arm(i).UpperArm.Velocity.X * 0.99f;

                    Arm(i).LowerArm.Velocity = Arm(i).LowerArm.Velocity + new Vector2(0, Arm(i).LowerArm.Gravity);
                    Arm(i).LowerArm.HandleBounces();
                    Arm(i).LowerClaw.Velocity.X = Arm(i).LowerClaw.Velocity.X * 0.99f;


                    Arm(i).UpperClaw.Velocity = Arm(i).UpperClaw.Velocity + new Vector2(0, Arm(i).UpperClaw.Gravity);
                    Arm(i).UpperClaw.HandleBounces();
                    Arm(i).UpperClaw.Velocity.X = Arm(i).UpperClaw.Velocity.X * 0.99f;

                    Arm(i).LowerClaw.Velocity = Arm(i).LowerClaw.Velocity + new Vector2(0, Arm(i).LowerClaw.Gravity);
                    Arm(i).LowerClaw.HandleBounces();
                    Arm(i).LowerClaw.Velocity.X = Arm(i).LowerClaw.Velocity.X * 0.99f;
                }

                Owner.Velocity = Owner.Velocity + new Vector2(0, 0.7f);
                Owner.Velocity = Owner.Velocity * 0.99f;
                Owner.RotationalVelocity = Owner.RotationalVelocity * 0.99f;
                HandleBounces();
            }

            int endTime = 200;

            if (time == endTime - 1)
            {
                Owner.CanDie = true;
                Owner.Die();

                for (int i = 0; i < 2; i++)
                {
                    Arm(i).UpperArm.Die();
                    Arm(i).LowerArm.Die();
                    Arm(i).UpperClaw.Die();
                    Arm(i).LowerClaw.Die();
                }
            }

            if (Owner.AITimer == 120)
            {
                UI.CreateAfterBossMenu();
            }
        }
    }
}
