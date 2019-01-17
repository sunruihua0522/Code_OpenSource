using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M12.Commands.IO
{
    public class CommandReadDOUT : CommandBase
    {
        public override Commands Command
        {
            get
            {
                return Commands.HOST_CMD_READ_DOUT;
            }
        }
    }
}
