using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using bullethellwhatever.MainFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.DirectoryServices.ActiveDirectory;
using bullethellwhatever.Projectiles;
using System.ComponentModel.Design.Serialization;
using bullethellwhatever.AssetManagement;

namespace bullethellwhatever.DrawCode
{
    public static class PrimitiveManager
    {
        public static GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

        public static VertexBuffer VertexBuffer;
        public static IndexBuffer IndexBuffer;

        public static RasterizerState RasteriserState;

        public static void Initialise()
        {
            RasteriserState = new RasterizerState();

            RasteriserState.CullMode = CullMode.None; // do i cull? no idae
        }

        public static Vector3 GameCoordsToVertexCoords(Vector2 coords)
        {
            // move 0,0 to centre of screen
            coords = coords - Utilities.CentreOfScreen();
            // move negative Y direction to bottom
            coords.Y *= -1f;
            // squish X and Y coordinates to -1 to 1 range
            coords.X /= Utilities.CentreOfScreen().X;
            coords.Y /= Utilities.CentreOfScreen().Y;

            return new Vector3(coords.X, coords.Y, 0);
        }
        public static Vector2 VertexCoordsToGameCoords(Vector2 coords)
        {
            coords.X *= Utilities.CentreOfScreen().X;
            coords.Y *= Utilities.CentreOfScreen().Y;

            coords.Y /= -1f;

            coords = coords + Utilities.CentreOfScreen();

            return coords;
        }
        public static VertexPositionColorTexture CreateVertex(Vector2 coords, Color colour, Vector2 texCoords)
        {
            return new VertexPositionColorTexture(GameCoordsToVertexCoords(coords), colour, texCoords);
        }
    }
    public class PrimitiveSet
    {
        public BasicEffect BasicEffect;
        public Effect Shader;

        public int IndiceCount;
        public PrimitiveSet(VertexPositionColorTexture[] vertices, short[] indices, string? shader = null)
        {
            BasicEffect = new BasicEffect(PrimitiveManager.GraphicsDevice);

            if (shader is not null)
            {
                Shader = AssetRegistry.GetShader(shader);
            }
            else
            {
                Shader = null;
            }

            BasicEffect.VertexColorEnabled = true;

            PrimitiveManager.VertexBuffer = new VertexBuffer(PrimitiveManager.GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.WriteOnly);
            PrimitiveManager.VertexBuffer.SetData(vertices);

            PrimitiveManager.IndexBuffer = new IndexBuffer(PrimitiveManager.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            PrimitiveManager.IndexBuffer.SetData(indices);

            IndiceCount = indices.Length;
        }

        public void Draw()
        {
            bool wasDrawingShaders = Drawing.DrawingShaders;
            bool shouldSwitchToShaderDrawing = !wasDrawingShaders && Shader is not null;

            if (shouldSwitchToShaderDrawing)
            {
                Drawing.RestartSpriteBatchForShaders(_spriteBatch);
            }

            PrimitiveManager.GraphicsDevice.SetVertexBuffer(PrimitiveManager.VertexBuffer);

            PrimitiveManager.GraphicsDevice.RasterizerState = PrimitiveManager.RasteriserState;
            PrimitiveManager.GraphicsDevice.Indices = PrimitiveManager.IndexBuffer;

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            if (Shader is not null)
            {
                Shader.CurrentTechnique.Passes[0].Apply();
            }

            PrimitiveManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, IndiceCount);

            if (shouldSwitchToShaderDrawing)
            {
                Drawing.RestartSpriteBatchForNotShaders(_spriteBatch);
            }
        }
    }

    public class PrimAfterImageTestProj : Projectile
    {
        public PrimAfterImageTestProj() : base()
        {
            SetDrawAfterimages(20, 5);
        }

        public override void DrawAfterImages()
        {
            float width = GetSize().X * Texture.Width;

            Vector2[] positions = afterimagesPositions.Where(position => position != Vector2.Zero).ToArray();

            // use only the number of after image indices as there are existing afterimages that are non zero
         
            int vertexCount = 2 * positions.Length - 1;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[vertexCount];

            for (int i = 0; i < positions.Length; i++)
            {
                float progress = i / (float)positions.Length;
                float fractionOfWidth = 1f - progress;
                float widthToUse = width * fractionOfWidth;
                int startingIndex = i * 2;

                Color colour = Colour * fractionOfWidth;

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

            //Texture2D texture = AssetRegistry.GetTexture2D("box");

            //foreach (VertexPositionColor vertex in vertices)
            //{
            //    _spriteBatch.Draw(texture, PrimitiveManager.VertexCoordsToGameCoords(new Vector2(vertex.Position.X, vertex.Position.Y)), null, Color.Red, 0, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 1);
            //}

            int numberOfTriangles = vertices.Length - 2;

            short[] indices = new short[numberOfTriangles * 3];

            for (int i = 0; i < numberOfTriangles; i++)
            {
                int startingIndex = i * 3;
                indices[startingIndex] = (short)i;
                indices[startingIndex + 1] = (short)(i + 1);
                indices[startingIndex + 2] = (short)(i + 2);
            }


            PrimitiveSet primSet = new PrimitiveSet(vertices, indices);

            primSet.Draw();
        }
    }
}
