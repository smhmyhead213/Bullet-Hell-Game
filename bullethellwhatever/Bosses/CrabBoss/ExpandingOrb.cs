using bullethellwhatever.BaseClasses;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.Bosses.CrabBoss.Projectiles;
using bullethellwhatever.Projectiles.Enemy;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace bullethellwhatever.Bosses.CrabBoss
{
    public class ExpandingOrb : CrabBossAttack
    {
        public int OrbExpansionTime;
        public int CentreMovingTime;
        public int TimeToStartOpeningHands;
        public int TimeToStopOpeningHands;
        public ExpandingOrb(int endTime) : base(endTime)
        {
            EndTime = endTime;
        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            CrabOwner.Velocity = Vector2.Zero;
            OrbExpansionTime = 300;
            CentreMovingTime = 30;

            TimeToStartOpeningHands = 40;
            TimeToStopOpeningHands = 280;
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int time = AITimer % (OrbExpansionTime);

            CrabOwner.SetBoosters(false);

            BigMassiveOrb orb = new BigMassiveOrb(0.03f, 120);

            Owner.Rotation = Owner.Rotation + (AITimer / 60000f);

            Owner.Velocity = Sin(AITimer / 40f) * -Vector2.UnitY * 2.5f;

        //    if (time < CentreMovingTime)
        //    {
        //        Owner.ContactDamage = false;
        //        MoveToPoint(Utilities.CentreOfScreen(), time, CentreMovingTime);
        //    }
        //    else Owner.ContactDamage = true;

        //    if (time > CentreMovingTime && time < TimeToStartOpeningHands) // put hands together
        //    {
        //        float xDistanceBetweenHands = Abs(Leg(0).Position.X - Leg(1).Position.X);

        //        float handClaspingTime = TimeToStartOpeningHands - CentreMovingTime;

        //        float angleToRotate = Acos(xDistanceBetweenHands / 2 / Leg(0).Length()); // assume that both arms are the same length, if not this needs fixed

        //        Leg(0).UpperArm.Rotate(-angleToRotate / handClaspingTime);
        //        Leg(0).LowerArm.Rotate(-angleToRotate / handClaspingTime);
        //        Leg(1).UpperArm.Rotate(angleToRotate / handClaspingTime);
        //        Leg(1).LowerArm.Rotate(angleToRotate / handClaspingTime);
        //    }

        //    if (time == TimeToStartOpeningHands)
        //    { 
        //        orb.Spawn(CrabOwner.Position + Utilities.RotateVectorClockwise(new Vector2(0, 100), CrabOwner.Rotation), Vector2.Zero, 1f, 1, "box", 0f, Vector2.One, Owner, true, Color.Red, false, false);
        //    }

        //    if (time > TimeToStartOpeningHands && time < TimeToStopOpeningHands)
        //    {
        //        if (time < TimeToStartOpeningHands + (TimeToStopOpeningHands - TimeToStartOpeningHands) / 2)
        //        {

        //        }
        //    }
            }

        public override void ExtraDraw(SpriteBatch s)
        {
           
        }
    }
}
