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
        public static IndexBuffer IndexBuffer
        {
            get;
            set;
        }

        public static readonly int MaxVertices = 6144;
        public static readonly int MaxIndices = 16384;

        public static VertexPositionColorTexture[] MainVertices;
        public static short[] MainIndices;

        public static RasterizerState RasteriserState;
        public static BasicEffect BasicEffect;

        public static void Initialise()
        {
            RasteriserState = new RasterizerState();

            RasteriserState.CullMode = CullMode.None; // do i cull? no idae

            MainVertices = new VertexPositionColorTexture[MaxVertices];
            MainIndices = new short[MaxIndices];

            BasicEffect = new BasicEffect(GraphicsDevice);

            VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorTexture), MaxVertices, BufferUsage.WriteOnly);

            IndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), MaxIndices, BufferUsage.WriteOnly);
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
        public Effect Shader;

        public int IndiceCount;
        public PrimitiveSet(int verticesCount, int indicesCount, Effect shader = null)
        {
            Shader = shader;
            PrepareBuffers(verticesCount, indicesCount);
        }
        public PrimitiveSet(int verticesCount, int indicesCount, string? shader = null)
        {
            if (shader is not null)
            {
                Shader = AssetRegistry.GetShader(shader);
            }
            else
            {
                Shader = null;
            }

            PrepareBuffers(verticesCount, indicesCount);
        }

        private void PrepareBuffers(int verticesCount, int indicesCount)
        {
            PrimitiveManager.BasicEffect.VertexColorEnabled = true;
            
            PrimitiveManager.VertexBuffer.SetData(PrimitiveManager.MainVertices, 0, verticesCount);

            //PrimitiveManager.IndexBuffer = new IndexBuffer(PrimitiveManager.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            PrimitiveManager.IndexBuffer.SetData(PrimitiveManager.MainIndices, 0, indicesCount);

            IndiceCount = indicesCount;
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

            PrimitiveManager.BasicEffect.TextureEnabled = Shader is not null;

            // WHY DO THE HEAVY LIFTING MYSELF WHEN BASIC EFFECT CAN DO IT FOR ME

            PrimitiveManager.BasicEffect.Parameters["WorldViewProj"]?.SetValue(MainCamera.ShaderMatrix());

            PrimitiveManager.BasicEffect.CurrentTechnique.Passes[0].Apply();
            
            if (Shader is not null)
            {
                Shader.CurrentTechnique.Passes[0].Apply();
            }

            // dont ask what the division by 3 is. i dont know. it doesnt work otherwise.
            PrimitiveManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, IndiceCount / 3);

            if (shouldSwitchToShaderDrawing)
            {
                Drawing.RestartSpriteBatchForNotShaders(_spriteBatch);
            }
        }
    }
}
