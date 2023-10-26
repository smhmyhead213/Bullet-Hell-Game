using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class CrabCharge : CrabBossAttack
    {
        //public Vector2 PositionToMoveTo;
        public bool IsCharging;
        public int ChargeWindupTimer;
        public int TimeToWindUpCharge;
        public int TelegraphTime;
        public int AdjustTimeAfterCharge;
        public int ProjectileBurstCooldown;
        public int ProjectlesPerRing;
        public int TimeSpentCharging;
        public Vector2 DirectionToChargeAt;
        public CrabCharge(int endTime) : base(endTime)
        {
            EndTime = endTime;            
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            IsCharging = false;
            ChargeWindupTimer = 0;
            TimeToWindUpCharge = 100;
            TelegraphTime = 50;
            AdjustTimeAfterCharge = 60;
            ProjectlesPerRing = Utilities.ValueFromDifficulty(12, 16, 20, 28);
            TimeSpentCharging = 0;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            if (ProjectileBurstCooldown > 0)
            {
                ProjectileBurstCooldown--;
            }

            if (ChargeWindupTimer < 0)
            {
                //CrabOwner.Position = Vector2.Lerp(CrabOwner.Position, Utilities.CentreOfScreen(), 0.01f);
                CrabOwner.Velocity = CrabOwner.Velocity * 0.95f; // stop ZOOMING
                CrabOwner.FacePlayer();
                HandleBounces();
                ChargeWindupTimer++;
            }
            if (!IsCharging && ChargeWindupTimer >= 0) // time < 0 is moving into map again
            {
                ChargeWindupTimer++;

                if (ChargeWindupTimer == TimeToWindUpCharge - TelegraphTime) // on the first frame of telegraphing the charge
                {
                    Owner.Velocity = Owner.Velocity * -1f; // reverse residual movement to wind up charge
                }

                if (ChargeWindupTimer > TimeToWindUpCharge - TelegraphTime) //tele line
                {
                    CrabOwner.SetBoosters(false);

                    Owner.Velocity = Owner.Velocity * 0.99f; // dont spend the whole wind up moving back uniformly, slow down to show that its getting ready

                    TelegraphLine t = new TelegraphLine(Utilities.RotationTowards(CrabOwner.Position, player.Position), 0, 0, CrabOwner.Texture.Width * CrabOwner.GetSize().X, 2000, 1, CrabOwner.Position, Color.White, "box", CrabOwner, true);
                    t.ChangeShader("OutlineTelegraphShader");
                    CrabOwner.FacePlayer();
                }
            }

            int chargeAccelTime = 25;
            float angleToOpenArmsBy = PI / 2f;

            if (ChargeWindupTimer >= TimeToWindUpCharge && ChargeWindupTimer < TimeToWindUpCharge + chargeAccelTime)
            {
                if (ChargeWindupTimer == TimeToWindUpCharge)
                {
                    DirectionToChargeAt = Utilities.SafeNormalise(player.Position - CrabOwner.Position);
                }

                IsCharging = true;
                ChargeWindupTimer++;
                CrabOwner.SetBoosters(true);

                float chargeSpeed = Utilities.ValueFromDifficulty(25f, 34f, 40f, 55f);

                Accelerate(chargeSpeed * DirectionToChargeAt, chargeAccelTime);
             
                Leg(0).UpperArm.Rotate(angleToOpenArmsBy / chargeAccelTime);
                Leg(0).LowerArm.Rotate(angleToOpenArmsBy / chargeAccelTime);
                Leg(1).UpperArm.Rotate(-angleToOpenArmsBy / chargeAccelTime);
                Leg(1).LowerArm.Rotate(-angleToOpenArmsBy / chargeAccelTime);

            }

            if (IsCharging)
            {
                TimeSpentCharging++;
            }

            if (IsCharging && Entity.touchingAnEdge(CrabOwner) && TimeSpentCharging > 30) // fixes instantly stop charging if its started while out of bounds
            {
                IsCharging = false;
                TimeSpentCharging = 0;
                HandleBounces();
                ChargeWindupTimer = -AdjustTimeAfterCharge;

                for (int i = 0; i < ProjectlesPerRing; i++)
                {
                    Projectile fast = new Projectile();
                    Projectile med = new Projectile();
                    Projectile slow = new Projectile();

                    float angle = 2f * PI / ProjectlesPerRing;

                    fast.Spawn(CrabOwner.Position, 3f * Utilities.AngleToVector(i * angle), 1f, 1, "box", 1.03f, Vector2.One, Owner, true, Color.Red, true, false); //start slow and speed up fast
                    med.Spawn(CrabOwner.Position, 7f * Utilities.AngleToVector(i * angle), 1f, 1, "box", 1.01f, Vector2.One, Owner, true, Color.Red, true, false);
                    slow.Spawn(CrabOwner.Position, 4f * Utilities.AngleToVector(i * angle), 1f, 1, "box", 1.005f, Vector2.One, Owner, true, Color.Red, true, false);
                }
            }

            float oneOverFractionOfRecoveryTimeToCloseArms = 2f;

            if (ChargeWindupTimer < -(AdjustTimeAfterCharge / oneOverFractionOfRecoveryTimeToCloseArms)) // if we are recovering from a charge
            {
                Leg(0).UpperArm.Rotate(-angleToOpenArmsBy / AdjustTimeAfterCharge * oneOverFractionOfRecoveryTimeToCloseArms);
                Leg(0).LowerArm.Rotate(-angleToOpenArmsBy / AdjustTimeAfterCharge * oneOverFractionOfRecoveryTimeToCloseArms);
                Leg(1).UpperArm.Rotate(angleToOpenArmsBy / AdjustTimeAfterCharge * oneOverFractionOfRecoveryTimeToCloseArms);
                Leg(1).LowerArm.Rotate(angleToOpenArmsBy / AdjustTimeAfterCharge * oneOverFractionOfRecoveryTimeToCloseArms);
            }
        }
    }
}
