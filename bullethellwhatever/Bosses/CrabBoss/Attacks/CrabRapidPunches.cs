using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.DrawCode;
using bullethellwhatever.DrawCode.Particles;
using bullethellwhatever.Projectiles;
using bullethellwhatever.UtilitySystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabRapidPunches : CrabBossAttack
    {
        public int Repetition;
        public bool FullTurnAroundAtEnd;
        public CrabRapidPunches(CrabBoss owner, int repetitions) : base(owner)
        {
            Repetition = repetitions;
            FullTurnAroundAtEnd = Utilities.RandomBool();
        }
        public override void Execute(int AITimer)
        {
            int rapidPunchesDuration = 45;
            int rapidPunchPullBackTime = 8;
            int rapidPunchSwingTime = 4;
            int pauseTimeAfterRapidPunch = 2;
            int rapidPunchTime = rapidPunchPullBackTime + rapidPunchSwingTime + pauseTimeAfterRapidPunch;
            int restTime = 30;
            float initialDashSpeed = 35f;

            if (AITimer >= 0 && AITimer <= rapidPunchesDuration)
            {
                int dashDuration = rapidPunchesDuration;
                float negligibleSpeed = 0.01f; // make sure the speed is never zero but is basically zero
                if (AITimer == 0)
                {
                    Owner.Velocity = negligibleSpeed * Owner.Position.DirectionToPlayer(); // set direction but dont move yet
                }

                if (AITimer <= dashDuration)
                {
                    float localDashTime = AITimer;

                    float interpolant = EasingFunctions.EaseOutExpo(localDashTime / (float)dashDuration);
                    float trackingStrength = 0.03f;

                    Owner.Velocity = MathHelper.Lerp(negligibleSpeed, initialDashSpeed, interpolant) * Utilities.SafeNormalise(Owner.Velocity);
                    Owner.Home(player.Position, trackingStrength);
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
                            float progress = localSwingThroughTime / (float)rapidPunchSwingTime;
                            float interpolant = EasingFunctions.EaseOutExpo(progress);
                            float holdOutDistFraction = 0.95f;
                            float maxXOffset = 20f;
                            float xOffset = maxXOffset * EasingFunctions.EaseInOutSin(progress);
                            Vector2 targetPosition = Arm(i).Position + new Vector2(expandedi * xOffset, Arm(i).Length()).Rotate(Owner.Rotation) * holdOutDistFraction;
                            Arm(i).LerpToPoint(targetPosition, interpolant);

                            Vector2 wristVelocityDirection = (Owner.Rotation + PI).ToVector();

                            Particle p = new Particle();
                            float particleSpeed = Utilities.RandomFloat(25f, 38f); // could make this scale on the velocity mayhaps
                            float angleVariance = PI / 9f;
                            float velocityAngle = Utilities.RandomAngle(angleVariance);
                            int particleLifetime = 16;
                            float initialOpacity = 1f;
                            p.Spawn("box", Arm(i).WristPosition(), particleSpeed * wristVelocityDirection.Rotate(velocityAngle), Vector2.Zero, Vector2.One * 0.4f, 0f, Color.Red, initialOpacity, particleLifetime);
                            p.AddTrail(14);
                            p.SetExtraAI(new Action(() =>
                            {
                                float interpolant = p.AITimer / (float)particleLifetime;
                                float opacity = MathHelper.Lerp(initialOpacity, 0f, interpolant);
                                p.Opacity = opacity;
                                p.GetComponent<PrimitiveTrail>().Opacity = opacity;
                                p.Velocity = p.Velocity.SetLength(MathHelper.Lerp(particleSpeed, 0, interpolant));
                                p.Rotation = p.Velocity.ToAngle();
                            }));
                        }

                        // spawn explosion at end of punch

                        if (armTimer == rapidPunchPullBackTime + rapidPunchSwingTime)
                        {
                            int projectiles = 3;
                            float spread = PI / 16;
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

            if (AITimer > rapidPunchesDuration && AITimer <= rapidPunchesDuration + restTime)
            {
                int localTime = AITimer - rapidPunchesDuration;
                // maybe move this to the accelerate part?
                float interpolant = EasingFunctions.EaseInQuart(localTime / (float)restTime);
                CrabOwner.LerpToFacePlayer(interpolant);

                if (FullTurnAroundAtEnd)
                    Owner.Velocity = Owner.Velocity.Length() * (Owner.Rotation + PI).ToVector(); // rotate velocity

                Owner.Velocity = Utilities.SafeNormalise(Owner.Velocity) * MathHelper.Lerp(initialDashSpeed, 0f, interpolant);
            }

            if (AITimer == rapidPunchesDuration + restTime)
            {
                // to do: make boss turn around to player here
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
