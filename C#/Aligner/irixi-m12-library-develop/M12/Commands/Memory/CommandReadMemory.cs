using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M12.Commands.Memory
{
    public class CommandReadMemory : CommandBase
    {

        public CommandReadMemory(uint Offset, uint LengthToRead)
        {
            this.Offset = Offset;
            this.LengthToRead = LengthToRead;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_READ_MEM;
            }
        }

        public uint Offset { get; set; }
        public uint LengthToRead { get; set; }

        #endregion

        internal override byte[] GeneratePayload()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write(Offset);
                    wr.Write(LengthToRead);
                }

                data = stream.ToArray();
            }

            return data;
        }
    }
}
