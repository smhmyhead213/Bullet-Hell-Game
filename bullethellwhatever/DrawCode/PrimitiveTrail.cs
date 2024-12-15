using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using bullethellwhatever.BaseClasses.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode
{
    public class PrimitiveTrail : Component
    {
        public Vector2[] afterimagesPositions;
        public PrimitiveSet PrimitiveSet;
        public Effect Shader;
        public float Opacity;

        public PrimitiveTrail(Entity owner, int length, string shader = null)
        {
            if (shader is not null)
            {
                Shader = AssetRegistry.GetShader(shader);
            }
            else
            {
                Shader = null;
            }

            Owner = owner;
            Opacity = 1f;
            afterimagesPositions = new Vector2[length];
        }
        public override void Update()
        {
            afterimagesPositions = Utilities.moveArrayElementsUpAndAddToStart(afterimagesPositions, Owner.Position);
        }

        public override void Draw(SpriteBatch s)
        {
            float width = Owner.GetSize().X * Owner.Texture.Width;

            Vector2[] positions = afterimagesPositions.Where(position => position != Vector2.Zero).ToArray();

            // explodes pancakes with mind
            if (positions.Length == 0)
            {
                return;
            }

            int vertexCount = 2 * (positions.Length + 1);
            Vector2 toNext = Utilities.SafeNormalise(positions[0] - Owner.Position);

            PrimitiveManager.MainVertices[0] = PrimitiveManager.CreateVertex(Owner.Position + width / 2f * Utilities.RotateVectorClockwise(toNext, PI / 2f), Owner.Colour, new Vector2(0f, 0f));
            PrimitiveManager.MainVertices[1] = PrimitiveManager.CreateVertex(Owner.Position + width / 2f * Utilities.RotateVectorCounterClockwise(toNext, PI / 2f), Owner.Colour, new Vector2(1f, 0f));

            for (int i = 0; i < positions.Length; i++)
            {
                float progress = (i + 1) / (float)positions.Length;
                float fractionOfWidth = 1f - progress;
                float widthToUse = width * fractionOfWidth;
                int startingIndex = (i + 1) * 2;

                // the last position gets multiplied by zero anyway so it can be whatever

                toNext = i == positions.Length - 1 ? Vector2.One : Utilities.SafeNormalise(positions[i + 1] - positions[i]);

                Color colour = Owner.Colour * fractionOfWidth * Opacity;

                PrimitiveManager.MainVertices[startingIndex] = PrimitiveManager.CreateVertex(positions[i] + widthToUse / 2f * Utilities.RotateVectorClockwise(toNext, PI / 2f), colour, new Vector2(0f, progress));
                PrimitiveManager.MainVertices[startingIndex + 1] = PrimitiveManager.CreateVertex(positions[i] + widthToUse / 2f * Utilities.RotateVectorCounterClockwise(toNext, PI / 2f), colour, new Vector2(1f, progress));      
            }

            int numberOfTriangles = vertexCount - 2;

            int indexCount = numberOfTriangles * 3;

            for (int i = 0; i < numberOfTriangles; i++)
            {
                int startingIndex = i * 3;
                PrimitiveManager.MainIndices[startingIndex] = (short)i;
                PrimitiveManager.MainIndices[startingIndex + 1] = (short)(i + 1);
                PrimitiveManager.MainIndices[startingIndex + 2] = (short)(i + 2);
            }

            PrimitiveSet primSet = new PrimitiveSet(vertexCount, indexCount, Shader);

            primSet.Draw();
        }
    }
}