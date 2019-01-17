using System.IO;
using M12.Definitions;

namespace M12.Commands.IO
{
    public class CommandSetDOUT : CommandBase
    {
        public CommandSetDOUT():base()
        {

        }

        public CommandSetDOUT(DigitalOutput Channel, DigitalIOStatus Status)
        {
            this.Channel = Channel;
            this.Status = Status;
        }


        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_SET_DOUT;
            }
        }


        public DigitalOutput Channel { get; set; }
        public DigitalIOStatus Status { get; set; }

        internal override byte[] GeneratePayload()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write((byte)Channel);
                    wr.Write((byte)Status);
                }

                data = stream.ToArray();
            }

            return data;
        }
    }
}
