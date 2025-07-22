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
using System.Diagnostics;
using System.CodeDom;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

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

        public float InitialAngle => -PI / 2.3f;
        public float PullBackAngle => -PI / 1.4f;
        public float SwingAngle => -PullBackAngle * 2;

        public int SwingDuration => 7;
        public int ChargeDuration => 30;
        public int ChargeTimer = 0;

        public int SwingEndLag = 8;
        public int MaximumExtraChargeTime = 30;

        public SwordSwingStages SwingStage;

        public float SwingDirection; // direction to mouse
        public float Width => 22.5f;
        public float Length => 90f;

        public float LengthModifier;

        public bool Swinging;

        public Vector2 ShakeOffset;
        public float MaxShakeOffset = 3f;

        public List<Vector2> SwordEndOffsets;

        public float TrailOffsetFromSwordTip => 10f;
        public float TrailWidth => 2 * TrailOffsetFromSwordTip;

        public Shader ThermalEffect;
        public Shader SwingEffect;
        public Shader FireEffect;

        public int DrillHitCooldown = 5;
        public float DrillDamage = 0.1f;
        public float SwordDamage = 5f;
        public float MaximumExtraChargeSwordDamage = 3f;
        public Color Colour => Color.Orange;

        public float TrailThickness => Length;
        public SwordWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {
            SwordEndOffsets = new List<Vector2>();
            ThermalEffect = AssetRegistry.GetShader("ThermalSwordShader");
            SwingEffect = AssetRegistry.GetShader("ThermalSwordSwing");
            FireEffect = AssetRegistry.GetShader("FireSwordParticleShader");

            ShakeOffset = Vector2.Zero;

            LengthModifier = 1f;
        }

        public Vector2 Position()
        {
            return Owner.Position + ShakeOffset;
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
            SwordEndOffsets = new List<Vector2>();
        }

        public float FinalLength()
        {
            return LengthModifier * Length;
        }

        public float LengthModifierThroughSwing(float progress)
        {
            float maximumStretch = 1.3f;
            float easeFactor = EasingFunctions.EaseParabolic(progress);
            return 1f + maximumStretch * easeFactor;
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
                        
                    }

                    if (TimeCharged() == MaximumExtraChargeTime)
                    {

                    }

                    if (TimeCharged() >= MaximumExtraChargeTime)
                    {
                        ShakeOffset = new Vector2(Utilities.RandomFloat(MaxShakeOffset), Utilities.RandomFloat(MaxShakeOffset));

                        if (TimeCharged() % DrillHitCooldown == 0)
                        {
                            HitEnemies.Clear();
                        }
                    }

                    // if the swing is fully charged, await release to swing
                    if (KeybindReleased(LeftClick))
                    {
                        SwingStage = SwordSwingStages.Swing;
                        AITimer = -1; // so that it can be 0 on the first frame of swinging
                        ShakeOffset = Vector2.Zero;
                        HitEnemies.Clear();
                        return;
                    }

                    WeaponRotation = PullBackAngle + SwingDirection;
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
                    LengthModifier = interpolant;
                    WeaponRotation = MathHelper.Lerp(InitialAngle, PullBackAngle, interpolant);
                    WeaponRotation += SwingDirection;
                }

                SwingDirection = (MousePositionWithCamera() - Owner.Position).ToAngle(); // lock in swing direction before start of swing
            }
            else if (SwingStage == SwordSwingStages.Swing)
            {
                int extraTrailPoints = 3;

                if (AITimer <= SwingDuration)
                {
                    bool lastAdd = AITimer == SwingDuration;
                    // prevent extra swing angle with extra trail points
                    if (lastAdd)
                    {
                        extraTrailPoints = 0;
                    }

                    for (int i = 0; i <= extraTrailPoints; i++)
                    {
                        float extraInterpolant = i == 0 ? 0 : i / (float)extraTrailPoints;
                        float finalInterpolant = (AITimer + extraInterpolant) / SwingDuration;
                        LengthModifier = LengthModifierThroughSwing(finalInterpolant);
                        WeaponRotation = MathHelper.Lerp(PullBackAngle, PullBackAngle + SwingAngle, EasingFunctions.EaseInQuad(finalInterpolant)) + SwingDirection;
                        Vector2 swordEnd = SwordEnd(0);

                        if (i != 0 || lastAdd)
                        {
                            SwordEndOffsets.Add(swordEnd - Owner.Position);
                        }
                    }

                    int particles = 0;

                    for (int i = 0; i < particles; i++)
                    {
                        Particle p = new Particle();
                        int lifetime = 30;
                        Color colour = Colour;
                        float rotation = WeaponRotation + Utilities.RandomAngle(PI / 15);
                        Vector2 velocity = rotation.ToVector() * Utilities.RandomFloat(2f, 3f);
                        float opacity = 1f;

                        p.Spawn("Circle", SwordEnd(TrailOffsetFromSwordTip), velocity, -velocity / lifetime, Vector2.One, rotation, colour, opacity, lifetime);

                        p.SetExtraAI(new Action(() =>
                        {
                            float progress = (float)p.AITimer / lifetime;
                            p.Opacity = MathHelper.Lerp(opacity, 0f, progress);
                        }));

                        //p.AddTrail(10);
                    }
                }

                if (AITimer == SwingDuration + 1 + SwingEndLag)
                {
                    Reset();
                }
            }
        }

        public Vector2 SwordEnd(float offset = 0f)
        {
            // re-add swingdirection
            Vector2 toSwordEnd = CalculateEnd(WeaponRotation) - Position();
            Vector2 point = Position() + toSwordEnd.SetLength(FinalLength() - offset);
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

            if (SwingStage == SwordSwingStages.Prepare)
            {
                DealDamage(npc, DrillDamage);
                HitEnemies.Add(npc);

                float sparkSpread = PI / 9;
                float sparkDirection = WeaponRotation + PI + Utilities.RandomFloat(sparkSpread);
                float sparkSpeed = 5f;

                CommonParticles.Spark(SwordEnd(), sparkSpeed * sparkDirection.ToVector(), 20, Color.Orange);
            }
            else
            {
                float damageInterpolant = MathHelper.Clamp(TimeCharged() / (float)MaximumExtraChargeTime, 0f, 1f);
                DealDamage(npc, SwordDamage + damageInterpolant * MaximumExtraChargeSwordDamage);
                HitEnemies.Add(npc);

                Drawing.ScreenShake(10, 3);
            }
        }
        public override void UpdateHitbox()
        {
            if (Swinging)
            {
                Hitbox = Utilities.FillRectWithCircles(Owner.Position + 0.5f * (CalculateEnd() - Position()), (int)Width, (int)Length, WeaponRotation);
            }
        }        

        public List<Vector2> TrailVertices()
        {
            if (SwordEndOffsets.Count == 0)
            {
                // explodes pancakes with mind
                return new List<Vector2>();
            }
            
            List<Vector2> vertices = new List<Vector2>();

            for (int i = 0;  i < SwordEndOffsets.Count - 1; i++)
            {
                vertices.Add(Owner.Position + SwordEndOffsets[i]);
                vertices.Add(Owner.Position + SwordEndOffsets[i + 1]);
                vertices.Add(Owner.Position);
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
                float width = 1f;

                PrimitiveManager.AddVertex(vertices[startIndex], colour, new Vector3(progress, 0f, width));
                PrimitiveManager.AddVertex(vertices[startIndex + 1], colour, new Vector3(nextProgress, 0f, width));
                PrimitiveManager.AddVertex(vertices[startIndex + 2], colour, new Vector3(progress, 1f, width));
            }

            int numberOfTriangles = vertexCount / 3;

            int indexCount = numberOfTriangles * 3;

            for (int i = 0; i < indexCount; i++)
            {
                PrimitiveManager.AddIndex(i);
            }

            SwingEffect.SetColour(Colour);

            if (AITimer >= SwingDuration)
                SwingEffect.SetParameter("fadeOutProgress", (AITimer - SwingDuration) / (float)SwingEndLag);
            else
                SwingEffect.SetParameter("fadeOutProgress", 0f);

            PrimitiveSet primSet = new PrimitiveSet(vertexCount, indexCount, SwingEffect);

            primSet.Draw();
        }

        public override void Draw(SpriteBatch s)
        {
            if (Swinging)
            {
                Drawing.RestartSB(s, true, true);

                ThermalEffect.SetParameter("heatYRatio", EasingFunctions.EaseInQuart(TimeCharged() / (float)MaximumExtraChargeTime));
                ThermalEffect.SetColour(Colour);
                ThermalEffect.Apply();

                Texture2D texture = AssetRegistry.GetTexture2D("SwordWeapon");
                float xscale = Width / texture.Width;
                float yscale = FinalLength() / texture.Height;
                Drawing.BetterDraw(texture, Position(), null, Colour, WeaponRotation, new Vector2(xscale, yscale), SpriteEffects.None, 0f, new Vector2(Width / 2 / xscale, FinalLength() / yscale));

                Drawing.RevertToPreviousSBState(s);
            }

            DrawSweepTrail(TrailVertices(), Color.Red);

            if (SwingStage == SwordSwingStages.Swing)
            {
                int timer = MathHelper.Clamp(AITimer, 0, SwingDuration);
                FireEffect.SetParameter("fadeOutProgress", EasingFunctions.Linear(timer / (float)SwingDuration));
                FireEffect.SetNoiseMap("RandomNoise", 0f);
                FireEffect.SetColour(Colour);
                FireEffect.SetParameter("uTime", AITimer);

                List<Vector2> vertices = new List<Vector2>();
                
                for (int i = 0; i < SwordEndOffsets.Count; i++)
                {
                    vertices.Add(Owner.Position);
                    vertices.Add(Owner.Position + SwordEndOffsets[i]);
                }

                int vertexCount = vertices.Count;

                if (vertexCount == 0) return;

                for (int i = 0; i < vertexCount / 2; i++)
                {
                    int startIndex = i * 2;
                    float progress = (float)i / (vertexCount / 2);

                    float width = progress;

                    float leftCurrentTextureCoord = 0.5f - width * 0.5f;
                    float rightCurrentTextureCoord = 0.5f + width * 0.5f;

                    PrimitiveManager.AddVertex(vertices[startIndex], Colour, new Vector3(progress, leftCurrentTextureCoord, width));
                    PrimitiveManager.AddVertex(vertices[startIndex + 1], Colour * progress, new Vector3(progress, rightCurrentTextureCoord, width));
                }

                int numberOfTriangles = vertexCount - 2;

                int indexCount = numberOfTriangles * 3;

                for (int i = 0; i < numberOfTriangles; i++)
                {
                    int startingIndex = i * 3;
                    PrimitiveManager.AddIndex(i);
                    PrimitiveManager.AddIndex(i + 1);
                    PrimitiveManager.AddIndex(i + 2);
                }

                PrimitiveSet primSet = new PrimitiveSet(vertexCount, indexCount, FireEffect);

                primSet.Draw();
            }        
        }
    }
}
