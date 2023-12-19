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
using bullethellwhatever.DrawCode;
using bullethellwhatever.Projectiles.Base;
using bullethellwhatever.BaseClasses.Hitboxes;

namespace bullethellwhatever.Bosses.EyeBoss
{
    public class EyeBossPhaseTwoMinion : EyeBoss
    {
        public EyeBoss Owner;
        public float BaseVulnerabilityRadius;
        public float VulnerabilityRadius;
        public bool OscillateRadius;
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
                new PhaseTwoBulletHell(90000),
            };

            ReplaceAttackPattern(attacks);

            BaseVulnerabilityRadius = 0;
            VulnerabilityRadius = BaseVulnerabilityRadius;
            OscillateRadius = false;

            CreateChain(linksInChain);
        }

        public override void Update()
        {
            base.Update();

            if (Health <= 0)
            {               
                IsDesperationOver = true;
            }

            if (OscillateRadius)
            {
                VulnerabilityRadius = BaseVulnerabilityRadius + 10f * Sin(AITimer / 10f);
            }
            else VulnerabilityRadius = BaseVulnerabilityRadius;

            if (!IsPlayerWithinVulnerabilityRadius()) // if the player is outside the ring
            {
                TargetableByHoming = false;
                IsInvincible = true;

                foreach (Projectile p in activeFriendlyProjectiles)
                {
                    if (Utilities.DistanceBetweenVectors(p.Position, Position) < VulnerabilityRadius && p is not Deathray) // if the projectile is within the ring
                    {
                        p.Die();
                        p.OnHitEffect(p.Position);
                    }
                }
            }
            else
            {
                IsInvincible = false;
                TargetableByHoming = true;
            }
        }

        public bool IsPlayerWithinVulnerabilityRadius()
        {
            return Utilities.DistanceBetweenVectors(player.Position, Position) < VulnerabilityRadius;
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

            float healEach = totalHeal / numberOfProjectles / 5;

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

        public override void TakeDamage(Collision collision, Projectile projectile)
        {
            base.TakeDamage(collision, projectile);

            foreach (ChainLink c in ChainLinks)
            {
                float projectileVelocityHorizontalComponent = -Sin(Utilities.VectorToAngle(projectile.Velocity));
                c.ApplyTorque(projectileVelocityHorizontalComponent / 200f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            bool npcHasShader = Shader is not null;

            if (!npcHasShader)
            {
                Drawing.RestartSpriteBatchForShaders(spriteBatch);
            }

            Effect circleShader = Shaders["CircleOutlineShader"];

            circleShader.Parameters["colour"]?.SetValue(Color.White.ToVector3());
            circleShader.Parameters["uTime"]?.SetValue(AITimer);
            circleShader.Parameters["radius"]?.SetValue(0.5f);
            ApplyRandomNoise(circleShader);

            circleShader.CurrentTechnique.Passes[0].Apply();

            Texture2D texture = Assets["box"];

            Drawing.BetterDraw(texture, Pupil.Position, null, Color.White, 0, Vector2.One * VulnerabilityRadius / texture.Width * 2f, SpriteEffects.None, 1);

            if (!npcHasShader)
            {
                Drawing.RestartSpriteBatchForNotShaders(spriteBatch);
            }
        }
    }
}
