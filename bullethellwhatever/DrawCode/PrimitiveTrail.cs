﻿using bullethellwhatever.AssetManagement;
using bullethellwhatever.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public Effect Shader;

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

            // use only the number of after image indices as there are existing afterimages that are non zero

            int vertexCount = 2 * positions.Length - 1;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[vertexCount];

            if (positions.Length > 1)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    float progress = i / (float)positions.Length;
                    float fractionOfWidth = 1f - progress;
                    float widthToUse = width * fractionOfWidth;
                    int startingIndex = i * 2;

                    Color colour = Owner.Colour * fractionOfWidth;

                    if (i != positions.Length - 1)
                    {
                        Vector2 directionToNextPoint = Utilities.SafeNormalise(positions[i + 1] - positions[i]);
                        vertices[startingIndex] = PrimitiveManager.CreateVertex(positions[i] + widthToUse / 2f * Utilities.RotateVectorClockwise(directionToNextPoint, PI / 2f), colour, new Vector2(0f, progress));
                        vertices[startingIndex + 1] = PrimitiveManager.CreateVertex(positions[i] + widthToUse / 2f * Utilities.RotateVectorCounterClockwise(directionToNextPoint, PI / 2f), colour, new Vector2(1f, progress));
                    }
                    else
                    {
                        vertices[startingIndex] = PrimitiveManager.CreateVertex(positions[i], colour, new Vector2(0f, progress));
                    }
                }

                int numberOfTriangles = vertices.Length - 2;
                int indiceCount = numberOfTriangles * 3;
                short[] indices = new short[indiceCount];

                for (int i = 0; i < numberOfTriangles; i++)
                {
                    int startingIndex = i * 3;
                    indices[startingIndex] = (short)i;
                    indices[startingIndex + 1] = (short)(i + 1);
                    indices[startingIndex + 2] = (short)(i + 2);
                }

                PrimitiveSet primSet = new PrimitiveSet(vertices, indices, Shader);

                primSet.Draw();
            }
        }
    }
}
