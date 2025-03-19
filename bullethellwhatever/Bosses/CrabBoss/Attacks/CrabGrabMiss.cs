using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Bosses;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework;
using bullethellwhatever.BaseClasses;
using System.Diagnostics;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabGrabMiss : CrabBossAttack
    {
        public CrabGrabMiss(CrabBoss owner) : base (owner)
        {

        }

        public override void Execute(int AITimer)
        {
            int screamTime = 20;
            int screamDuration = 60;
            int waitTimeAfterScream = 20;

            int screamEndTime = screamTime + screamDuration + waitTimeAfterScream;

            if (AITimer == 0)
            {
                Owner.ContactDamage = true;
            }

            if (AITimer >= screamTime && AITimer < screamTime + screamDuration)
            {
                int screamPeriod = 8;

                if (AITimer % screamPeriod == 0)
                {
                    Drawing.ScreenShake(7, screamPeriod);
                    ShockwaveRing shockwave = new ShockwaveRing(0f, 120f, 4, 2);
                    shockwave.ScrollSpeed = 0.04f;
                    shockwave.Spawn(Owner.Position + new Vector2(0f, 20f), Owner, Color.Gray);
                }
            }

            if (AITimer >= screamTime + screamDuration && AITimer < screamEndTime)
            {
                int localTime = AITimer - (screamTime + screamDuration);
                float progress = localTime / (float)waitTimeAfterScream;
                float interpolant = progress;
                float finalArmScale = 1.2f; // size to have arms at for punches

                foreach (CrabArm arm in CrabOwner.Arms)
                {
                    arm.LerpArmToRest(interpolant);
                    float scale = MathHelper.Lerp(arm.UpperArm.Scale.X, finalArmScale, interpolant);
                    arm.SetScale(scale);
                }
            }

            if (AITimer == screamEndTime)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            return new CrabRapidPunches(CrabOwner, 1);
        }
    }
}
