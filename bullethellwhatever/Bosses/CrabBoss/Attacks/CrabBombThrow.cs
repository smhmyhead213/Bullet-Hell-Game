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
    public class CrabBombThrow : CrabBossAttack
    {
        public CrabBombThrow(CrabBoss owner) : base(owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int pullBackArmTime = 45;
            int throwTime = 15;

            int expandedi = Utilities.ExpandedIndex(ChosenArmIndex());

            CrabOwner.FacePlayer();

            Vector2 target = player.Position + new Vector2(0, -500f);

            Owner.Velocity += Utilities.SafeNormalise(target - Owner.Position) * 0.2f;
            int loopedAITImer = AITimer % (pullBackArmTime + throwTime);
           
            if (loopedAITImer == 0)
            {
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

            if (loopedAITImer < pullBackArmTime)
            {
                RotateArm(ChosenArmIndex(), -expandedi * PI / 2, loopedAITImer, pullBackArmTime, EasingFunctions.EaseOutQuad);
            }

            if (loopedAITImer >= pullBackArmTime && loopedAITImer < pullBackArmTime + throwTime)
            {
                int localTime = loopedAITImer - pullBackArmTime;

                RotateArm(ChosenArmIndex(), -expandedi *- PI / 2, localTime, throwTime, EasingFunctions.EaseOutQuad);
            }
        }
    }
}
