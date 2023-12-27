using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using bullethellwhatever.MainFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.BaseClasses;
using System.Windows.Forms;
using System.DirectoryServices.ActiveDirectory;

namespace bullethellwhatever.DrawCode
{
    public class PrimitiveSet
    {
        public BasicEffect BasicEffect;
        public GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        public RasterizerState RasteriserState;

        Matrix WorldMatrix = Matrix.Identity;
        Matrix ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix ProjectionMatrix = Matrix.CreateOrthographic(ScreenWidth, ScreenHeight, 0, 1000);
        public PrimitiveSet()
        {
            BasicEffect = new BasicEffect(GraphicsDevice);

            RasteriserState = new RasterizerState();

            RasteriserState.CullMode = CullMode.None;

            VertexPositionColor[] vertices = new VertexPositionColor[4];

            // coordinates within VertexPositionColour are fractions of the screen.

            //vertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            //vertices[1] = new VertexPositionColor(new Vector3(+0.5f, 0, 0), Color.Green);
            //vertices[2] = new VertexPositionColor(new Vector3(-0.5f, 0, 0), Color.Blue);

            vertices[0] = CreateVertex(Utilities.CentreOfScreen(), Color.Red);
            vertices[1] = CreateVertex(new Vector2(0, ScreenHeight), Color.Green);
            vertices[2] = CreateVertex(new Vector2(ScreenWidth, ScreenHeight), Color.Blue);
            vertices[3] = CreateVertex(new Vector2(ScreenWidth / 2, 0), Color.Yellow);

            VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(vertices);
        }

        public Vector3 GameCoordsToVertexCoords(Vector2 coords)
        {
            return new Vector3(coords.X - ScreenWidth / 2, ScreenHeight / 2 - coords.Y, 0);
        }

        public VertexPositionColor CreateVertex(Vector2 coords, Color colour)
        {
            return new VertexPositionColor(GameCoordsToVertexCoords(coords), colour);
        }
        public Vector3 GameCoordsToVertexCoords(float x, float y)
        {
            return new Vector3(x - ScreenWidth / 2, ScreenHeight / 2 - y, 0);
        }
        public void SetMatrices()
        {
            BasicEffect.World = WorldMatrix;
            BasicEffect.View = ViewMatrix;
            BasicEffect.Projection = ProjectionMatrix;
        }
        public void Update()
        {
            SetMatrices();
        }
        public void Draw()
        {
            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            BasicEffect.VertexColorEnabled = true;

            GraphicsDevice.RasterizerState = RasteriserState;

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
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
            PrimitiveSet primSet = new PrimitiveSet();

            int verticesCount = afterimagesPositions.Length * 3;

            VertexPositionColor[] vertices = new VertexPositionColor[verticesCount];

            float width = GetSize().X;

            for (int i = 0; i < afterimagesPositions.Length; i++)
            {
                float progress = i / (float)afterimagesPositions.Length;
                float fractionOfWidth = 1f - (width * progress);
                float widthToUse = width * fractionOfWidth;
                int startingIndex = i * 3;

                Vector2 halfWidth = Utilities.RotateVectorClockwise(new Vector2(widthToUse / 2, 0), Rotation);
                vertices[startingIndex] = primSet.CreateVertex(afterimagesPositions[i] + halfWidth, Colour);
                vertices[startingIndex + 1] = primSet.CreateVertex(afterimagesPositions[i] - halfWidth, Colour);
                vertices[startingIndex + 2] = primSet.CreateVertex(afterimagesPositions[i + 1], Colour);
            }

            short[] indices = new short[4 * verticesCount - 3];
        }
    }
}
