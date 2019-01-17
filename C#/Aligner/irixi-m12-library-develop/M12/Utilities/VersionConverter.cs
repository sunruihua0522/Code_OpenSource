using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M12.Utilities
{
    public class VersionConverter
    {
        public static Version FromByteArray(byte[] Data)
        {
            var major = (int)Data[0];
            var minor = (int)Data[1];
            var patch = (int)Data[2];
            var build = (int)Data[3];

            return new Version(major, minor, patch, build);
        }
    }
}
