using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M12.Commands.General
{
    public class CommandGetLastError : CommandBase
    {
        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_GET_ERR;
            }
        }

        #endregion
    }
}
