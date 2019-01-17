using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M12.Definitions;

namespace M12.Base
{
    public class SystemLastError
    {
        public SystemLastError(byte[] Data)
        {
            using (MemoryStream stream = new MemoryStream(Data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    this.UnitID = (UnitID)reader.ReadByte();
                    this.Error = (Errors)reader.ReadByte();
                }
            }
        }

        public SystemLastError(UnitID UnitID, Errors Error)
        {
            this.UnitID = UnitID;
            this.Error = Error;
        }

        public UnitID UnitID { get; set; }
        public Errors Error { get; set; }
    }
}
