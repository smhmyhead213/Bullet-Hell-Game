using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.Enemy;
using System;
using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses;

namespace bullethellwhatever.Bosses.TestBoss
{
    public class Charge : BossAttack
    {
        public float ChargeFrequency;
        public float ChargeSpeed;
        public float ChargeProjectileAcceleration;
        public float ChargeProjectileSpeed;
        public bool HasChosenChargeDirection;

        public Charge(int endTime) : base(endTime)
        {

        }

        public override void InitialiseAttackValues()
        {
            ChargeFrequency = Owner.BarDuration * 2;

            switch (GameState.Difficulty)
            {
                case GameState.Difficulties.Easy:
                    ChargeSpeed = 5f;
                    ChargeProjectileAcceleration = 1f;
                    ChargeProjectileSpeed = 6f;
                    break;
                case GameState.Difficulties.Normal:
                    ChargeSpeed = 7f;
                    ChargeProjectileAcceleration = 1.01f;
                    ChargeProjectileSpeed = 9f;
                    break;
                case GameState.Difficulties.Hard:
                    ChargeSpeed = 9f;
                    ChargeProjectileAcceleration = 1.035f;
                    ChargeProjectileSpeed = 13f;
                    break;
                case GameState.Difficulties.Insane:
                    ChargeSpeed = 11f;
                    ChargeProjectileAcceleration = 1.04f;
                    ChargeProjectileSpeed = 15f;
                    break;
            }
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            float moveSpeed;

            if (AITimer == 0)
            {
                Owner.ContactDamage = true; //turn on contact damage
            }

            if (!HasChosenChargeDirection)
            {
                Owner.Velocity = ChargeSpeed * Utilities.Normalise(Main.player.Position - Owner.Position);
                HasChosenChargeDirection = true; //charge
            }

            moveSpeed = ChargeSpeed * (MathF.Cos(AITimer % ChargeFrequency / (ChargeFrequency / 2)) + 0.5f); //the velocity follows a sine curve, so the acceleration follows its derived graph, cos x

            Owner.Velocity = moveSpeed * Utilities.SafeNormalise(Owner.Velocity, Vector2.Zero);


            SpinUpClockwise(ref Owner.Rotation, 20f);

            if (AITimer % ChargeFrequency == 0)
            {
                HasChosenChargeDirection = false; //enable the next charge to start

                int projs = 8;

                for (int i = 0; i < projs; i++)
                {
                    BasicProjectile proj = new BasicProjectile();
                    proj.Spawn(Owner.Position, 9f * Utilities.RotateVectorClockwise(Utilities.SafeNormalise(Vector2.UnitY, Vector2.Zero), MathF.PI * 2 / projs * i),
                        1f, 1, "box", 0, Vector2.One, Owner, true, Color.Red, true, false);
                }

            }

            if (!(AITimer % ChargeFrequency > 0 && AITimer % ChargeFrequency < ChargeFrequency / 2 + 30f) && AITimer % 30 % 2 == 0 && AITimer > 0)// check if aitimer is between 1 and 15 and if its even
            {
                WeakHomingProjectile projectile = new WeakHomingProjectile(ChargeProjectileSpeed, 120);
                projectile.Spawn(Owner.Position, 0.5f * Utilities.Normalise(Main.player.Position - Owner.Position), 1f, 1, "box", ChargeProjectileAcceleration, Vector2.One, Owner, true, Color.Red, true, false);

                //BasicProjectile projectile = new BasicProjectile();
                //projectile.Spawn(Position, 5f * Utilities.Normalise(Main.player.Position - Position), 1f, Texture, chargeProjectileAcceleration, Vector2.One, this, true, Color.Red, true, false);
            }

            HandleBounces();
        }
    }
}
