using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.AssetManagement
{
    public class ManagedShader : ManagedAsset
    {
        private Effect asset;
        public EffectParameterCollection Parameters => asset.Parameters;
        public EffectTechnique CurrentTechnique => asset.CurrentTechnique;
        public Effect Asset
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

        public ManagedShader(Effect shader)
        {
            Asset = shader;
            TimeSinceLastUse = 0;
        }
    }
}
