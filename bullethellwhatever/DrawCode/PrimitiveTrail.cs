using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.BaseClasses.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode
{
    public class PrimitiveTrail : Component
    {
        public Vector2[] afterimagesPositions;
        public PrimitiveSet PrimitiveSet;
        public Shader Shader;
        public float Opacity;
        public float Width;
        public bool AccountForOwnerOpacity;
        public Color Colour;
        public PrimitiveTrail(Entity owner, int length, string shader = null)
        {
            if (shader is not null)
            {
                Shader = new Shader(shader, Color.White);
            }
            else
            {
                Shader = null;
            }

            Owner = owner;
            Opacity = 1f;
            afterimagesPositions = new Vector2[length];
            Width = Owner.GetSize().X * Owner.Texture.Width;

            Colour = Owner.Colour;
            AccountForOwnerOpacity = false;
        }

        // unused, consider removing if you rememeber why you added it in the first place
        public PrimitiveTrail(int length, float width, Color colour, string shader = null)
        {
            if (shader is not null)
            {
                Shader = new Shader(shader, Color.White);
            }
            else
            {
                Shader = null;
            }

            Opacity = 1f;
            afterimagesPositions = new Vector2[length];
            Width = width;
            Colour = colour;
            AccountForOwnerOpacity = false;
        }
        public void SetWidth(float width)
        {
            Width = width;
        }

        public void SetColour(Color colour)
        {
            Colour = colour;
        }

        public void AddPoint(Vector2 point)
        {
            afterimagesPositions = Utilities.moveArrayElementsUpAndAddToStart(afterimagesPositions, point);
        }

        public void PreUpdate(float width, Vector2 pointToAdd, Color colour)
        {            
            Width = width;
            Colour = colour;
        }

        public override void PostUpdate()
        {
            AddPoint(Owner.Position);
        }
        public void PostUpdate(Vector2 point)
        {
            AddPoint(point);
        }

        public override void PreUpdate()
        {
            //AddPoint(Owner.Position);
            Colour = Owner.Colour;
            Width = Owner.Width();
        }

        public Vector2[] GenerateVertices(Vector2[] positions)
        {
            return PrimitiveManager.GenerateStripVertices(positions, (progress) => Width * (1f - progress));
        }
        public override void Draw(SpriteBatch s)
        {
            Vector2[] positions = afterimagesPositions.Where(position => position != Vector2.Zero).ToArray();

            // explodes pancakes with mind
            if (positions.Length == 0)
            {
                return;
            }

            Vector2[] vertices = GenerateVertices(positions);
            PrimitiveManager.DrawVertexStrip(vertices, Colour, (progress) => Opacity * (1f - progress), Shader);
        }
    }
}
