using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M12.Commands.Memory
{
    public class CommandClearMemory : CommandBase
    {
        #region Properties

        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_CLEAR_MEM;
            }
        }

        #endregion
    }
}
