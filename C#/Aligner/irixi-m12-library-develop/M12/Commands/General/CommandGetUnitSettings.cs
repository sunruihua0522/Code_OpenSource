using M12.Definitions;

namespace M12.Commands.General
{
    public class CommandGetUnitSettings : CommandBase
    {

        public CommandGetUnitSettings(UnitID UnitID)
        {
            this.UnitID = UnitID;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_GET_MCSU_SETTINGS;
            }
        }

        #endregion

        #region Methods

        internal override byte[] GeneratePayload()
        {
            return new byte[] { (byte)this.UnitID };
        }

        #endregion
    }
}
