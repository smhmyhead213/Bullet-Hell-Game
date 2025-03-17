using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.Projectiles;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabRapidPunches : CrabBossAttack
    {
        public int Repetition;
        public CrabRapidPunches(CrabBoss owner, int repetitions) : base(owner)
        {
            Repetition = repetitions;
        }
        public override void Execute(int AITimer)
        {
            int rapidPunchesDuration = 90;
            int rapidPunchPullBackTime = 8;
            int rapidPunchSwingTime = 4;
            int pauseTimeAfterRapidPunch = 5;
            int rapidPunchTime = rapidPunchPullBackTime + rapidPunchSwingTime + pauseTimeAfterRapidPunch;

            if (AITimer >= 0 && AITimer < rapidPunchesDuration)
            {
                int dashDuration = rapidPunchesDuration;
                float initialDashSpeed = 25f;
                // lunge at player
                if (AITimer == 0)
                {
                    Owner.Velocity = 5f * Owner.Position.DirectionToPlayer();
                }

                if (AITimer > 0 && AITimer <= 0 + dashDuration)
                {
                    float localDashTime = AITimer;
                    float interpolant = EasingFunctions.Linear(localDashTime / (float)dashDuration);

                    Owner.Velocity = Utilities.SafeNormalise(Owner.Velocity) * MathHelper.Lerp(initialDashSpeed, 0f, interpolant);
                    Owner.Rotation = Owner.Velocity.ToAngle() + PI;
                }

                int armDesync = rapidPunchTime / 2;
                int localTime = AITimer;

                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);
                    int lagBehind = i == 0 ? 0 : -armDesync;
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
                        float interpolant = (resetTimer + 1) / (float)timeToSpendWindingDown;
                        //Assert(i == 1); 
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
                            p.SetShader("FirePunchShader").SetNoiseMap("FireNoise2", 0f);

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
                    }
                }
            }

            if (AITimer == rapidPunchesDuration)
            {
                End();
            }
        }

        public override BossAttack PickNextAttack()
        {
            if (Repetition == 3)
                return base.PickNextAttack();
            else return new CrabRapidPunches(CrabOwner, Repetition + 1);
        }
    }
}
