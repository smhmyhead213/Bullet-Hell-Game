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
            Effect = AssetRegistry.GetShader(filename);
            Colour = colour;
        }

        public void SetNoiseMap(string filename, float scrollSpeed)
        {
            Map = new NoiseMap(AssetRegistry.GetTexture2D(filename), scrollSpeed);
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

        public void Apply(int pass = 0)
        {
            _graphics.GraphicsDevice.Textures[1] = Map.Texture;
            Effect.CurrentTechnique.Passes[pass].Apply();
        }
    }
}
