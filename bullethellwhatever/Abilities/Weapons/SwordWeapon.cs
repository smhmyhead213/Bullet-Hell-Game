using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using bullethellwhatever.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses.Entities;
using bullethellwhatever.DrawCode;
using bullethellwhatever.UtilitySystems;
using bullethellwhatever.NPCs;
using System.Windows.Forms;
using bullethellwhatever.AssetManagement;
using bullethellwhatever.DrawCode.Particles;
using SharpDX.DirectWrite;

namespace bullethellwhatever.Abilities.Weapons
{
    public enum SwordSwingStages
    {
        Prepare,
        Swing,
    }
    public class SwordWeapon : Weapon
    {
        public float WeaponRotation;

        public float PullBackAngle => -PI / 2.4f;
        public float SwingAngle => -PullBackAngle * 2;

        public int SwingDuration => 7;
        public int ChargeDuration => 30;
        public int ChargeTimer = 0;
        public int ChargeHoldDuration => 15;
        public int SwingEndLag = 20;
        public int MaximumExtraChargeTime = 30;

        public SwordSwingStages SwingStage;

        public float SwingDirection; // direction to mouse
        public float Width => 22.5f;
        public float Length => 90f;

        public bool Swinging;

        public List<Vector2> TrailPoints;

        public float TrailOffsetFromSwordTip => 10f;
        public float TrailWidth => 2 * TrailOffsetFromSwordTip;

        public Shader ThermalEffect;
        public SwordWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {
            TrailPoints = new List<Vector2>();
            ThermalEffect = AssetRegistry.GetShader("ThermalSwordShader");
        }

        public override void WeaponInitialise()
        {
            WeaponRotation = 0f;
            SwingStage = SwordSwingStages.Prepare;
            AITimer = 0;
            Swinging = false;
        }
        public override bool CanSwitchWeapon()
        {
            return !Swinging;
        }

        public int SwingDirectionSign()
        {
            return PullBackAngle < SwingAngle ? 1 : -1; // erm
        }
        public Vector2 CalculateEnd()
        {
            return CalculateEnd(WeaponRotation);
        }
        public Vector2 CalculateEnd(float angle)
        {
            return Owner.Position - Utilities.RotateVectorClockwise(new Vector2(0f, Length), angle);
        }

        public void Reset()
        {
            AITimer = 0;
            WeaponRotation = 0f;
            ChargeTimer = 0;
            SwingDirection = (MousePositionWithCamera() - Owner.Position).ToAngle(); // lock in swing direction at start of swing
            HitEnemies.Clear();
            Swinging = false;
            SwingStage = SwordSwingStages.Prepare;
            TrailPoints = new List<Vector2>();
        }
        public override void AI()
        {
            if (SwingStage == SwordSwingStages.Prepare)
            {
                if (KeybindPressed(LeftClick))
                {
                    ChargeTimer++;
                }

                if (Charged())
                {
                    if (TimeCharged() == 0)
                    {
                        int particles = 10;

                        for (int i = 0; i < particles; i++)
                        {
                            Particle particle = new Particle();
                            float direction = Utilities.RandomAngle();
                            float speed = Utilities.RandomFloat(2f, 7f);
                            int lifetime = Utilities.RandomInt(25, 35);
                            float initialOpacity = 0.4f;
                            Vector2 velocity = direction.ToVector() * speed;

                            particle.Spawn("box", SwordEnd(), velocity, -velocity / lifetime, Vector2.One * 0.4f, direction, Color.Brown, initialOpacity, lifetime);
                            particle.AddTrail(10);
                            particle.SetExtraAI(new Action(() =>
                            {
                                particle.Opacity = MathHelper.Lerp(initialOpacity, 0f, (float)particle.AITimer / lifetime);
                            }));
                        }
                    }

                    if (TimeCharged() == MaximumExtraChargeTime)
                    {
                        int particles = 20;

                        for (int i = 0; i < particles; i++)
                        {
                            Particle particle = new Particle();
                            float direction = Utilities.RandomAngle();
                            float speed = Utilities.RandomFloat(4f, 9f);
                            int lifetime = Utilities.RandomInt(25, 35);
                            float initialOpacity = 0.4f;
                            Vector2 velocity = direction.ToVector() * speed;

                            particle.Spawn("box", SwordEnd(), velocity, -velocity / lifetime, Vector2.One * 0.4f, direction, Color.Brown, initialOpacity, lifetime);
                            particle.AddTrail(10);
                            particle.SetExtraAI(new Action(() =>
                            {
                                particle.Opacity = MathHelper.Lerp(initialOpacity, 0f, (float)particle.AITimer / lifetime);
                            }));
                        }
                    }

                    // if the swing is fully charged, await release to swing
                    if (KeybindReleased(LeftClick))
                    {
                        SwingStage = SwordSwingStages.Swing;
                        ChargeTimer = 0;
                        AITimer = 0;
                        return;
                    }
                }
                else
                {
                    Swinging = KeybindPressed(LeftClick);

                    // if we are not fully charged, allow releasing mouse to abort swing
                    if (KeybindReleased(LeftClick))
                    {
                        ChargeTimer = 0;
                        Swinging = false;
                    }

                    // perform charge
                    float chargeTimeSpent = MathHelper.Clamp(ChargeTimer, 0f, ChargeDuration);
                    float interpolant = EasingFunctions.EaseOutCubic((float)chargeTimeSpent / ChargeDuration);
                    WeaponRotation = MathHelper.Lerp(0f, PullBackAngle, interpolant);
                    SwingDirection = (MousePositionWithCamera() - Owner.Position).ToAngle(); // lock in swing direction at start of swing
                }
            }
            else if (SwingStage == SwordSwingStages.Swing)
            {
                int extraTrailPoints = 3;

                if (AITimer <= SwingDuration)
                {
                    // prevent extra swing angle with extra trail points
                    if (AITimer == SwingDuration)
                    {
                        extraTrailPoints = 0;
                    }

                    for (int i = 0; i <= extraTrailPoints; i++)
                    {
                        float extraInterpolant = i == 0 ? 0 : i / (float)extraTrailPoints;
                        WeaponRotation = MathHelper.Lerp(PullBackAngle, PullBackAngle + SwingAngle, EasingFunctions.EaseInQuad((AITimer + extraInterpolant) / SwingDuration));
                        Vector2 point = SwordEnd(TrailOffsetFromSwordTip);
                        TrailPoints.Add(point);
                    }
                }

                if (AITimer == SwingDuration + 1 + SwingEndLag)
                {
                    Reset();
                }
            }

            //WeaponRotation += SwingDirection; // face mouse
        }

