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
        public static List<PrimitiveSet> PrimitiveSets;

        public static VertexBuffer VertexBuffer;
        public static IndexBuffer IndexBuffer;

        public static int MaxUsedIndex;

        public static void Initialise()
        {
            PrimitiveSets = new List<PrimitiveSet>();
            MaxUsedIndex = 0;
        }
        public static Vector3 GameCoordsToVertexCoords(Vector2 coords)
        {
            //return new Vector3(coords.X - GameWidth / 2, GameHeight / 2 - coords.Y, 0);

            // move 0,0 to centre of screen
            coords = coords - Utilities.CentreOfScreen();
            // move negative Y direction to bottom
            coords.Y *= -1f;
            // squish X and Y coordinates to -1 to 1 range
            coords.X /= Utilities.CentreOfScreen().X;
            coords.Y /= Utilities.CentreOfScreen().Y;

            return new Vector3(coords.X, coords.Y, 0);
        }

        public static VertexPositionColor CreateVertex(Vector2 coords, Color colour)
        {
            return new VertexPositionColor(GameCoordsToVertexCoords(coords), colour);
        }
        public static Vector3 GameCoordsToVertexCoords(float x, float y)
        {
            return new Vector3(x - GameWidth / 2, GameHeight / 2 - y, 0);
        }
        public static void DrawPrimitives()
        {
            foreach (PrimitiveSet prims in PrimitiveSets)
            {
                prims.Draw();
            }
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
            IndiceCount = indices.Length;

            BasicEffect = new BasicEffect(GraphicsDevice);

            RasteriserState = new RasterizerState();

            RasteriserState.CullMode = CullMode.None; // do i cull? no idae

            PrimitiveManager.VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            PrimitiveManager.VertexBuffer.SetData(vertices);

            PrimitiveManager.IndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            PrimitiveManager.IndexBuffer.SetData(indices);
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

        public void SendToDrawer()
        {

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

            // the array index where the afterimages start to be 0,0 

            int usedVerticesIndex = afterimagesPositions.Length - 1;

            for (int i = 0; i < afterimagesPositions.Length; i++)
            {
                if (afterimagesPositions[i] == Vector2.Zero)
                {
                    usedVerticesIndex = i;
                    break;
                }
            }

            // use only the number of after image indices as there are existing afterimages that are non zero

            int vertexCount = 2 * usedVerticesIndex - 1;

            VertexPositionColor[] vertices = new VertexPositionColor[vertexCount];

            for (int i = 0; i < usedVerticesIndex; i++)
            {
                float progress = i / (float)afterimagesPositions.Length;
                float fractionOfWidth = 1f - progress;
                float widthToUse = width * fractionOfWidth;
                int startingIndex = i * 2;

                if (i != usedVerticesIndex - 1)
                {
                    Vector2 directionToNextPoint = Utilities.SafeNormalise(afterimagesPositions[i + 1] - afterimagesPositions[i]);
                    vertices[startingIndex] = PrimitiveManager.CreateVertex(afterimagesPositions[i] + widthToUse / 2f * Utilities.RotateVectorClockwise(directionToNextPoint, PI / 2f), Colour);
                    vertices[startingIndex + 1] = PrimitiveManager.CreateVertex(afterimagesPositions[i] + widthToUse / 2f * Utilities.RotateVectorCounterClockwise(directionToNextPoint, PI / 2f), Colour);
                }
                else
                {
                    vertices[startingIndex] = PrimitiveManager.CreateVertex(afterimagesPositions[i], Colour);
                }
            }

            short[] indices = new short[vertexCount * 3];

            for (int i = 0; i < indices.Length; i += 3)
            {
                indices[i] = (short)i;
                indices[i + 1] = (short)(i + 1);
                indices[i + 2] = (short)(i + 2);
            }

            Texture2D texture = AssetRegistry.GetTexture2D("box");

            //_spriteBatch.Draw(texture, Utilities.CentreOfScreen(), null, Color.Red, 0, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 1);

            //foreach (VertexPositionColor vertex in vertices)
            //{
            //    _spriteBatch.Draw(texture, new Vector2(vertex.Position.X, vertex.Position.Y), null, Color.Red, 0, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, 1);
            //}
            
            PrimitiveSet primSet = new PrimitiveSet(vertices, indices);

            primSet.Draw();
        }
    }
}
