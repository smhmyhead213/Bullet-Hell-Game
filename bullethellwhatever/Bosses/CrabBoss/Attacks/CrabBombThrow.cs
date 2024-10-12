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

            if (AITimer == 0)
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
                    Projectile p = new Projectile(bomb.Position, Vector2.Zero, 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                    RadialProjectileBurst(p, 60, 0, 10f);
                    bomb.InstantlyDie();
                }));
            }

            if (AITimer < pullBackArmTime)
            {
                RotateArm(ChosenArmIndex(), -expandedi * PI / 2, AITimer, pullBackArmTime, EasingFunctions.EaseOutQuad);
            }

            if (AITimer >= pullBackArmTime && AITimer < pullBackArmTime + throwTime)
            {
                int localTime = AITimer - pullBackArmTime;

                RotateArm(ChosenArmIndex(), -expandedi *- PI / 2, localTime, throwTime, EasingFunctions.EaseOutQuad);
            }
        }
    }
}
