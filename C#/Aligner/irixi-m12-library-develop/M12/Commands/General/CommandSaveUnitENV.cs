using M12.Definitions;

namespace M12.Commands.General
{
    public class CommandSaveUnitENV : CommandBase
    {

        public CommandSaveUnitENV(UnitID UnitID)
        {
            this.UnitID = UnitID;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_SAV_MCSU_ENV;
            }
        }

        #endregion
    }
}
