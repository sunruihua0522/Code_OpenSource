using System.IO;
using M12.Base;
using M12.Definitions;

namespace M12.Commands.General
{
    public class CommandSetMode : CommandBase
    {

        public CommandSetMode(UnitID UnitID, UnitSettings Mode)
        {
            this.UnitID = UnitID;
            this.Mode = Mode;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_SET_MODE;
            }
        }

        public UnitSettings Mode { get; set; }

        #endregion

        #region Methods

        internal override byte[] GeneratePayload()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter wr = new BinaryWriter(stream))
                {
                    wr.Write((byte)UnitID);
                    wr.Write(Mode.ToArray());
                }

                data = stream.ToArray();
            }

            return data;
        }

        #endregion
    }
}
