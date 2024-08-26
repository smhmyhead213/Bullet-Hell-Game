using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullethellwhatever.UtilitySystems.PlatformSpecific
{
    public abstract class Platform
    {
        public abstract string SavePath
        {
            get;
            set;
        }
        public abstract void OpenURL();


    }
}
