using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 命令模式
{
    class Program
    {
        static void Main(string[] args)
        {
            //统一协调,命令接收者
            Reciever.Reciever_Student Student = new Reciever.Reciever_Student();

            //具体命令
            Command.CommandBase Command = new Command.CommandRun(Student);
            //Command = new Command.CommandFly(Student);
            //Command = new Command.CommandEat(Student);

            //命令的发起者
            Invoker.InvokerTeacher Teacher = new Invoker.InvokerTeacher(Command);

            //执行命令
            Teacher.Excute();
            Teacher.UnDo();


            Console.ReadKey();
        }
    }
}
