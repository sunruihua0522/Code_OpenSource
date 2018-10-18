using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 命令模式.Command
{
    public class CommandEat : CommandBase
    {
        public CommandEat(Reciever.Reciever_Student student) : base(student)
        {
        }
        public override void Action()
        {
            reciever.Eat();
        }

        public override void UnDo()
        {
            reciever.UnDoEat();
        }
    }
}
