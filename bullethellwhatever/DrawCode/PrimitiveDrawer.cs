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
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

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
        public static void AddPoint(int index, Vector2 point, Color colour, Vector2 texCoord)
        {
            MainVertices[index] = CreateVertex(point, colour, texCoord);
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

        public static Vector2[] GenerateStripVertices(Vector2[] points, Func<float, float> widthFunction)
        {
            if (points.Length < 2)
            {
                throw new Exception("Cannot make a primitive strip with less than two points.");
            }

            Vector2[] vertices = new Vector2[points.Length * 2];
            int index = 0;

            Vector2 toNext = Utilities.SafeNormalise(points[1] - points[0]);
            vertices[index++] = points[0] + widthFunction(0) / 2 * toNext.Rotate(PI / 2);
            vertices[index++] = points[0] + widthFunction(0) / 2 * toNext.Rotate(-PI / 2);

            for (int i = 1; i < points.Length; i++)
            {
                float y_progress = (float)i / points.Length;
                Vector2 centrePoint = points[i];
                float widthHere = widthFunction(y_progress);
                Vector2 fromPrevToMe = Utilities.SafeNormalise(points[i] - points[i - 1]);
                vertices[index++] = centrePoint + widthHere / 2 * fromPrevToMe.Rotate(PI / 2);
                vertices[index++] = centrePoint + widthHere / 2 * fromPrevToMe.Rotate(-PI / 2);
            }

            return vertices;
        }

        public static void DrawVertexStrip(Vector2[] vertices, Color colour, Shader shader)
        {
            int vertexCount = vertices.Length;

            for (int i = 0; i < vertexCount / 2; i++) // this is okay because vertices come in pairs
            {
                int startIndex = i * 2;
                float progress = i / (vertexCount / 2);
                MainVertices[startIndex] = CreateVertex(vertices[startIndex], colour, new Vector2(0f, progress));
                MainVertices[startIndex + 1] = CreateVertex(vertices[startIndex + 1], colour, new Vector2(1f, progress));
            }

            int numberOfTriangles = vertexCount - 2;

            int indexCount = numberOfTriangles * 3;

            for (int i = 0; i < numberOfTriangles; i++)
            {
                int startingIndex = i * 3;
                MainIndices[startingIndex] = (short)i;
                MainIndices[startingIndex + 1] = (short)(i + 1);
                MainIndices[startingIndex + 2] = (short)(i + 2);
            }

            PrimitiveSet primSet = new PrimitiveSet(vertexCount, indexCount, shader);

            primSet.Draw();
        }
    }
    public class PrimitiveSet
    {
        public Shader Shader;

        public int IndiceCount;
        public PrimitiveSet(int verticesCount, int indicesCount, Shader shader = null)
        {
            Shader = shader;

            PrepareBuffers(verticesCount, indicesCount);
        }
        public PrimitiveSet(int verticesCount, int indicesCount, string? shader)
        {
            if (shader is not null)
            {
                Shader = new Shader(shader, Color.Gray); // treating gray as a debug colour to see if the actual colour is passed through
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
            if (IndiceCount != 0)
            {
                bool wasDrawingShaders = Drawing.DrawingShaders;
                bool shouldSwitchToShaderDrawing = !wasDrawingShaders && Shader is not null;

                if (shouldSwitchToShaderDrawing)
                {
                    Drawing.RestartSpriteBatchForShaders(_spriteBatch, true);
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
                    Shader.SetParameter("view_projection", MainCamera.ShaderMatrix());
                    Shader.Apply();
                }

                // dont ask what the division by 3 is. i dont know. it doesnt work otherwise.
                PrimitiveManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, IndiceCount / 3);

                if (shouldSwitchToShaderDrawing)
                {
                    Drawing.RestartSpriteBatchForNotShaders(_spriteBatch, true);
                }
            }
        }
    }
}
