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

        public float PullBackAngle => -PI / 4;
        public float SwingAngle => -PullBackAngle * 2;

        public int SwingDuration => 30;
        public int ChargeDuration => 30;
        public int ChargeTimer = 0;

        public SwordSwingStages SwingStage;

        public float SwingDirection; // direction to mouse
        public float Width => 22.5f;
        public float Length => 90f;

        public bool Swinging;

        public List<Vector2> TrailPoints;

        public float TrailOffsetFromSwordTip => 10f;
        public float TrailWidth => 2 * TrailOffsetFromSwordTip;
        public SwordWeapon(Player player, string iconTexture) : base(player, iconTexture)
        {
            TrailPoints = new List<Vector2>();
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
            return;

            if (SwingStage == SwordSwingStages.Prepare)
            {
                if (KeybindPressed(LeftClick) || true)
                {
                    ChargeTimer++;
                    Swinging = true;
                }
                else
                {
                    Swinging = false;
                }

                if (Charged())
                {
                    SwingStage = SwordSwingStages.Swing;
                    ChargeTimer = 0;
                    AITimer = 0;
                }

                float interpolant = EasingFunctions.EaseOutCubic((float)ChargeTimer / ChargeDuration);
                WeaponRotation = MathHelper.Lerp(0f, PullBackAngle, interpolant);
                SwingDirection = (MousePositionWithCamera() - Owner.Position).ToAngle(); // lock in swing direction at start of swing
            }
            else if (SwingStage == SwordSwingStages.Swing)
            {
                int extraTrailPoints = 2;

                for (int i = 0; i < extraTrailPoints; i++)
                {
                    float extraInterpolant = i / (float)extraTrailPoints;
                    WeaponRotation = MathHelper.Lerp(PullBackAngle, PullBackAngle + SwingAngle, EasingFunctions.EaseInQuad((AITimer + extraInterpolant) / SwingDuration));
                    Vector2 toSwordEnd = CalculateEnd(WeaponRotation + SwingDirection) - Owner.Position;
                    Vector2 point = Owner.Position + toSwordEnd.SetLength(Length - TrailOffsetFromSwordTip);
                    TrailPoints.Add(point);
                }

                if (AITimer == SwingDuration + 1)
                {
                    Reset();
                }
            }

            WeaponRotation += SwingDirection; // face mouse
        }

        public override void PrimaryFire()
        {

        }

        public override void SecondaryFire()
        {

        }

        public bool Charged()
        {
            return ChargeTimer == ChargeDuration;
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
            List<Vector2> output = new List<Vector2>();

            for (int i = 0; i < TrailPoints.Count - 1; i++)
            {
                // generate a point for each side of the trail
                // get a vector to the next point
                Vector2 toNext = TrailPoints[i + 1] - TrailPoints[i];
                
                Vector2 above = TrailPoints[i] + toNext.SetLength(TrailWidth / 2).Rotate(PI / 2);
                Vector2 below = TrailPoints[i] + toNext.SetLength(TrailWidth / 2).Rotate(-PI / 2);

                output.Add(above);
                output.Add(below);
            }

            return output;
        }

        public void DrawTrail()
        {
            List<Vector2> vertices = TrailVertices();

            if (vertices.Count == 0)
            {
                return; // explodes pancakes with mind
            }

            for (int i = 0; i < vertices.Count; i += 2)
            {
                float progress = i / (float)vertices.Count;
                PrimitiveManager.MainVertices[i] = PrimitiveManager.CreateVertex(vertices[i], Color.Red, new Vector2(0f, progress));
                PrimitiveManager.MainVertices[i + 1] = PrimitiveManager.CreateVertex(vertices[i + 1], Color.Red, new Vector2(1f, progress));
            }

            int numberOfTriangles = vertices.Count - 2;

            int indexCount = numberOfTriangles * 3;

            for (int i = 0; i < numberOfTriangles; i++)
            {
                int startingIndex = i * 3;
                PrimitiveManager.MainIndices[startingIndex] = (short)i;
                PrimitiveManager.MainIndices[startingIndex + 1] = (short)(i + 1);
                PrimitiveManager.MainIndices[startingIndex + 2] = (short)(i + 2);
            }

            //Utilities.drawTextInDrawMethod((StartPosition - positions.Last()).Length().ToString(), player.Position + new Vector2(50f, 0f), s, font, Color.White);

            PrimitiveSet primSet = new PrimitiveSet(vertices.Count, indexCount);

            primSet.Draw();
        }

        public override void Draw(SpriteBatch s)
        {
            if (Swinging)
            {
                Texture2D texture = AssetRegistry.GetTexture2D("SwordWeapon");
                float xscale = Width / texture.Width;
                float yscale = Length / texture.Height;
                Drawing.BetterDraw(texture, Owner.Position, null, Color.Orange, WeaponRotation, new Vector2(xscale, yscale), SpriteEffects.None, 0f, new Vector2(Width / 2 / xscale, Length / yscale));
            }

            //foreach (Vector2 point in TrailVertices())
            //{
            //    Drawing.DrawBox(point, Color.Red, 0.5f);
            //}

            DrawTrail();
        }
    }
}
