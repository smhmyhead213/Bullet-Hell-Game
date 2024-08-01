using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.AssetManagement
{
    public abstract class ManagedAsset
    {
        public float TimeSinceLastUse;
        public string Name;
    }
}
