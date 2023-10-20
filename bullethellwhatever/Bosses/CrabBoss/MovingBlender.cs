using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class MovingBlender : CrabBossAttack
    {
        public Vector2 LeftArmStartPos;
        public Vector2 TargetPosition;
        public bool Targeting;
        public MovingBlender(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            LeftArmStartPos = Vector2.Zero;
            Targeting = false;
            TargetPosition = Vector2.Zero;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer;

            int moveInwardsTime = 15;

            // move close to the player

            int blenderBeams = 3;
            int teleDuration = 90;
            int blenderDuration = EndTime - 200;

            // ---- body code ----

            if (time == 1)
            {
                Vector2 distanceToTravel = player.Position - Owner.Position;
                distanceToTravel = distanceToTravel * 0.7f; // dont go all the way to player
                Owner.Velocity = distanceToTravel / moveInwardsTime;

                for (int i = 0; i < blenderBeams; i++)
                {
                    TelegraphLine t = new TelegraphLine(i * Tau / blenderBeams, 0, 0, 50, 3000, teleDuration, Owner.Position, Color.White, "box", Owner, true);

                    t.SpawnDeathrayOnDeath(1f, blenderDuration, PI / 300, 0, true, Color.Red, "DeathrayShader2", Owner, true);
                    //t.ToSpawn.SetStayWithOwner(true);
                }

                LeftArmStartPos = Leg(0).Position;
            }

            if (time < moveInwardsTime)
            {
                CrabOwner.FacePlayer();
            }

            int decelTime = 7;

            if (time > moveInwardsTime && time < moveInwardsTime + decelTime)
            {
                Owner.Velocity = Owner.Velocity * 0.8f;
            }

            if (time == moveInwardsTime + decelTime)
            {
                Owner.Velocity = Vector2.Zero;
            }

            if (time > moveInwardsTime + decelTime)
            {
                CrabOwner.FacePlayer();

                CrabOwner.Velocity = Utilities.SafeNormalise(player.Position - Owner.Position);
            }

            //------------------

            //---- arm code ----

            // arm 0 will hover over the player dropping bombs

            int leftArmTimer = AITimer;

            Vector2 targetPos = player.Position + new Vector2(0, -300);

            if (leftArmTimer <= moveInwardsTime)
            {
                Leg(0).ContactDamage(false);
                Leg(0).Position = Vector2.Lerp(LeftArmStartPos, targetPos, time / (float)moveInwardsTime); // move towards player, and end up above guaranteed (use frequently)
            }
            else
            {
                Leg(0).Velocity = Leg(0).Velocity + new Vector2(Utilities.SafeNormalise(targetPos - Leg(0).Position).X, 0) * 0.2f; // move towards the target position, plus a bit more for wobbling

                float vLength = Leg(0).Velocity.Length();
                float maxVLength = 10f;

                if (vLength > maxVLength)
                {
                    Leg(0).Velocity = Leg(0).Velocity / (vLength / maxVLength); // ensure the wobble isnt too much
                }

                Leg(0).Position.Y = player.Position.Y - 300f; // always stay above player

                int timeForOneOpen = 100;
                float lowerClawOpenAngle =  PI / 2;

                float directionToPlayer = Utilities.VectorToAngle(player.Position - Leg(0).Position);

                Leg(0).LowerClaw.PointInDirection(directionToPlayer); // rotate towards player, and add appropriate rotation

                int timeThroughThisOpening = (time - moveInwardsTime) % timeForOneOpen;

                if (timeThroughThisOpening <= timeForOneOpen / 2)
                {                  
                    Leg(0).LowerClaw.Rotate(lowerClawOpenAngle / (timeForOneOpen / 2) * timeThroughThisOpening); // open claw
                }
                else
                {                 
                    Leg(0).LowerClaw.Rotate(lowerClawOpenAngle / (timeForOneOpen / 2) * (timeForOneOpen - timeThroughThisOpening)); // close claw
                }

                if (timeThroughThisOpening == timeForOneOpen / 2)
                {
                    int fragments = 10;

                    ExplodingProjectile p = new ExplodingProjectile(fragments, 180, 0, false, false, true);

                    p.Spawn(Leg(0).LowerClaw.Position, 10f * Utilities.SafeNormalise(player.Position - Leg(0).LowerClaw.Position), 1f, 1, "box", 0f, Vector2.One * 1.5f, Owner, true, Color.Red, true, false);
                }
            }

            float direction = Utilities.VectorToAngle(player.Position - Leg(0).Position);

            Leg(0).UpperArm.PointInDirection(direction);
            Leg(0).LowerArm.PointInDirection(direction);
            Leg(0).UpperClaw.PointInDirection(direction);

            if (Leg(0).Position.Y < 0)
            {
                Leg(0).Position.Y = 0;
            }

            // right arm will move to a random spot around the player

            int teleTime = 60;
            int chargeTime = 10;
            int projFireTime = 50;
            int delayAfter = 30;

            int totalTime = teleTime + chargeTime + projFireTime + delayAfter;

            int rightArmTimer = AITimer % (totalTime);

            if (rightArmTimer == 1)
            {
                Targeting = true;

                TargetPosition = player.Position; // target past player

                TargetPosition.X = MathHelper.Clamp(TargetPosition.X, 0, ScreenWidth);

                TargetPosition.Y = MathHelper.Clamp(TargetPosition.Y, 0, ScreenHeight); // keep target winthin bounds

                float toReticle = Utilities.VectorToAngle(TargetPosition - Leg(1).Position);

                Leg(1).PointLegInDirection(toReticle);

                TelegraphLine t = new TelegraphLine(toReticle, 0, 0, Leg(1).UpperArm.Texture.Width, 3000, teleTime - 1, Leg(1).Position, Color.White, "box", Leg(1).UpperArm, true);

                t.ChangeShader("OutlineTelegraphShader");
            }

            int moveBackTime = 10;
            
            if (rightArmTimer == teleTime - moveBackTime)
            {
                Vector2 distanceToTravelBackwards = (TargetPosition - Leg(1).Position) / 15f; // recoil by a fraction of the distance
                Leg(1).Velocity = -distanceToTravelBackwards / moveBackTime;
            }

            if (rightArmTimer == teleTime)
            {
                Targeting = false;

                Leg(1).Velocity = (TargetPosition - Leg(1).Position) / chargeTime; // go such that we get to target

                for (int i = -2; i < 3; i++) // shotgun blast
                {
                    Projectile p = new Projectile();

                    float angleBetweenEachProjectile = PI / 6;

                    Vector2 toReticle = TargetPosition - Leg(1).Position;

                    Vector2 projDirection = Utilities.RotateVectorClockwise(toReticle, i * angleBetweenEachProjectile);

                    TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(projDirection), 0, 0, 10, 3000, 20, Leg(1).LowerClaw.Position, Color.White, "box", Leg(1).UpperArm, false);

                    p.Spawn(Leg(1).LowerClaw.Position, 10f * Utilities.SafeNormalise(projDirection), 1f, 1, "box", 0, Vector2.One, Owner, true, Color.Red, true, false);
                }
            }

            int decelTimeTwo = 20;

            if (rightArmTimer > teleTime + chargeTime && rightArmTimer < teleTime + chargeTime + decelTimeTwo)
            {
                Leg(1).Velocity = Leg(1).Velocity * 0.7f; // decelerate after reaching target

                if (Entity.touchingAnEdge(Leg(1).LowerClaw))
                {
                    Leg(1).Velocity = Leg(1).Velocity * 0f; // stop if we hit an edge
                }
            }

            if (rightArmTimer == teleTime + chargeTime + decelTimeTwo)
            {
                Leg(1).Velocity = Leg(1).Velocity * 0f; // stop anyway after some time
            }

            int delayBeforeProjectiles = 15;

            if (rightArmTimer > teleTime + chargeTime + delayBeforeProjectiles && rightArmTimer < totalTime - delayAfter) // fire some projectiles
            {
                Vector2 toPlayer = player.Position - Leg(1).LowerClaw.Position;

                Leg(1).PointLegInDirection(Utilities.VectorToAngle(toPlayer));

                int delayBetweenShots = 20;
                float recoilSpeed = 20f;

                if (rightArmTimer % delayBetweenShots == 0)
                {
                    Leg(1).Velocity = recoilSpeed * Utilities.SafeNormalise(-toPlayer); // recoil

                    for (int i = -2; i < 3; i++) // shotgun blast
                    {
                        Projectile p = new Projectile();

                        float angleBetweenEachProjectile = PI / 6;

                        Vector2 projDirection = Utilities.RotateVectorClockwise(toPlayer, i * angleBetweenEachProjectile);

                        TelegraphLine t = new TelegraphLine(Utilities.VectorToAngle(projDirection), 0, 0, 10, 3000, 20, Leg(1).LowerClaw.Position, Color.White, "box", Leg(1).UpperArm, false);

                        p.Spawn(Leg(1).LowerClaw.Position, 10f * Utilities.SafeNormalise(projDirection), 1f, 1, "box", 1.01f, Vector2.One, Owner, true, Color.Red, true, false);
                    }
                }

                else
                {
                    Leg(1).Velocity = Leg(1).Velocity - (recoilSpeed / delayBetweenShots) * Utilities.SafeNormalise(-toPlayer); // slow down
                }
            }

            if (rightArmTimer > totalTime - delayAfter && rightArmTimer < totalTime)
            {
                Leg(1).Velocity = Leg(1).Velocity * 0.98f;

                Vector2 toPlayer = player.Position - Leg(1).LowerClaw.Position;

                Leg(1).PointLegInDirection(Utilities.VectorToAngle(toPlayer));
            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            if (Targeting)
            {
                Drawing.BetterDraw(Assets["TargetReticle"], TargetPosition, null, Color.White, 0, Vector2.One, SpriteEffects.None, 1);
            }
        }
    }
}
