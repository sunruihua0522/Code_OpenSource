using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M12.Definitions;

namespace M12.Commands.General
{
    public class CommandGetUnitState : CommandBase
    {
        public CommandGetUnitState(UnitID UnitID)
        {
            this.UnitID = UnitID;
        }

        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_GET_MCSU_STA;
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
