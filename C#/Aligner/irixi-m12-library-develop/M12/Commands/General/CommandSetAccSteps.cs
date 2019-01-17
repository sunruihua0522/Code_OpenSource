using System.IO;
using M12.Definitions;

namespace M12.Commands.General
{
    public class CommandSetAccSteps : CommandBase
    {
        public CommandSetAccSteps(UnitID UnitID, ushort AccelerationSteps)
        {
            this.UnitID = UnitID;
            this.AccelerationSteps = AccelerationSteps;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_SET_ACC;
            }
        }

        public ushort AccelerationSteps { get; set; }

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
                    wr.Write(AccelerationSteps);
                }

                data = stream.ToArray();
            }

            return data;
        }

        #endregion
    }
}
