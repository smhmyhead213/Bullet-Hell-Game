﻿using Microsoft.Xna.Framework;
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

        public void Apply(int pass = 0)
        {
            if (Map is not null)
            {
                // Set shader params.
                // Matrix thingy, IDT its automatic with the sb one? Doesn't hurt idt. Do a ? so it doesn't crash if it cant be found.
                //int width = _graphics.GraphicsDevice.Viewport.Width;
                //int height = _graphics.GraphicsDevice.Viewport.Height;
                //Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

                // probably move this out of the if?
                Effect.Parameters["view_projection"]?.SetValue(MainCamera.Matrix);

                Effect.Parameters["NoiseTexture"].SetValue(Map.Texture);
            }

            Effect.Parameters["colour"]?.SetValue(Colour.ToVector3());

            Effect.CurrentTechnique.Passes[pass].Apply();
        }
    }
}
