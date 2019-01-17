using M12.Definitions;
using System.IO;

namespace M12.Commands.General
{
    public class CommandSetCSSEnable : CommandBase
    {
        public CommandSetCSSEnable(CSSCH Channel, bool IsEnabled)
        {
            this.CSSChannel = Channel;
            this.IsEnabled = IsEnabled;
        }

        #region Properties

        public CSSCH CSSChannel { get; }

        public bool IsEnabled { get; }


        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_EN_CSS;
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
                    wr.Write(IsEnabled ? (byte)1 : (byte)0);
                }

                data = stream.ToArray();
            }

            return data;
        }

        #endregion
    }
}
