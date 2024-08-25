using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.DrawCode;

 
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using bullethellwhatever.Projectiles;
using bullethellwhatever.AssetManagement;

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

            int blenderBeams = Utilities.ValueFromDifficulty(2, 3, 3, 4);

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
                    float rotation = i * Tau / blenderBeams;

                    TelegraphLine t = SpawnTelegraphLine(rotation, 0, 50, 3000, teleDuration, Owner.Position, Color.White, "box", Owner, true);

                    t.SetOnDeath(new Action(() =>
                    {
                        Deathray ray = new Deathray();
                        ray.SetStayWithOwner(true);
                        ray.SpawnDeathray(t.Origin, rotation, 1f, blenderDuration, "box", 50, 3000, PI / 600, true, Color.White, "DeathrayShader2", Owner);
                    }));
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

            float heightAbovePlayer = 500f;

            Vector2 targetPos = player.Position + new Vector2(0, -heightAbovePlayer);

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

                Leg(0).Position.Y = player.Position.Y - heightAbovePlayer; // always stay above player

                int timeForOneOpen = Utilities.ValueFromDifficulty(120, 110, 100, 80);

                float lowerClawOpenAngle = PI / 2;

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
                    int fragments = Utilities.ValueFromDifficulty(6, 8, 10, 14);

                    Projectile bomb = SpawnProjectile(Leg(0).LowerClaw.Position, 10f * Utilities.SafeNormalise(player.Position - Leg(0).LowerClaw.Position), 1f, 1, "box", Vector2.One * 1.5f, Owner, true, Color.Red, true, false);
                    bomb.SetExtraAI(new Action(() =>
                    {
                        if (bomb.AITimer == 180 || bomb.TouchingAnEdge())
                        {
                            Projectile fragment = new Projectile(bomb.Position, 3f * Vector2.One, 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                            RadialProjectileBurst(fragment, fragments, 0f, 3f);
                            bomb.InstantlyDie();
                        }
                    }));

                    
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

            int teleTime = 60;
            int chargeTime = 10;
            int projFireTime = 50;
            int delayAfter = 30;

            int totalTime = teleTime + chargeTime + projFireTime + delayAfter;

            int rightArmTimer = AITimer % (totalTime);

            if (rightArmTimer == 1)
            {
                Targeting = true;

                Leg(1).Velocity = Vector2.Zero;

                TargetPosition = player.Position; // target past player

                TargetPosition.X = MathHelper.Clamp(TargetPosition.X, 0, GameWidth);

                TargetPosition.Y = MathHelper.Clamp(TargetPosition.Y, 0, GameHeight); // keep target winthin bounds

                float toReticle = Utilities.VectorToAngle(TargetPosition - Leg(1).Position);

                Leg(1).PointLegInDirection(toReticle);

                float teleLength = Utilities.DistanceBetweenVectors(Leg(1).Position, TargetPosition); // telegraph line goes to target

                TelegraphLine t = SpawnTelegraphLine(toReticle, 0, Leg(1).UpperArm.Texture.Width, teleLength, teleTime - 1, Leg(1).Position, Color.White, "box", Leg(1).UpperArm, true);

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

                int numberOfProjsInBlast = Utilities.ValueFromDifficulty(3, 5, 5, 7);

                int loweri = -numberOfProjsInBlast / 2;
                int upperi = numberOfProjsInBlast / 2 + 1;

                for (int i = loweri; i < upperi; i++) // shotgun blast
                {
                    float angleBetweenEachProjectile = Utilities.ValueFromDifficulty(PI / 6, PI / 6, PI / 6, PI / 8);

                    Vector2 toReticle = TargetPosition - Leg(1).Position;

                    Vector2 projDirection = Utilities.RotateVectorClockwise(toReticle, i * angleBetweenEachProjectile);

                    SpawnTelegraphLine(Utilities.VectorToAngle(projDirection), 0, 10, 3000, 20, Leg(1).LowerClaw.Position, Color.White, "box", Leg(1).UpperArm, false);

                    SpawnProjectile(Leg(1).LowerClaw.Position, 10f * Utilities.SafeNormalise(projDirection), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                }
            }

            int decelTimeTwo = 20;

            if (rightArmTimer > teleTime + chargeTime && rightArmTimer < teleTime + chargeTime + decelTimeTwo)
            {
                Leg(1).Velocity = Leg(1).Velocity * 0.7f; // decelerate after reaching target

                if (Leg(1).LowerClaw.TouchingAnEdge())
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

                    int numberOfProjsInBlast = Utilities.ValueFromDifficulty(3, 5, 5, 7);

                    int loweri = -numberOfProjsInBlast / 2;
                    int upperi = numberOfProjsInBlast / 2 + 1;

                    for (int i = loweri; i < upperi; i++) // shotgun blast
                    {
                        Projectile p = new Projectile();

                        float angleBetweenEachProjectile = PI / 6;

                        Vector2 projDirection = Utilities.RotateVectorClockwise(toPlayer, i * angleBetweenEachProjectile);

                        SpawnTelegraphLine(Utilities.VectorToAngle(projDirection), 0, 10, 3000, 20, Leg(1).LowerClaw.Position, Color.White, "box", Leg(1).UpperArm, false);

                        SpawnProjectile(Leg(1).LowerClaw.Position, 10f * Utilities.SafeNormalise(projDirection), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
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
                Drawing.BetterDraw(AssetRegistry.GetTexture2D("TargetReticle"), TargetPosition, null, Color.White, 0, Vector2.One, SpriteEffects.None, 1);
            }
        }
    }
}
