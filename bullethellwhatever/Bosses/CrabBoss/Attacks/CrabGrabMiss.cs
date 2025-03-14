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
            int screamDuration = 60;
            int waitTimeAfterScream = 20;

            int rapidPunchesDuration = 90;
            int rapidPunchPullBackTime = 4;
            int rapidPunchSwingTime = 6;
            int rapidPunchTime = rapidPunchPullBackTime + rapidPunchSwingTime;


            int screamEndTime = screamTime + screamDuration + waitTimeAfterScream;

            if (AITimer >= screamTime && AITimer < screamTime + screamDuration)
            {
                int screamPeriod = 8;

                if (AITimer % screamPeriod == 0)
                {
                    Drawing.ScreenShake(7, screamPeriod);
                    ShockwaveRing shockwave = new ShockwaveRing(0f, 130f, 4, 2);
                    shockwave.ScrollSpeed = 0.04f;
                    shockwave.Spawn(Owner.Position + new Vector2(0f, 20f), Owner, Color.Gray);
                }
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
                int armDesync = rapidPunchTime / 2;
                int localTime = AITimer - screamEndTime;

                for (int i = 0; i < 2; i++)
                {
                    int lagBehind = i == 0 ? 0 : - armDesync;
                    int armTimer = localTime + lagBehind;

                    //int punchStopTime = attackDuration - totalPunchTime; // time after which we should stop punching

                    // calculate whether or not we are in the "wind down" portion of this arms movement
                    int availablePunchTime = rapidPunchesDuration + lagBehind;
                    int punchesAvailable = availablePunchTime / rapidPunchTime;
                    int timeLastPunchEnds = punchesAvailable * rapidPunchTime; // -armInitialTimes[i] + 

                    // figure out how long we have until the attack fully ends, if we dont have time
                    int timeToSpendWindingDown = rapidPunchesDuration - timeLastPunchEnds; //+ armInitialTimes[i];
                                                                                     // dont start another punch if we dont have time
                    if (armTimer < timeLastPunchEnds)
                    {
                        armTimer = armTimer % rapidPunchTime;
                    }
                    else
                    {
                        int resetTimer = armTimer - timeLastPunchEnds;
                        float interpolant = resetTimer / (float)timeToSpendWindingDown;
                        //Debug.Assert(i == 0);
                        Arm(i).LerpArmToRest(interpolant);
                    }

                    if (armTimer < rapidPunchPullBackTime)
                    {
                        float interpolant = armTimer / (float)rapidPunchPullBackTime;
                        Vector2 targetPosition = Arm(i).Position + (Arm(i).RestPositionEnd() - Arm(i).Position) * 0.6f;
                        Arm(i).LerpToPoint(targetPosition, interpolant);
                    }

                    if (armTimer >= rapidPunchPullBackTime && AITimer < rapidPunchPullBackTime + rapidPunchSwingTime)
                    {
                        int localSwingThroughTime = armTimer - rapidPunchPullBackTime;
                        float interpolant = localSwingThroughTime / (float)rapidPunchSwingTime;
                        Arm(i).LerpToRestPosition(interpolant);
                    }
                }
            }
        }
    }
}