        public Vector2 SwordEnd(float offset = 0f)
        {
            // re-add swingdirection
            Vector2 toSwordEnd = CalculateEnd(WeaponRotation) - Owner.Position;
            Vector2 point = Owner.Position + toSwordEnd.SetLength(Length - offset);
            return point;
        }

        public override void PrimaryFire()
        {

        }

        public override void SecondaryFire()
        {

        }

        public bool Charged()
        {
            return ChargeTimer >= ChargeDuration;
        }

        public int TimeCharged()
        {
            return ChargeTimer - ChargeDuration;
        }
        public override void OnHit(NPC npc)
        {
            // to do: centralised damage system

            DealDamage(npc, 5f);
            HitEnemies.Add(npc);

            Drawing.ScreenShake(10, 3);
        }
        public override void UpdateHitbox()
        {
            if (Swinging)
            {
                Hitbox = Utilities.FillRectWithCircles(Owner.Position + 0.5f * (CalculateEnd() - Owner.Position), (int)Width, (int)Length, WeaponRotation);
            }
        }        

        public List<Vector2> TrailVertices()
        {
            if (TrailPoints.Count == 0)
            {
                // explodes pancakes with mind
                return new List<Vector2>();
            }
            
            List<Vector2> vertices = new List<Vector2>();

            for (int i = 0;  i < TrailPoints.Count - 1; i++)
            {
                vertices.Add(TrailPoints[i]);
                vertices.Add(TrailPoints[i + 1]);
                vertices.Add(player.Position);
            }

            return vertices;
        }

        public void DrawSweepTrail(List<Vector2> vertices, Color colour)
        {
            int vertexCount = vertices.Count;

            if (vertexCount == 0) return;

            for (int i = 0; i < vertexCount / 3; i++) // this is okay because vertices come in triplets
            {
                int startIndex = i * 3;
                float progress = (float)i / (vertexCount / 3);
                float nextProgress = (float)(i + 1) / (vertexCount / 3);
                PrimitiveManager.AddVertex(vertices[startIndex], colour, new Vector2(0f, progress));
                PrimitiveManager.AddVertex(vertices[startIndex + 1], colour, new Vector2(0f, nextProgress));
                PrimitiveManager.AddVertex(vertices[startIndex + 2], colour, new Vector2(1f, progress));
            }

            int numberOfTriangles = (vertexCount - 1) / 2;

            int indexCount = numberOfTriangles * 3;

            for (int i = 0; i < indexCount; i++)
            {
                PrimitiveManager.AddIndex(i);
            }

            PrimitiveSet primSet = new PrimitiveSet(vertexCount, indexCount);

            primSet.Draw();
        }

        public override void Draw(SpriteBatch s)
        {
            if (Swinging)
            {
                Drawing.RestartSB(s, true, true);

                Color colour = Color.Orange;

                ThermalEffect.SetParameter("heatYRatio", EasingFunctions.EaseInQuart(TimeCharged() / (float)MaximumExtraChargeTime));
                ThermalEffect.SetColour(colour);
                ThermalEffect.Apply();

                Texture2D texture = AssetRegistry.GetTexture2D("SwordWeapon");
                float xscale = Width / texture.Width;
                float yscale = Length / texture.Height;
                Drawing.BetterDraw(texture, Owner.Position, null, colour, WeaponRotation, new Vector2(xscale, yscale), SpriteEffects.None, 0f, new Vector2(Width / 2 / xscale, Length / yscale));

                Drawing.RevertToPreviousSBState(s);
            }

            //foreach (Vector2 point in TrailVertices())
            //{
            //    Drawing.DrawBox(point, Color.Red, 0.5f);
            //}

            //foreach (Vector2 point in TrailVertices())
            //{
            //    Drawing.DrawBox(point, Color.Red, 1.5f);
            //}

            DrawSweepTrail(TrailVertices(), Color.Red);
        }
    }
}
