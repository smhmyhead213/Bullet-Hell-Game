using bullethellwhatever.AssetManagement;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.DrawCode
{
    public class NoiseMap
    {
        public Texture2D Texture;
        public float ScrollSpeed;
        public NoiseMap(Texture2D map, float scrollSpeed)
        {
            Texture = map;
            ScrollSpeed = scrollSpeed;
        }
        public NoiseMap(ManagedTexture map, float scrollSpeed)
        {
            Texture = map.Asset;
            ScrollSpeed = scrollSpeed;
        }
    }
}
