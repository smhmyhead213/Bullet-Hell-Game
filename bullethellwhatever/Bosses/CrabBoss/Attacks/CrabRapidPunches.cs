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
            int rapidPunchesDuration = 45;
            int rapidPunchPullBackTime = 8;
            int rapidPunchSwingTime = 4;
            int pauseTimeAfterRapidPunch = 2;
            int rapidPunchTime = rapidPunchPullBackTime + rapidPunchSwingTime + pauseTimeAfterRapidPunch;

            if (AITimer >= 0 && AITimer < rapidPunchesDuration)
            {
                int dashDuration = rapidPunchesDuration;
                float initialDashSpeed = 20f;
                // lunge at player
                if (AITimer == 0)
                {
                    Owner.Velocity = 5f * Owner.Position.DirectionToPlayer();
                }

                if (AITimer <= dashDuration)
                {
                    float localDashTime = AITimer;
                    float interpolant = EasingFunctions.easeInExpo(localDashTime / (float)dashDuration);

                    Owner.Velocity = Utilities.SafeNormalise(Owner.Velocity) * MathHelper.Lerp(initialDashSpeed, initialDashSpeed / 5f, interpolant);
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
                    bool windingDown = armTimer >= timeLastPunchEnds;

                    if (!windingDown)
                    {
                        armTimer = armTimer % rapidPunchTime;
                    }
                    else
                    {
                        int resetTimer = armTimer - timeLastPunchEnds;
                        float interpolant = (resetTimer + 1) / (float)timeToSpendWindingDown;
                        //Assert(interpolant < 0.99f); 
                        //Arm(i).LerpArmToRest(interpolant);
                    }

                    if (armTimer >= 0 && !windingDown)
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
                            int projectiles = 3;
                            float spread = PI / 8;
                            Vector2 spawnPos = Arm(i).WristPosition();
                            int onEachSide = projectiles / 2;
                            float initialSpeed = 30f;
                            Projectile p = SpawnProjectile(spawnPos, Arm(i).LowerArm.RotationFromV().ToVector() * initialSpeed, 1f, 1, "box", Vector2.One, Owner, true, false, Color.Red, true, false);
                            p.AddTrail(14);
                            p.Raycast = new BaseClasses.Hitboxes.RaycastData(p.GetVelocity, 1);
                            p.SetExtraAI(new Action(() =>
                            {
                                p.Rotation = p.Velocity.ToAngle();
                                p.ExponentialAccelerate(1.1f);
                            }));

                            for (int j = 1; j < onEachSide + 1; j++)
                            {
                                // this sucks
                                float angle = j / (float)onEachSide * spread / 2;
                                Projectile shotgun = SpawnProjectile(spawnPos, Arm(i).LowerArm.RotationFromV().ToVector().Rotate(angle) * initialSpeed, 1f, 1, "box", Vector2.One, Owner, true, false, Color.Red, true, false);
                                shotgun.AddTrail(14);
                                shotgun.Raycast = new BaseClasses.Hitboxes.RaycastData(shotgun.GetVelocity, 1);
                                shotgun.SetExtraAI(new Action(() =>
                                {
                                    shotgun.Rotation = shotgun.Velocity.ToAngle();
                                    shotgun.ExponentialAccelerate(1.1f);
                                }));

                                Projectile shotgun2 = SpawnProjectile(spawnPos, Arm(i).LowerArm.RotationFromV().ToVector().Rotate(-angle) * initialSpeed, 1f, 1, "box", Vector2.One, Owner, true, false, Color.Red, true, false);
                                shotgun2.AddTrail(14);
                                shotgun2.Raycast = new BaseClasses.Hitboxes.RaycastData(shotgun2.GetVelocity, 1);
                                shotgun2.SetExtraAI(new Action(() =>
                                {
                                    shotgun2.Rotation = shotgun2.Velocity.ToAngle();
                                    shotgun2.ExponentialAccelerate(1.1f);
                                }));
                            }                            
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
            {
                //return new DoNothing(CrabOwner);
                return base.PickNextAttack();
            }
            else return new CrabRapidPunches(CrabOwner, Repetition + 1);
        }
    }
}
