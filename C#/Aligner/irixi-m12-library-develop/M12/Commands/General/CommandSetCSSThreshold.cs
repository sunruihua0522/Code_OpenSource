using System.IO;
using M12.Definitions;

namespace M12.Commands.General
{
    public class CommandSetCSSThreshold : CommandBase
    {
        public CommandSetCSSThreshold(CSSCH Channel, ushort LowThreshold, ushort HighThreshold)
        {
            this.CSSChannel = Channel;
            this.LowThreshold = LowThreshold;
            this.HighThreshold = HighThreshold;
        }

        #region Properties

        public CSSCH CSSChannel { get; }

        public ushort LowThreshold { get; }

        public ushort HighThreshold { get; }


        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_SET_CSSTHD;
            }
        }

        #endregion

        #region Methods

        internal override byte[] GeneratePayload()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write((byte)CSSChannel);
                    wr.Write((ushort)LowThreshold);
                    wr.Write((ushort)HighThreshold);
                }

                data = stream.ToArray();
            }

            return data;
        }

        #endregion
    }
}
