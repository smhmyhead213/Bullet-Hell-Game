using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace bullethellwhatever.AssetManagement
{
    public class ManagedTexture : ManagedAsset
    {
        private Texture2D asset;

        public int Width => Asset.Width;
        public int Height => Asset.Height;
        public Texture2D Asset
        {
            get
            {
                return asset;
            }
            set
            {
                asset = value;
                TimeSinceLastUse = 0;
            }
        }

        public ManagedTexture(Texture2D asset)
        {
            Asset = asset;
            TimeSinceLastUse = 0;
        }
    }
}