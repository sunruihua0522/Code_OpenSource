using System;
using System.Collections.Generic;
using System.IO;
using M12.Utilities;

namespace M12.Base
{
    public class SystemInformation
    {
        public SystemInformation(byte[] Data)
        {
            UnitFwInfo = new List<UnitInformation>();

            using (MemoryStream stream = new MemoryStream(Data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    this.MaxUnit = (int)reader.ReadByte();
                    this.FirmwareVersion = VersionConverter.FromByteArray(reader.ReadBytes(4));
                    for(int i = 0; i < MaxUnit; i++)
                    {
                        UnitFwInfo.Add(new UnitInformation(reader.ReadBytes(4)));
                    }
                }
            }
        }

        public int MaxUnit { get; private set; }
        public Version FirmwareVersion { get; private set; }
        public List<UnitInformation> UnitFwInfo { get; set; }
    }
}
