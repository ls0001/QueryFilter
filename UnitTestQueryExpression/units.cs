using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace UnitsComm
{ 
public  class Units
    {
        public static void Log(object format ,params dynamic[] p)
        {

            var po = Array.ConvertAll(p, new Converter<dynamic, string>(x => x?.ToString()));
            var str = Convert.ToString(format);
            Logger.LogMessage(str, po);
        }
        
    }
}
