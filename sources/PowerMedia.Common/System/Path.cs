using RealSystem = System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerMedia.Common.System.IO
{
    public class Path
    {
        
        public static char DirectorySeparatorChar
        {
            get
            {                  
                switch (RealSystem.Environment.OSVersion.Platform)                
                {
                    case RealSystem.PlatformID.Unix:                        
                    case RealSystem.PlatformID.MacOSX:
                        return '/';
                    default:
                        return '\\';
                }                
            }
        }
    }
}
