using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 命令模式.Command
{
    public class CommandRun : CommandBase
    {
        public CommandRun(Reciever.Reciever_Student reciever) : base(reciever)
        {

        }
        public override void Action()
        {
            reciever.Run500Miles();
        }

        public override void UnDo()
        {
            reciever.UnDoRun500Miles();
        }
    }
}
