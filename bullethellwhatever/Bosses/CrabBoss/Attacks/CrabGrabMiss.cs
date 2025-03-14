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

            int rapidPunchesDuration = 90;
            int rapidPunchPullBackTime = 8;
            int rapidPunchSwingTime = 4;
            int pauseTimeAfterRapidPunch = 5;
            int rapidPunchTime = rapidPunchPullBackTime + rapidPunchSwingTime + pauseTimeAfterRapidPunch;


            int screamEndTime = screamTime + screamDuration + waitTimeAfterScream;

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

            if (AITimer >= screamEndTime && AITimer < screamEndTime + rapidPunchesDuration)
            {
                int armDesync = rapidPunchTime / 2;
                int localTime = AITimer - screamEndTime;

                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);
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

                    if (armTimer >= 0)
                    {
                        //Assert(i == 0);

                        if (armTimer < rapidPunchPullBackTime)
                        {
                            float interpolant = EasingFunctions.Linear(armTimer / (float)rapidPunchPullBackTime);
                            float pullArmInFrac = 0.4f;
                            Vector2 targetPosition = Arm(i).Position + new Vector2(0f, Arm(i).Length()).Rotate(-expandedi * -PI / 18) * pullArmInFrac;
                            Arm(i).LerpToPoint(targetPosition, interpolant);
                        }

                        if (armTimer >= rapidPunchPullBackTime && armTimer <= rapidPunchPullBackTime + rapidPunchSwingTime)
                        {
                            int localSwingThroughTime = armTimer - rapidPunchPullBackTime;
                            float interpolant = EasingFunctions.EaseOutExpo(localSwingThroughTime / (float)rapidPunchSwingTime);
                            float holdOutDistFraction = 0.95f;
                            Vector2 targetPosition = Arm(i).Position + new Vector2(0f, Arm(i).Length()).Rotate(Owner.Rotation) * holdOutDistFraction;
                            Arm(i).LerpToPoint(targetPosition, interpolant);
                        }

                        // spawn explosion at end of punch

                        if (armTimer == rapidPunchPullBackTime + rapidPunchSwingTime)
                        {                
                            string textureName = "Circle";
                            Texture2D texture = AssetRegistry.GetTexture2D(textureName);
                            float radius = 150f;
                            float explosionScale = radius / texture.Width;
                            int explosionLifetime = 12;
                            Vector2 spawnPosition = Arm(i).PositionAtDistanceFromWrist(20f);
                            Projectile p = SpawnProjectile(spawnPosition, Vector2.Zero, 1f, 1, textureName, Vector2.One * explosionScale, Owner, true, false, Color.Red, false, false);
                            
                            int locali = i;

                            p.SetExtraAI(new Action(() =>
                            {
                                p.Position = Arm(locali).PositionAtDistanceFromWrist(20f);

                                if (p.AITimer == explosionLifetime)
                                {
                                    p.InstantlyDie();
                                }
                            }));
                        }

                        //if (armTimer >= rapidPunchPullBackTime + rapidPunchSwingTime && armTimer < rapidPunchTime)
                        //{

                        //}
                    }
                }
            }
        }
    }
}
