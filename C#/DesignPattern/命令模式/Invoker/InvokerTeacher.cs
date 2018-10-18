using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 命令模式.Invoker
{
    public class InvokerTeacher
    {
        private Command.CommandBase command;
        public InvokerTeacher(Command.CommandBase command)
        {
            this.command = command;
        }
        public void Excute()
        {
            command.Action();
        }
        public void UnDo()
        {
            command.UnDo();
        }

    }
}
