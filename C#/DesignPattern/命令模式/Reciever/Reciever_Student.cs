using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 命令模式.Reciever
{
    public class Reciever_Student
    {
        public void Run500Miles()
        {
            Console.WriteLine("Reciever_Student Run 500Miles");
        }
        public void UnDoRun500Miles()
        {
            Console.WriteLine("Reciever_Student 后退 500Miles");
        }


        public void Fly10000Miles()
        {
            Console.WriteLine("飞行了10000米");
        }
        public void UnDoFly10000Miles()
        {
            Console.WriteLine("后退飞行了10000米");
        }


        public void Eat()
        {
            Console.WriteLine("吃饭");
        }
        public void UnDoEat()
        {
            Console.WriteLine("取消吃饭");
        }

    }
}
