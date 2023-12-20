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

namespace bullethellwhatever.DrawCode
{
    public class PrimitiveDrawer
    {
        public BasicEffect BasicEffect;
        public GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

        VertexBuffer VertexBuffer;

        Matrix WorldMatrix = Matrix.Identity;
        Matrix ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        //Matrix ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), ScreenWidth / ScreenHeight, 0.01f, 100f);
        Matrix ProjectionMatrix = Matrix.CreateOrthographic(ScreenWidth, ScreenHeight, 0, 1000);

        //        public struct VertexPositionColour : IVertexType
        //        {
        //            public Vector3 Position;

        //            public Color Colour;

        //            public Vector2 TextureCoordinates;

        //            private static readonly VertexDeclaration _VertexDeclaration = new(new VertexElement[]
        //{
        //                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        //                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
        //                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
        //            });

        //            #pragma warning disable IDE0251 // Make member 'readonly'
        //            public VertexDeclaration VertexDeclaration => _VertexDeclaration;
        //            #pragma warning restore IDE0251 // Make member 'readonly'
        //            public VertexPositionColour(Vector3 position, Color colour, Vector2 texCoords)
        //            {
        //                Position = position;
        //                Colour = colour;
        //                TextureCoordinates = texCoords;
        //            }
        //        }

        public PrimitiveDrawer()
        {
            BasicEffect = new BasicEffect(GraphicsDevice);

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

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }

        }
    }
}
