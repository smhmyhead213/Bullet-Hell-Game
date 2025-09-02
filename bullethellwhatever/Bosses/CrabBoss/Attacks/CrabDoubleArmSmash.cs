using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles;

using bullethellwhatever.Projectiles.TelegraphLines;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabDoubleArmSmash : CrabBossAttack
    {
        public Vector2 SlamTargetPosition;
        public CrabDoubleArmSmash(CrabBoss owner) : base(owner)
        {
            SlamTargetPosition = Vector2.Zero;
        }
        public override void Execute(int AITimer)
        {
            int preparationTime = 120;

            for (int i = 0; i < 2; i++)
            {
                if (AITimer <= preparationTime)
                {
                    // make arms longer so they actually reach

                    float distanceToPlayer = Utilities.DistanceBetweenVectors(Owner.Position, player.Position);
                    float spaceOnEachSideOfPlayer = distanceToPlayer * 0.67f;

                    // become longer so arms bend to look like crushing the player
                    float additionalScale = 1.6f;
                    float scaleFactor = distanceToPlayer / Arm(i).OriginalLength() * additionalScale;

                    float interpolant = (float)AITimer / preparationTime;
                    int expandedi = Utilities.ExpandedIndex(i);
                    Vector2 targetPosition = player.Position + expandedi * spaceOnEachSideOfPlayer * (PI / 2).ToVector();

                    BoxDrawer.DrawBox(targetPosition);

                    Arm(i).LerpToPoint(targetPosition, interpolant);
                    Arm(i).SetScale(MathHelper.Lerp(Arm(i).Scale(), scaleFactor, interpolant));
                }
            }
        }
    }
}
