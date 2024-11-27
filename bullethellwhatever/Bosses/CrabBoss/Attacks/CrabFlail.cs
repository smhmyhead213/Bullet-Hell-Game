using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class NeutralToCrabFlailChargeTransition : CrabBossAttack
    {
        public NeutralToCrabFlailChargeTransition(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int spinUpTime = 60;
            float spinAngularAccel = PI / 540;
            ref float angularVelocity = ref Owner.ExtraData[1]; // index 0 is reserved 

            if (AITimer < spinUpTime)
            {
                angularVelocity += spinAngularAccel;

                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);
                    float holdOutArmsAngle = PI / 2;
                    RotateArm(i, -expandedi * holdOutArmsAngle, AITimer, spinUpTime, EasingFunctions.EaseInQuint);
                }

                // move away from player
                float interpolant = 1f - EasingFunctions.EaseOutExpo(AITimer / (float)spinUpTime);
                float moveBackSpeed = 30f;
                Owner.Velocity = interpolant * moveBackSpeed * Utilities.SafeNormalise(Owner.Position - player.Position);
            }

            Owner.Rotation += angularVelocity;

            if (AITimer == spinUpTime)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabFlail(CrabOwner, 0);
        }
    }
    public class CrabFlail : CrabBossAttack
    {
        public int Repetitions;
        public CrabFlail(CrabBoss owner, int repetitions) : base(owner)
        {
            Repetitions = repetitions;
        }

        public override void Execute(int AITimer)
        {            
            int accelerateTime = 10;
            int slowDownTime = 30;
            int chargeTime = 20;

            ref float angularVelocity = ref Owner.ExtraData[1]; // index 0 is reserved 
            Owner.Rotation += angularVelocity;

            // to do: make camera pan logartihmically to boss
            //MainCamera.Position = Owner.Position;

            float maxChargeSpeed = 30f;

            if (AITimer == 0)
            {
                Owner.Velocity = Utilities.SafeNormalise(player.Position - Owner.Position) * 0.01f; // set direction of movement but dont start moving
            }

            if (AITimer > 0 && AITimer <= accelerateTime)
            {
                int localTime = AITimer;
                float interpolant = EasingFunctions.EaseOutExpo(localTime / (float)accelerateTime);
                
                Owner.Velocity = maxChargeSpeed * interpolant * Utilities.SafeNormalise(Owner.Velocity);
            }

            float projSpeed = 10f;

            if (AITimer > accelerateTime && AITimer <= accelerateTime + chargeTime)
            {
                float projAngle = Utilities.RandomAngle();
                Projectile p = SpawnProjectile(Owner.Position, projSpeed * Utilities.AngleToVector(projAngle), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                p.AddTrail(22);
                p.Rotation = projAngle;

                p.SetExtraAI(new Action(() =>
                {
                    if (p.AITimer > 120)
                    {
                        p.Velocity *= 1.01f;
                    }
                }));
            }

            // revisit movement formula and add projectile barfing
            if (AITimer > accelerateTime + chargeTime && AITimer <= accelerateTime + chargeTime + slowDownTime)
            {
                int localTime = AITimer - (accelerateTime + chargeTime);
                float interpolant = 1 - EasingFunctions.EaseOutExpo(localTime / (float)slowDownTime);

                Owner.Velocity = maxChargeSpeed * interpolant * Utilities.SafeNormalise(Owner.Velocity);

                float projAngle = Utilities.RandomAngle();                

                //Projectile p = SpawnProjectile(Owner.Position, projSpeed * Utilities.AngleToVector(projAngle), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                //p.AddTrail(22);
                //p.Rotation = projAngle;
            }

            int waitTimeBeforeAttackEnd = 10;

            if (AITimer == accelerateTime + chargeTime + slowDownTime + waitTimeBeforeAttackEnd)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            int rng = Utilities.RandomInt(5, 10);

            if (rng < 10 - Repetitions)
            {
                return new CrabFlail(CrabOwner, Repetitions + 1);
            }
            else
            {
                return new CrabFlailToNeutralTransition(CrabOwner);
            }
        }
    }

    public class CrabFlailToNeutralTransition : CrabBossAttack
    {
        public CrabFlailToNeutralTransition(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int decelTime = 120000;
            
            ref float angularVelocity = ref Owner.ExtraData[1]; // index 0 is reserved

            // subtract pi from the boss rotation since the boss faces towards its rotation
            float rotationToPlayer = Utilities.SmallestAngleBetween(Utilities.BringAngleIntoRange(Owner.Rotation + PI), Utilities.AngleToPlayerFrom(Owner.Position));

            angularVelocity = 0.2f * rotationToPlayer;

            Owner.Rotation += angularVelocity;

            if (AITimer == decelTime)
            {
                CrabOwner.ResetArmRotations();
                End();
            }

        }

        public override BossAttack PickNextAttack()
        {
            return new NeutralToCrabFlailChargeTransition(CrabOwner);
        }
    }
}
