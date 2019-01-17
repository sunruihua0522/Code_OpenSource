using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M12.Utilities;

namespace M12.Base
{
    public class UnitInformation
    {
        public UnitInformation(byte[] Data)
        {
            this.FirmwareVersion = VersionConverter.FromByteArray(Data);
        }

        public Version FirmwareVersion { get; private set; }
    }
}
