using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace bullethellwhatever.Bosses.CrabBoss
{
    public class SideImpacts : CrabBossAttack
    {
        public int Timer;
        public bool HitWall;
        public int Direction;
        public float OtherArmRotation;
        public bool MovedToCentre;
        public int ProjectileTimer;
        public SideImpacts(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();
            Timer = -50;
            ProjectileTimer = 0;
            HitWall = false;
            Direction = 1;
            MovedToCentre = false;
        }

        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int heightToBeAt = ScreenHeight / 30;

            if (Timer < 0 && !MovedToCentre)
            {
                CrabOwner.SetBoosters(false);
                MoveToPoint(new Vector2(ScreenWidth / 2, heightToBeAt), Timer + 50, 50);
                Owner.DealDamage = false;
            }

            if (Timer == 0)
            {                
                Owner.DealDamage = true;
                MovedToCentre = true;
            }

            int timeToStart = 10;
            float accel = 1f;
            float initSpeed = 10f;

            if (Timer == timeToStart)
            {
                Owner.Velocity = initSpeed * Vector2.UnitX * Direction;
            }

            int legIndexToUse;
            int otherLeg;

            if (Direction == 1)
            {
                legIndexToUse = 1;
                otherLeg = 0;
            }
            else
            {
                legIndexToUse = 0;
                otherLeg = 1;
            }

            if (Entity.touchingLeft(Leg(legIndexToUse).LowerClaw) || Entity.touchingRight(Leg(legIndexToUse).LowerClaw))
            {
                HitWall = true;
            }

            if (Timer > 10 && !HitWall)
            {
                float anglePerFrame = PI / 70f;

                Owner.Velocity = Owner.Velocity + Direction * Vector2.UnitX * accel;

                Owner.Rotation = Direction * Owner.Velocity.Length() * PI / 3000f; // slight movement rotation

                Leg(legIndexToUse).RotateLeg(-anglePerFrame * Direction);

                if (Timer % 3 == 0)
                {
                    int teleDuration = 75;

                    Projectile p = new Projectile(); // falling projectiles
                    Projectile otherArmSweep = new Projectile();

                    float angleFromV = Leg(otherLeg).LowerArm.RotationFromV();

                    TelegraphLine t = new TelegraphLine(PI, 0, 0, 10, ScreenHeight, teleDuration, Owner.Position, Color.White, "box", Owner, false);
                    TelegraphLine otherArmProjLine = new TelegraphLine(angleFromV, 0, 0, 10, ScreenHeight, teleDuration, Owner.Position, Color.White, "box", Owner, false);

                    p.VelocityFunction = x => new Vector2(0f, 0.6f * x);
                    otherArmSweep.VelocityFunction = x => Utilities.RotateVectorClockwise(new Vector2(0f, 0.6f * x), angleFromV - PI);

                    p.Spawn(Owner.Position, Vector2.Zero, 1f, 1, "box", 0, Vector2.One * 0.8f, Owner, true, Color.Red, true, false);
                    otherArmSweep.Spawn(Owner.Position, Vector2.Zero, 1f, 1, "box", 0, Vector2.One * 0.8f, Owner, true, Color.Red, true, false);
                }

                if (Leg(otherLeg).LowerArm.Rotation != Leg(otherLeg).LowerArm.RotationConstant) // if its rotated
                {
                    // calculate the time it takes to move to the other side

                    float time = (-initSpeed + MathF.Sqrt(initSpeed * initSpeed + 2 * accel * ScreenWidth)) / accel; // solve s = ut  0.5at2 for t

                    Leg(otherLeg).RotateLeg(-OtherArmRotation / time); // half the angle to go back to rest position
                }
            }

            if (HitWall) // stuff to do on wall hit
            {
                Direction = -Direction;
                HitWall = false;
                Timer = -10;
                Owner.Velocity = Vector2.Zero;
                OtherArmRotation = Leg(legIndexToUse).UpperArm.Rotation; // store the current arms rotation so it can be undone

                //Drawing.ScreenShake(2, 8);

                int distanceBetweenEach = 28;

                int numberOfProjectiles = (ScreenHeight - heightToBeAt) / distanceBetweenEach;
                
                int teleDuration = 75;

                float randomOffset = Utilities.RandomFloat(-10f, 10f);

                for (int i = 0; i < numberOfProjectiles; i++) // this is done after the direction flip so that direction can represent the projectiles' direction as the boss will then move in the same direction as the projs
                {
                    Projectile p = new Projectile();

                    //p.Velocity = (i + 1) * Vector2.UnitX * Direction;
                    int directionLocal = Direction; // store locally because its trying to take Direction as a ref

                    p.VelocityFunction = x => new Vector2(x * 0.4f * directionLocal, 0f);

                    float xPos = Leg(legIndexToUse).LowerClaw.Position.X;

                    Vector2 spawnPos = new Vector2(xPos, heightToBeAt + ((i + 1) * distanceBetweenEach) + randomOffset);

                    p.Spawn(spawnPos, Vector2.Zero, 1f, 1, "box", 0f, Vector2.One * 0.5f, Owner, true, Color.Red, true, false);

                    TelegraphLine t = new TelegraphLine(Direction * PI / 2, 0, 0, 10, ScreenWidth * 1.1f, teleDuration, spawnPos, Color.White, "box", Owner, false);
                }

                // aimed proj

                Projectile pr = new Projectile();

                int directionLocal2 = Direction; // store locally because its trying to take Direction as a ref

                pr.VelocityFunction = x => new Vector2(x * 0.4f * directionLocal2, 0f);

                Vector2 spawnPosition = new Vector2((ScreenWidth / 2) - (Direction * ScreenWidth / 2), player.Position.Y);

                TelegraphLine t2 = new TelegraphLine(Direction * PI / 2, 0, 0, 10, ScreenWidth * 1.1f, teleDuration, spawnPosition, Color.White, "box", Owner, false);
            }

            Timer++;
        }

        public override void ExtraDraw(SpriteBatch s)
        {
            //if (activeProjectiles.Count > 0)
            //{
            //    Utilities.drawTextInDrawMethod(activeProjectiles[0].AITimer.ToString(), Utilities.CentreOfScreen(), _spriteBatch, font, Color.White);
            //    Utilities.drawTextInDrawMethod(((Deathray)activeProjectiles[0]).Duration.ToString(), Utilities.CentreOfScreen() + new Vector2(0, 20), _spriteBatch, font, Color.White);
            //    Utilities.drawTextInDrawMethod((Time - TimeToStartForcingPlayerLeft - 60).ToString(), Utilities.CentreOfScreen() + new Vector2(0, 40), _spriteBatch, font, Color.White);

            //}
        }
    }
}
