using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 命令模式.Reciever;
namespace 命令模式.Command
{
    public abstract class CommandBase
    {
        protected Reciever_Student reciever;
        public CommandBase(Reciever_Student reciever)
        {
            this.reciever = reciever;
        }
        public abstract void Action();
        public abstract void UnDo();
    }
}
