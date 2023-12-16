using bullethellwhatever.DrawCode.UI;
using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using FMOD;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EyeBossPhaseTwoMinion : EyeBoss
    {
        public EyeBoss Owner;
        public EyeBossPhaseTwoMinion(EyeBoss owner, int linksInChain, Vector2 chainStartPos)
        {
            Owner = owner;

            ChainStartPosition = chainStartPos;

            Velocity = Vector2.Zero;
            Texture = Assets["Circle"];
            Size = Vector2.One * 2f;

            MaxHP = 20;

            Colour = Color.White;

            BossAttack[] attacks = new BossAttack[]
            {
                new PhaseTwoBulletHell(900),
            };

            ReplaceAttackPattern(attacks);

            CreateChain(linksInChain);
        }

        public override void Update()
        {
            base.Update();

            if (Health <= 0)
            {               
                IsDesperationOver = true;
            }
        }

        public override void HandlePhaseChanges()
        {
            // do nothing
        }

        public override void Die()
        {
            base.Die();

            int numberOfProjectles = 60;

            float totalHeal = Owner.MaxHP / Owner.PhaseTwoMinionCount;

            float healEach = totalHeal / numberOfProjectles;

            for (int i = 0; i < numberOfProjectles; i++)
            {
                Projectile p = new Projectile();

                p.SetExtraAI(new Action(() =>
                {
                    if (p.AITimer <= 55)
                    {
                        p.Velocity = p.Velocity * 0.98f;
                    }
                    else
                    {
                        p.HomeAtTarget(Owner.Position, 0.14f);
                        p.Velocity = p.Velocity * 1.05f;
                    }

                    if (p.IsCollidingWith(Owner))
                    {
                        p.DeleteNextFrame = true;
                        Owner.Heal(healEach);
                    }
                }));

                p.SetDrawAfterimages(11, 7);

                float releaseAngle = Utilities.RandomAngle();

                float speed = Utilities.RandomFloat(10f, 20f);

                p.Spawn(Pupil.Position, speed * Utilities.RotateVectorClockwise(Vector2.UnitX, releaseAngle), 0f, 1, "Circle", 1f, Vector2.One * 0.1f, Owner, false, Color.LightBlue, false, true);

                p.SetParticipating(false);
            }
        }
        public override void DrawHPBar(SpriteBatch spriteBatch)
        {
            // do the same as NPC, dont draw the large HP bar

            if (Participating)
            {
                UI.DrawHealthBar(spriteBatch, this, Position + new Vector2(0, 10f * DepthFactor()), 50f * DepthFactor(), 10f * DepthFactor());
            }
        }
    }
}
