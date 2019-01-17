using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M12.Definitions;

namespace M12.Commands.Analog
{
    public class CommandReadADC : CommandBase
    {
        public CommandReadADC() : base()
        {

        }

        public CommandReadADC(ADCChannels ChannelEnabled)
        {
            this.ChannelEnabled = ChannelEnabled;
        }


        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_READ_AD;
            }
        }


        public ADCChannels ChannelEnabled { get; set; }

        internal override byte[] GeneratePayload()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write((byte)ChannelEnabled);
                }

                data = stream.ToArray();
            }

            return data;
        }
    }
}
