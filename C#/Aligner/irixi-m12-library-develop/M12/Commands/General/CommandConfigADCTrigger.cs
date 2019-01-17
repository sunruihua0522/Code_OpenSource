using System.IO;
using M12.Definitions;

namespace M12.Commands.General
{
    public class CommandConfigADCTrigger : CommandBase
    {
        public CommandConfigADCTrigger() : base()
        {

        }

        public CommandConfigADCTrigger(ADCChannels ChannelEnabled)
        {
            this.ChannelEnabled = ChannelEnabled;
        }


        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_SET_T_ADC;
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
