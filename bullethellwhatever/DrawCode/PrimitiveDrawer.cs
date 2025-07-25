﻿using Microsoft.Xna.Framework.Graphics;
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
using System.Net.Sockets;

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

        public static VertexPositionColourTexture3[] MainVertices;
        public static short[] MainIndices;

        public static RasterizerState RasteriserState;
        public static BasicEffect BasicEffect;

        public static int VertexCounter;
        public static int IndexCounter;

        public static void Initialise()
        {
            InitialiseArrays();

            RasteriserState = new RasterizerState();

            RasteriserState.CullMode = CullMode.None; // do i cull? no idae

            BasicEffect = new BasicEffect(GraphicsDevice);

            VertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColourTexture3), MaxVertices, BufferUsage.WriteOnly);

            IndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), MaxIndices, BufferUsage.WriteOnly);

            VertexCounter = 0;
            IndexCounter = 0;
        }

        public static void InitialiseArrays()
        {
            MainVertices = new VertexPositionColourTexture3[MaxVertices];
            MainIndices = new short[MaxIndices];
        }

        public static void AddVertex(Vector2 coords, Color colour, Vector3 texCoords)
        {
            MainVertices[VertexCounter] = CreateVertex(coords, colour, texCoords);
            VertexCounter++;
        }
        public static void AddIndex(int index)
        {
            MainIndices[IndexCounter] = (short)index;
            IndexCounter++;
        }

        public static void AddPoint(int index, Vector2 point, Color colour, Vector3 texCoord)
        {
            MainVertices[index] = CreateVertex(point, colour, texCoord);
        }

        public static SharpDX.Matrix3x3 GameCoordsToVertexCoordsMatrix()
        {
            Vector2 screenCentre = Utilities.CentreOfScreen();
            // move 0,0 to centre of screen
            SharpDX.Matrix3x3 recentre = new SharpDX.Matrix3x3(1, 0, -screenCentre.X, 0, 1, -screenCentre.Y, 0, 0, 1);
            // move negative y direction to bottom
            SharpDX.Matrix3x3 negateY = new SharpDX.Matrix3x3(1, 0, 0, 0, -1, 0, 0, 0, 1);
            // squish x and y coordinates to -1 to 1 range
            SharpDX.Matrix3x3 squishXY = new SharpDX.Matrix3x3(1f / screenCentre.X, 0, 0, 0, 1f / screenCentre.Y, 0, 0, 0, 1);

            return squishXY * negateY * recentre;
        }

        public static System.Numerics.Matrix4x4 FourByFourGameToVertex()
        {
            Vector2 screenCentre = Utilities.CentreOfScreen();
            //System.Numerics.Matrix4x4 recentre = new System.Numerics.Matrix4x4(1, 0, 0, -screenCentre.X, 0, 1, 0, -screenCentre.Y, 0, 0, 1, 0, 0, 0, 0, 1);
            System.Numerics.Matrix4x4 recentre = System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(-screenCentre.X, -screenCentre.Y, 0));
            System.Numerics.Matrix4x4 negateY = System.Numerics.Matrix4x4.CreateReflection(new System.Numerics.Plane(new System.Numerics.Vector3(0, 1, 0), 0));
            System.Numerics.Matrix4x4 squishXY = new System.Numerics.Matrix4x4(1f / screenCentre.X, 0, 0, 0, 0, 1f / screenCentre.Y, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            // order reversed here because Vector4x4.Transform uses a row vector instead of a column vector for no reason
            System.Numerics.Matrix4x4 final = recentre * negateY * squishXY;
            return final;
        }

        public static System.Numerics.Matrix4x4 FourByFourVertexToGame()
        {
            Vector2 screenCentre = Utilities.CentreOfScreen();
            System.Numerics.Matrix4x4 unsquishXY = new System.Numerics.Matrix4x4(screenCentre.X, 0, 0, 0, 0, screenCentre.Y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0); // ???????????
            System.Numerics.Matrix4x4 negateY = System.Numerics.Matrix4x4.CreateReflection(new System.Numerics.Plane(new System.Numerics.Vector3(0, 1, 0), 0));
            System.Numerics.Matrix4x4 uncentre = System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(screenCentre.X, screenCentre.Y, 0));
            return uncentre * negateY * unsquishXY;
        }

        public static Vector2 ToVertexCoords(Vector2 world)
        {
            System.Numerics.Matrix4x4 transformMatrix = FourByFourGameToVertex();
            Vector4 infour = Vector4.Transform(new Vector4(world.X, world.Y, 1, 1), transformMatrix);
            return new Vector2(infour.X, infour.Y);
        }
        public static System.Numerics.Vector3 ToVertexCoordsThree(Vector2 world)
        {
            System.Numerics.Matrix4x4 transformMatrix = FourByFourGameToVertex();
            Vector4 infour = Vector4.Transform(new Vector4(world.X, world.Y, 0, 1), transformMatrix);
            return new System.Numerics.Vector3(infour.X, infour.Y, 1);
        }
        public static Vector3 OldGameCoordsToVertexCoords(Vector2 coords)
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

        public static Vector3 GameCoordsToVertexCoords(Vector2 coords)
        {
            float z = 0.5f;
            Vector3 output =  new Vector3(coords.X, coords.Y, 1).Transfom(GameCoordsToVertexCoordsMatrix());
            return output;
        }
        public static Vector2 VertexCoordsToGameCoords(Vector2 coords)
        {
            coords.X *= Utilities.CentreOfScreen().X;
            coords.Y *= Utilities.CentreOfScreen().Y;

            coords.Y /= -1f;

            coords = coords + Utilities.CentreOfScreen();

            return coords;
        }
        public static VertexPositionColourTexture3 CreateVertex(Vector2 coords, Color colour, Vector3 texCoords)
        {
            return new VertexPositionColourTexture3(GameCoordsToVertexCoords(coords), colour, texCoords);
        }

        // width function takes a progress ratio from 0-1 and returns an absolute width
        public static Vector2[] GenerateStripVertices(Vector2[] points, Func<float, float> widthFunction)
        {
            if (points.Length < 2)
            {
                return Array.Empty<Vector2>();
                //throw new Exception("Cannot make a primitive strip with less than two points.");
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

        public static void DrawVertexStrip(Vector2[] vertices, Color colour, Func<float, float> opacity, Shader shader = null)
        {
            int vertexCount = vertices.Length;

            if (vertexCount == 0) return;

            for (int i = 0; i < vertexCount / 2; i++) // this is okay because vertices come in pairs
            {
                int startIndex = i * 2;
                float progress = (float)i / (vertexCount / 2);
                float width = 1f; // for now

                MainVertices[startIndex] = CreateVertex(vertices[startIndex], colour * opacity(progress), new Vector3(progress, 0f, width));
                MainVertices[startIndex + 1] = CreateVertex(vertices[startIndex + 1], colour * opacity(progress), new Vector3(progress, 1f, width));
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

        public static void VisualiseTriangles(int indexCount)
        {
            for (int i = 0; i < indexCount; i += 3)
            {
            }
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
            PrimitiveManager.VertexCounter = 0;
            PrimitiveManager.IndexCounter = 0;

            if (IndiceCount != 0)
            {
                bool wasDrawingShaders = Drawing.SBSettings.DrawingShaders;
                bool shouldSwitchToShaderDrawing = !wasDrawingShaders && Shader is not null;

                if (shouldSwitchToShaderDrawing)
                {
                    Drawing.RestartSB(_spriteBatch, true, true);
                }

                PrimitiveManager.GraphicsDevice.SetVertexBuffer(PrimitiveManager.VertexBuffer);

                PrimitiveManager.GraphicsDevice.RasterizerState = PrimitiveManager.RasteriserState;
                PrimitiveManager.GraphicsDevice.Indices = PrimitiveManager.IndexBuffer;

                PrimitiveManager.BasicEffect.TextureEnabled = Shader is not null;

                PrimitiveManager.BasicEffect.CurrentTechnique.Passes[0].Apply();

                System.Numerics.Matrix4x4 matrix = MainCamera.ShaderMatrix();

                if (Shader is not null)
                {
                    //Shader.SetParameter("worldViewProjection", matrix);
                    Shader.Apply(prims: true);
                }
                else
                {
                    PrimitiveManager.BasicEffect.Parameters["WorldViewProj"]?.SetValue(matrix);
                    PrimitiveManager.BasicEffect.CurrentTechnique.Passes[0].Apply();
                }
      
                PrimitiveManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, IndiceCount / 3);

                if (shouldSwitchToShaderDrawing)
                {
                    Drawing.RestartSB(_spriteBatch, false, true);
                }
            }
        }

        public void TestTransform()
        {
            System.Numerics.Matrix4x4 matrix = MainCamera.ShaderMatrix();

            List<Vector4> testoutput = new List<Vector4>();

            for (int i = 0; i < PrimitiveManager.MainVertices.Count(); i++)
            {
                Vector3 point = PrimitiveManager.MainVertices[i].Position;

                if (point.X == 0 && point.Y == 0)
                {
                    break;
                }

                Vector4 test = new Vector4(point.X, point.Y, point.Z, 1);
                Vector4 transformed = Vector4.Transform(point, matrix);
                testoutput.Add(transformed);
            }
        }
    }
}
