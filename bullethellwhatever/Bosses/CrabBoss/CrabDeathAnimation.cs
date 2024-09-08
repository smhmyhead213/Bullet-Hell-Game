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
        public CrabDeathAnimation() : base()
        {

        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer;

            int fallApartTime = 45;

            if (time == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).ContactDamage(false);

                    Leg(i).Velocity = (player.Position - Leg(i).Position) / fallApartTime;

                    Leg(i).PointLegInDirection(Utilities.VectorToAngle(player.Position - Leg(i).Position));
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
                    Leg(i).Velocity = Leg(i).Velocity * 0.99f;
                    Leg(i).PointLegInDirection(Utilities.VectorToAngle(player.Position - Leg(i).Position));
                }

                Owner.Velocity = Owner.Velocity * 0.98f;

                CrabOwner.FacePlayer();
            }
            if (time == fallApartTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).DeathAnimation = true;
                    Leg(i).UpperArm.Velocity = Leg(i).Velocity * 0.9f;
                    Leg(i).LowerArm.Velocity = Leg(i).Velocity;
                    Leg(i).UpperClaw.Velocity = Leg(i).Velocity * 1.1f;
                    Leg(i).LowerClaw.Velocity = Leg(i).Velocity * 1.2f;
                }

                Owner.RotationalVelocity = Sign(Owner.Velocity.X) * CrabOwner.SpinVelOnDeath;
            }

            if (time > fallApartTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Leg(i).UpperArm.Velocity = Leg(i).UpperArm.Velocity + new Vector2(0, Leg(i).UpperArm.Gravity);
                    Leg(i).UpperArm.HandleBounces();
                    Leg(i).UpperArm.Velocity.X = Leg(i).UpperArm.Velocity.X * 0.99f;

                    Leg(i).LowerArm.Velocity = Leg(i).LowerArm.Velocity + new Vector2(0, Leg(i).LowerArm.Gravity);
                    Leg(i).LowerArm.HandleBounces();
                    Leg(i).LowerClaw.Velocity.X = Leg(i).LowerClaw.Velocity.X * 0.99f;


                    Leg(i).UpperClaw.Velocity = Leg(i).UpperClaw.Velocity + new Vector2(0, Leg(i).UpperClaw.Gravity);
                    Leg(i).UpperClaw.HandleBounces();
                    Leg(i).UpperClaw.Velocity.X = Leg(i).UpperClaw.Velocity.X * 0.99f;

                    Leg(i).LowerClaw.Velocity = Leg(i).LowerClaw.Velocity + new Vector2(0, Leg(i).LowerClaw.Gravity);
                    Leg(i).LowerClaw.HandleBounces();
                    Leg(i).LowerClaw.Velocity.X = Leg(i).LowerClaw.Velocity.X * 0.99f;
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
                    Leg(i).UpperArm.Die();
                    Leg(i).LowerArm.Die();
                    Leg(i).UpperClaw.Die();
                    Leg(i).LowerClaw.Die();
                }
            }

            if (Owner.AITimer == 120)
            {
                UI.CreateAfterBossMenu();
            }
        }
    }
}
