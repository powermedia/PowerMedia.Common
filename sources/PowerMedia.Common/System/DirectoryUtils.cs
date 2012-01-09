using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PowerMedia.Common.Threading;

namespace PowerMedia.Common.System
{
    public static class DirectoryUtils
    {

        public static bool CanAccessDirectory(string path)
        {
            var result = false;
            var executed = false;
            for (uint i = 0; i < 3; ++i)
            {
                executed = ThreadUtils.ExecuteMethodWithTimeoutSync(5000, () => result = DoesDirectoryExist(path));
                if (!executed) 
                {
                    continue;
                }
                break;
            }

            if (!executed) { return false; }

            return result;
        }

        public static bool DoesDirectoryExist(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            bool exists = false;
            try
            {
                exists = directoryInfo.Parent.GetDirectories()
                .Where(d => d.Name.Equals(directoryInfo.Name, StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            catch (IOException) { }

            return exists;
        }

        public static DirectoryInfo CreateDirectoryIfNeeded(string directory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            if (dirInfo.Exists)
            {
                return dirInfo;
            }
            dirInfo.Create();
            return dirInfo;
        }
        
        public static DirectoryInfo RemoveDirectoryIfExists(string directory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            if (dirInfo.Exists)
            {
                dirInfo.Delete(true);
            }
            return dirInfo;
        }
        
    }
}
