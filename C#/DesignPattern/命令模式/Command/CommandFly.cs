using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 命令模式.Command
{
    public class CommandFly : CommandBase
    {
        public CommandFly(Reciever.Reciever_Student student) : base(student)
        {
        }
        public override void Action()
        {
            reciever.Fly10000Miles();
        }

        public override void UnDo()
        {
            reciever.UnDoFly10000Miles();
        }
    }
}
