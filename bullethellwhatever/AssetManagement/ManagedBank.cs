using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMOD.Studio;

namespace bullethellwhatever.AssetManagement
{
    public class ManagedBank : ManagedAsset
    {
        private Bank asset;
        
        public Bank Asset
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

        public ManagedBank(Bank bank)
        {
            Asset = asset;
            TimeSinceLastUse = 0;
        }
    }
}
