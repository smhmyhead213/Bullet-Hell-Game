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
        public static VertexBuffer VertexBuffer;
        public static IndexBuffer IndexBuffer;

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
        public static VertexPositionColor CreateVertex(Vector2 coords, Color colour)
        {
            return new VertexPositionColor(GameCoordsToVertexCoords(coords), colour);
        }
    }
    public class PrimitiveSet
    {
        public BasicEffect BasicEffect;
        public GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

        public RasterizerState RasteriserState;

        public int IndiceCount;

        public PrimitiveSet(VertexPositionColor[] vertices, short[] indices)
        {
            BasicEffect = new BasicEffect(GraphicsDevice);

            RasteriserState = new RasterizerState();

            RasteriserState.CullMode = CullMode.None; // do i cull? no idae

            PrimitiveManager.VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            PrimitiveManager.VertexBuffer.SetData(vertices);

            PrimitiveManager.IndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            PrimitiveManager.IndexBuffer.SetData(indices);


            IndiceCount = indices.Length;

        }

        public void Draw()
        {
            GraphicsDevice.SetVertexBuffer(PrimitiveManager.VertexBuffer);
            BasicEffect.VertexColorEnabled = true;

            GraphicsDevice.RasterizerState = RasteriserState;
            GraphicsDevice.Indices = PrimitiveManager.IndexBuffer;

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, IndiceCount);
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

            VertexPositionColor[] vertices = new VertexPositionColor[vertexCount];

            for (int i = 0; i < positions.Length; i++)
            {
                float progress = i / (float)positions.Length;
                float fractionOfWidth = 1f - progress;
                float widthToUse = width * fractionOfWidth;
                int startingIndex = i * 2;

                if (i != positions.Length - 1)
                {
                    Vector2 directionToNextPoint = Utilities.SafeNormalise(positions[i + 1] - positions[i]);
                    vertices[startingIndex] = PrimitiveManager.CreateVertex(positions[i] + widthToUse / 2f * Utilities.RotateVectorClockwise(directionToNextPoint, PI / 2f), Colour);
                    vertices[startingIndex + 1] = PrimitiveManager.CreateVertex(positions[i] + widthToUse / 2f * Utilities.RotateVectorCounterClockwise(directionToNextPoint, PI / 2f), Colour);
                }
                else
                {
                    vertices[startingIndex] = PrimitiveManager.CreateVertex(positions[i], Colour);
                }
            }

            //Texture2D texture = AssetRegistry.GetTexture2D("box");

            //foreach (VertexPositionColor vertex in vertices)
            //{
            //    _spriteBatch.Draw(texture, PrimitiveManager.VertexCoordsToGameCoords(new Vector2(vertex.Position.X, vertex.Position.Y)), null, Color.Red, 0, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 1);
            //}

            short[] indices = new short[vertexCount * 3];

            for (int i = 0; i < indices.Length / 3; i++)
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
