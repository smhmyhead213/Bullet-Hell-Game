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
using System.Security.Cryptography.X509Certificates;

namespace bullethellwhatever.DrawCode
{
    public class PrimitiveDrawer
    {
        public BasicEffect BasicEffect;
        public GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

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
        }

        public void SetMatrices()
        {
            Matrix world = Matrix.CreateTranslation(0, 0, 0);
            Matrix view = Matrix.Identity;
            Matrix projection = Matrix.CreateOrthographic(ScreenWidth, ScreenHeight, 0f, 1f);

            BasicEffect.World = world;
            BasicEffect.View = view;
            BasicEffect.Projection = projection;
        }

        public void Draw()
        {
            //basicEffect.World = world;
            //basicEffect.View = view;
            //basicEffect.Projection = projection;

            float height = 50;

            VertexPositionColor[] vertices = new VertexPositionColor[3];
            vertices[0] = new VertexPositionColor(new Vector3(ScreenWidth / 3, ScreenHeight / 2 + height, 0), Color.White);
            vertices[1] = new VertexPositionColor(new Vector3(ScreenWidth / 3 + height, ScreenHeight / 2, 0), Color.White);
            vertices[2] = new VertexPositionColor(new Vector3(ScreenWidth / 3 - height, ScreenHeight / 2, 0), Color.White);

            MainVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);

            MainVertexBuffer.SetData(vertices);
            BasicEffect.VertexColorEnabled = true;

            GraphicsDevice.SetVertexBuffer(MainVertexBuffer);

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
