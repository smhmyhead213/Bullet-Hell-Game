using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
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
        public VertexBuffer VertexBuffer;

        public BasicEffect BasicEffect;
        public GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;
        public struct VertexPosition2DColour : IVertexType
        {
            public Vector2 Position;

            public Color Colour;

            public Vector2 TextureCoordinates;

            private static readonly VertexDeclaration _VertexDeclaration = new(new VertexElement[]
{
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            });

            #pragma warning disable IDE0251 // Make member 'readonly'
            public VertexDeclaration VertexDeclaration => _VertexDeclaration;
            #pragma warning restore IDE0251 // Make member 'readonly'
            public VertexPosition2DColour(Vector2 position, Color colour, Vector2 texCoords)
            {
                Position = position;
                Colour = colour;
                TextureCoordinates = texCoords;
            }
        }
        public PrimitiveDrawer()
        {
            BasicEffect = new BasicEffect(GraphicsDevice);
        }

        public void Draw()
        {
            //basicEffect.World = world;
            //basicEffect.View = view;
            //basicEffect.Projection = projection;

            BasicEffect.View = Matrix.Identity;

            float height = 50;

            VertexPosition2DColour[] vertices = new VertexPosition2DColour[3];
            vertices[0] = new VertexPosition2DColour(new Vector2(ScreenWidth / 3, ScreenHeight / 2 + height), Color.White, new Vector2(0.5f, 0));
            vertices[1] = new VertexPosition2DColour(new Vector2(ScreenWidth / 3 + height, ScreenHeight / 2), Color.White, new Vector2(1, 1));
            vertices[2] = new VertexPosition2DColour(new Vector2(ScreenWidth / 3 - height, ScreenHeight / 2), Color.White, new Vector2(0, 1));

            VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPosition2DColour), 3, BufferUsage.WriteOnly);
            VertexBuffer.SetData<VertexPosition2DColour>(vertices);

            BasicEffect.VertexColorEnabled = true;

            GraphicsDevice.SetVertexBuffer(VertexBuffer);

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
