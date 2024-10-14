using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using System.Windows.Forms.Design;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabBombThrow : CrabBossAttack
    {
        public CrabBombThrow(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int pullBackArmTime = 45;
            int throwTime = 15;

            float pullBackArmAngle = PI / 2;
            float throwAngle = 2 * PI / 3;
            int expandedi = Utilities.ExpandedIndex(ChosenArmIndex());
            float jumpBackSpeed = 10f;

            CrabOwner.FacePlayer();

            if (AITimer == 0)
            {
                Owner.Velocity = Utilities.SafeNormalise(Owner.Position - player.Position) * jumpBackSpeed;

                Projectile bomb = SpawnProjectile(ChosenArm().PositionAtDistanceFromWrist(20), Vector2.Zero, 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                
                bomb.SetExtraAI(new Action(() =>
                {
                    int releaseTime = pullBackArmTime + throwTime + 3;
                    if (bomb.AITimer < releaseTime)
                        bomb.Position = ChosenArm().PositionAtDistanceFromWrist(20);
                    else if (bomb.AITimer == releaseTime)
                    {
                        bomb.Velocity = Utilities.SafeNormalise(player.Position - bomb.Position) * 10f;
                    }
                }));

                bomb.SetEdgeTouchEffect(new Action(() =>
                {
                    int numberOfProjectiles = 26;

                    for (int i = 0; i < numberOfProjectiles; i++)
                    {
                        Projectile p = SpawnProjectile(bomb.Position, 0.1f * Utilities.AngleToVector(Tau / numberOfProjectiles * i), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);

                        p.AddTrail(22);

                        p.SetExtraAI(new Action(() =>
                        {
                            p.Velocity += 0.2f * Utilities.SafeNormalise(p.Velocity);
                        }));

                    }

                    bomb.InstantlyDie();
                }));
            }

            if (AITimer < pullBackArmTime)
            {
                RotateArm(ChosenArmIndex(), -expandedi * pullBackArmAngle, AITimer, pullBackArmTime, EasingFunctions.EaseOutQuad);

                Owner.Velocity = Owner.Velocity * 0.965f;
            }

            if (AITimer >= pullBackArmTime && AITimer < pullBackArmTime + throwTime)
            {
                int localTime = AITimer - pullBackArmTime;

                RotateArm(ChosenArmIndex(), -expandedi * -throwAngle, localTime, throwTime, EasingFunctions.EaseOutQuad);
            }

            if (AITimer == pullBackArmTime + throwTime)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new BombThrowToNeutralTransition(CrabOwner);
        }
    }

    public class BombThrowToNeutralTransition : CrabBossAttack
    {
        public BombThrowToNeutralTransition(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int duration = 30;
            int expandedi = Utilities.ExpandedIndex(ChosenArmIndex());
            float pullBackArmAngle = PI / 2;
            float throwAngle = 2 * PI / 3;

            float difference = throwAngle - pullBackArmAngle;

            if (AITimer < duration)
            {
                RotateArm(ChosenArmIndex(), -expandedi * difference, AITimer, duration, EasingFunctions.EaseOutQuad);
            }
            if (AITimer == duration)
            {
                End();
            }
        }
        public override BossAttack PickNextAttack()
        {
            return new CrabPunch(CrabOwner);
        }
    }
}
