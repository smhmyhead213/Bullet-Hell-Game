using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bullethellwhatever.MainFiles;
using log4net;
using log4net.Config;

namespace bullethellwhatever.UtilitySystems
{
    public static class ErrorLogger
    {
        private static ILog Log;

        public static void Initialise()
        {
            Log = LogManager.GetLogger(typeof(Main));
            BasicConfigurator.Configure();
        }
    }
}
