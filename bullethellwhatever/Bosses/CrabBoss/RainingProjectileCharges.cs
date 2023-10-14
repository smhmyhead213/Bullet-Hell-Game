using bullethellwhatever.BaseClasses;
using bullethellwhatever.DrawCode;
using bullethellwhatever.MainFiles;
using bullethellwhatever.Projectiles.TelegraphLines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.Bosses.CrabBoss
{
    public class RainingProjectileCharges : CrabBossAttack
    {
        public bool[] HasLegScreenShook;
        public RainingProjectileCharges(int endTime) : base(endTime)
        {

        }

        public override void InitialiseAttackValues()
        {
            base.InitialiseAttackValues();

            HasLegScreenShook = new bool[2] { false, false };
        }
        public override void Execute(ref int AITimer, ref int AttackNumber)
        {
            int armTime = AITimer;
            int bodyTime = AITimer;

            int waitBeforeMoving = 30;
            int SlamIntoCielingTime = 60;

            // --- arm code --- 
            if (armTime == waitBeforeMoving)
            {
                Owner.Velocity = Vector2.Zero;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 posToMoveTo = new Vector2(ScreenWidth / 2 * i + (ScreenWidth / 4), 0);

                    Vector2 direction = posToMoveTo - Leg(i).LowerClaw.Position;

                    Leg(i).PointLegInDirection(Utilities.VectorToAngle(direction));

                    Leg(i).Velocity = (direction) / SlamIntoCielingTime * 2f;
                }
            }

            if (armTime > waitBeforeMoving && armTime < waitBeforeMoving + SlamIntoCielingTime + 30) // spend a bit longer ensuring the arms dont leave to be safe
            {
                for (int i = 0; i < 2; i++)
                {
                    if (Entity.touchingAnEdge(Leg(i).LowerClaw))
                    {
                        Leg(i).Velocity = Vector2.Zero;

                        if (!HasLegScreenShook[i])
                        {
                            Drawing.ScreenShake(4, 8);
                            HasLegScreenShook[i] = true;
                        }
                    }
                }
            }

            if (armTime % 10 == 0 && armTime > waitBeforeMoving + SlamIntoCielingTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile projectile = new Projectile();

                    float horizSpeed = Utilities.RandomFloat(-10f, 10);
                    
                    projectile.VelocityFunction = x => new Vector2(horizSpeed, x * 0.3f);

                    projectile.Spawn(Leg(i).LowerClaw.Position, new Vector2(horizSpeed, 0), 1f, 1, "box", 0f, Vector2.One, Owner, true, Color.Red, true, false);
                }
            }

            // ----- body code -----

            if (bodyTime > waitBeforeMoving + SlamIntoCielingTime)
            {

            }
        }

        public override void ExtraDraw(SpriteBatch s)
        {

        }
    }
}

