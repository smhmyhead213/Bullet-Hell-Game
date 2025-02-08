﻿using bullethellwhatever.AssetManagement;
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
        public Effect Shader;
        public float Opacity;
        public float Width;
        public Vector2 StartPosition;
        public Color Colour;
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
            Width = Owner.GetSize().X * Owner.Texture.Width;
            StartPosition = Owner.Position;
            Colour = Owner.Colour;
        }

        public PrimitiveTrail(int length, float width, Vector2 startPos, Color colour, string shader = null)
        {
            if (shader is not null)
            {
                Shader = AssetRegistry.GetShader(shader);
            }
            else
            {
                Shader = null;
            }

            Opacity = 1f;
            afterimagesPositions = new Vector2[length];
            Width = width;
            StartPosition = startPos;
            Colour = colour;
        }

        public void SetWidth(float width)
        {
            Width = width;
        }

        public void SetPosition(Vector2 position)
        {
            StartPosition = position;
        }

        public void SetColour(Color colour)
        {
            Colour = colour;
        }

        public void Update(float width, Vector2 startPosition, Vector2 pointToAdd, Color colour)
        {
            afterimagesPositions = Utilities.moveArrayElementsUpAndAddToStart(afterimagesPositions, pointToAdd);
            Width = width;
            Colour = colour;
            StartPosition = startPosition;
        }

        public override void Update()
        {
            afterimagesPositions = Utilities.moveArrayElementsUpAndAddToStart(afterimagesPositions, Owner.Position);
        }

        public override void Draw(SpriteBatch s)
        {
            float width = Width;
            Vector2 startPosition = StartPosition;
            Color colour = Colour;

            Vector2[] positions = afterimagesPositions.Where(position => position != Vector2.Zero).ToArray();

            // explodes pancakes with mind
            if (positions.Length == 0)
            {
                return;
            }

            int vertexCount = 2 * (positions.Length + 1);
            Vector2 toNext = Utilities.SafeNormalise(positions[0] - startPosition);

            PrimitiveManager.MainVertices[0] = PrimitiveManager.CreateVertex(startPosition + width / 2f * Utilities.RotateVectorClockwise(toNext, PI / 2f), colour, new Vector2(0f, 0f));
            PrimitiveManager.MainVertices[1] = PrimitiveManager.CreateVertex(startPosition + width / 2f * Utilities.RotateVectorCounterClockwise(toNext, PI / 2f), colour, new Vector2(1f, 0f));

            for (int i = 0; i < positions.Length; i++)
            {
                float progress = (i + 1) / (float)positions.Length;
                float fractionOfWidth = 1f - progress;
                float widthToUse = width * fractionOfWidth;
                int startingIndex = (i + 1) * 2;

                // the last position gets multiplied by zero anyway so it can be whatever

                toNext = i == positions.Length - 1 ? Vector2.One : Utilities.SafeNormalise(positions[i + 1] - positions[i]);

                colour = colour * fractionOfWidth * Opacity;

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