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
            ProjectlesPerRing = 20;
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

                if (ChargeWindupTimer > TimeToWindUpCharge - TelegraphTime) //tele line
                {
                    CrabOwner.SetBoosters(false);
                    TelegraphLine t = new TelegraphLine(Utilities.RotationTowards(CrabOwner.Position, player.Position), 0, 0, CrabOwner.Texture.Width * CrabOwner.Size.X, 2000, 1, CrabOwner.Position, Color.White, "box", CrabOwner, true);
                    t.ChangeShader("OutlineTelegraphShader");
                    CrabOwner.FacePlayer();
                }
            }

            if (ChargeWindupTimer == TimeToWindUpCharge)
            {
                IsCharging = true;
                ChargeWindupTimer++;
                CrabOwner.SetBoosters(true);
                CrabOwner.Velocity = 40f * Utilities.SafeNormalise(player.Position - CrabOwner.Position);
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
        }
    }
}
