using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Bosses;
using bullethellwhatever.DrawCode;
using Microsoft.Xna.Framework;
using bullethellwhatever.BaseClasses;

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
            int screamDuration = 30;
            int waitTimeAfterScream = 20;

            int rapidPunchesDuration = 40;
            int rapidPunchTime = 6;


            int screamEndTime = screamTime + screamDuration + waitTimeAfterScream;

            if (AITimer == screamTime)
            {
                ShockwaveRing shockwave = new ShockwaveRing(200f, 0, 20000, 30);
                shockwave.ScrollSpeed = 0.04f;
                shockwave.Spawn(Owner.Position, Owner, Color.White);
            }

            if (AITimer >= screamTime + screamDuration && AITimer < screamEndTime)
            {
                int localTime = AITimer - (screamTime + screamDuration);
                float progress = localTime / (float)waitTimeAfterScream;
                float interpolant = progress;

                foreach (CrabArm arm in CrabOwner.Arms)
                {
                    arm.LerpArmToRest(interpolant);
                    float scale = MathHelper.Lerp(arm.UpperArm.Scale.X, 1f, interpolant);
                    arm.SetScale(scale);
                }
            }

            if (AITimer >= screamEndTime && AITimer < screamEndTime + rapidPunchesDuration)
            {
                int armZeroTimer =
            }
        }
    }
}
