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
            return new Vector3(coords.X - GameWidth / 2, GameHeight / 2 - coords.Y, 0);
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

        public PrimitiveSet(VertexPositionColor[] vertices)
        {
            BasicEffect = new BasicEffect(GraphicsDevice);

            RasteriserState = new RasterizerState();

            RasteriserState.CullMode = CullMode.None; // do i cull? no idae

            PrimitiveManager.VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            PrimitiveManager.VertexBuffer.SetData(vertices);
        }

        public void Draw()
        {
            GraphicsDevice.SetVertexBuffer(PrimitiveManager.VertexBuffer);
            BasicEffect.VertexColorEnabled = true;

            GraphicsDevice.RasterizerState = RasteriserState;

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
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
            int verticesCount = afterimagesPositions.Length * 2;

            VertexPositionColor[] vertices = new VertexPositionColor[verticesCount];

            short[] indices = new short[4 * verticesCount - 3];

            float width = GetSize().X;

            for (int i = 0; i < afterimagesPositions.Length; i++)
            {
                float progress = i / (float)afterimagesPositions.Length;
                float fractionOfWidth = 1f - (width * progress);
                float widthToUse = width * fractionOfWidth;
                int startingIndex = i * 2;

                if (i != afterimagesPositions.Length - 1)
                {
                    vertices[startingIndex] =
                }
            }

            PrimitiveSet primSet = new PrimitiveSet(vertices);

            primSet.SendToDrawer();
        }
    }
}
