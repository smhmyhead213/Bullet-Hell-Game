using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.AssetManagement;
using System.Reflection;

namespace bullethellwhatever.DrawCode
{
    public class Shader
    {
        public Effect Effect;
        public Color Colour;
        public NoiseMap Map;
        public Shader(string filename, Color colour)
        { 
            Effect = AssetRegistry.GetEffect(filename);
            Colour = colour;
        }

        public void SetNoiseMap(string filename, float scrollSpeed)
        {
            Map = new NoiseMap(AssetRegistry.GetTexture2D(filename), scrollSpeed);
            SetParameter("NoiseTexture", Map.Texture);
            SetParameter("scrollSpeed", Map.ScrollSpeed);
        }
        public void SetColour(Color colour)
        {
            Colour = colour;
        }

        public void UpdateShaderParameters(float AITimer)
        {
            SetParameter("uTime", AITimer);
            SetParameter("colour", Colour);

            if (Map is not null)
            {
                SetParameter("noiseMap", Map.Texture);
                SetParameter("scrollSpeed", Map.ScrollSpeed);
            }
        }

        public void SetParameter(string name, float value)
        {
            Effect.Parameters[name]?.SetValue(value);
        }
        public void SetParameter(string name, Color colour)
        {
            Effect.Parameters[name]?.SetValue(colour.ToVector3());
        }
        public void SetParameter(string name, Texture2D texture)
        {
            Effect.Parameters[name]?.SetValue(texture);
        }
        public void SetParameter(string name, System.Numerics.Matrix4x4 matrix)
        {
            Effect.Parameters[name]?.SetValue(matrix);
        }
        public void SetParameter(string name, Matrix matrix)
        {
            Effect.Parameters[name]?.SetValue(matrix);
        }
        public void Apply(int pass = 0, bool prims = false)
        {
            if (Map is not null)
            {
                //int width = _graphics.GraphicsDevice.Viewport.Width;
                //int height = _graphics.GraphicsDevice.Viewport.Height;
                //Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

                // probably move this out of the if?
                //Effect.Parameters["view_projection"]?.SetValue(MainCamera.Matrix);

                //Effect.Parameters["NoiseTexture"]?.SetValue(Map.Texture);
                SetParameter("NoiseTexture", Map.Texture);
            }

            Matrix matrix = prims ? MainCamera.ShaderMatrix() : Matrix.Identity;

            //Effect.Parameters["worldViewProjection"]?.SetValue(matrix);

            SetParameter("worldViewProjection", matrix);

            //Effect.Parameters["colour"]?.SetValue(Colour.ToVector3());
            SetParameter("colour", Colour);

            Effect.CurrentTechnique.Passes[pass].Apply();
        }
    }
}
