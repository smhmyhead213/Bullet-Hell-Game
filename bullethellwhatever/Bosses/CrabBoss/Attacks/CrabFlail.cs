using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class NeutralToCrabFlailChargeTransition : CrabBossAttack
    {
        public NeutralToCrabFlailChargeTransition(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int spinUpTime = 20;
            float spinAngularAccel = PI / 180;
            ref float angularVelocity = ref Owner.ExtraData[1]; // index 0 is reserved 
            float holdOutArmsAngle = PI / 2;

            if (AITimer < spinUpTime)
            {
                angularVelocity += spinAngularAccel;

                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);
                    
                    RotateArm(i, -expandedi * holdOutArmsAngle, AITimer, spinUpTime, EasingFunctions.EaseInOutCirc);
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
                Projectile p = SpawnProjectile<Projectile>(Owner.Position, projSpeed * Utilities.AngleToVector(projAngle), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
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

            if (AITimer > accelerateTime + chargeTime && AITimer <= accelerateTime + chargeTime + slowDownTime)
            {
                int localTime = AITimer - (accelerateTime + chargeTime);
                float slowDownScale = 0.7f; // control how close to a stop the boss gets
                float interpolant = 1 - slowDownScale * EasingFunctions.EaseOutExpo(localTime / (float)slowDownTime);

                Owner.Velocity = maxChargeSpeed * interpolant * Utilities.SafeNormalise(Owner.Velocity);

                float projAngle = Utilities.RandomAngle();                

                // spawn projectiles when spinning down
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
            int decelTime = 30;
            
            ref float angularVelocity = ref Owner.ExtraData[1]; // index 0 is reserved
            float holdOutArmsAngle = PI / 2;

            // subtract pi from the boss rotation since the boss faces towards its rotation
            float rotationToPlayer = Utilities.SmallestAngleTo(Owner.Rotation + PI, Utilities.AngleToPlayerFrom(Owner.Position));

            angularVelocity = 0.2f * rotationToPlayer;

            Owner.Rotation += angularVelocity;

            if (AITimer == decelTime)
            {
                CrabOwner.ResetArmRotations();
                End();
            }

            for (int i = 0; i < 2; i++)
            {
                int expandedi = Utilities.ExpandedIndex(i);

                RotateArm(i, expandedi * holdOutArmsAngle, AITimer, decelTime, EasingFunctions.EaseOutBack);
            }
        }

        public override BossAttack PickNextAttack()
        {
            int rng = Utilities.RandomInt(1, 2);

            return rng switch
            {
                1 => new CrabPunch(CrabOwner),
                2 => new CrabBombThrow(CrabOwner),
                // idk how this would get reached but whatever
                _ => new CrabPunch(CrabOwner),
            };
        }
    }
}
