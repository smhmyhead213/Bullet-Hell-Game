using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.UtilitySystems;

namespace bullethellwhatever.Bosses.CrabBoss.Attacks
{
    public class CrabMoveWithOrb : CrabBossAttack
    {
        public CrabMoveWithOrb(CrabBoss owner) : base(owner)
        {

        }
        public override void Execute(int AITimer)
        {
            int openArmsTime = 30;
            int timeAfterOrbSpawnToStartFiringProjectiles = 30;
            float totalAngleToOpenArms = PI / 5;
            float projectilesFinalWidth = 50;

            CrabOwner.FacePlayer();

            if (AITimer == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);

                    Vector2 projectileLocation = Arm(i).UpperClaw.Position + projectilesFinalWidth * Utilities.AngleToVector(Arm(i).UpperClaw.RotationFromV());
                    Texture2D box = AssetRegistry.GetTexture2D("box");

                    Vector2 finalSize = Vector2.One * (float)projectilesFinalWidth / box.Width;

                    Projectile orb = SpawnProjectile(projectileLocation, Vector2.Zero, 1f, 1, "box", Vector2.One, Owner, true, Color.White, false, false);

                    int locali = i;

                    orb.SetExtraAI(new Action(() =>
                    {
                        orb.Position = Arm(locali).UpperClaw.Position + projectilesFinalWidth * Utilities.AngleToVector(Arm(locali).UpperClaw.RotationFromV());

                        float interpolant = MathHelper.Clamp(EasingFunctions.EaseInQuart(orb.AITimer / (float)openArmsTime), 0f, 1f);
                        orb.Size = Vector2.Lerp(Vector2.One, finalSize, interpolant);

                        if (orb.AITimer > timeAfterOrbSpawnToStartFiringProjectiles && orb.AITimer % 10 == 0)
                        {
                            float shootAngle = Utilities.RandomAngle();
                            Projectile p = SpawnProjectile(orb.Position, 5f * Utilities.AngleToVector(shootAngle), 1f, 1, "box", Vector2.One, Owner, true, Color.Red, true, false);
                            p.Rotation = shootAngle;
                            p.AddTrail(20);

                            int projectieSlowTime = 45;

                            p.SetExtraAI(new Action(() =>
                            {
                                if (p.AITimer < projectieSlowTime)
                                {
                                    p.Velocity *= 0.98f;
                                }
                                else if (p.AITimer == projectieSlowTime)
                                {
                                    p.Velocity = 0.1f * Utilities.UnitVectorToPlayerFrom(p.Position);
                                }
                                else
                                {
                                    float speed = (p.AITimer - projectieSlowTime);
                                    p.Velocity = speed * Utilities.SafeNormalise(p.Velocity);
                                }
                            }));
                        }
                    }));

                    orb.SetShader("CrabSmallOrb");

                    //Leg(i).UpperClaw
                }
            }

            float upperClawOpenAngle = PI / 4;

            Owner.Velocity = Utilities.SafeNormalise(player.Position - Owner.Position);

            if (AITimer < openArmsTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    int expandedi = Utilities.ExpandedIndex(i);

                    Arm(i).LowerClaw.Rotate(-expandedi * upperClawOpenAngle / openArmsTime);
                    Arm(i).RotateLeg(-expandedi * totalAngleToOpenArms / openArmsTime);
                }
            }           
        }
    }
}
