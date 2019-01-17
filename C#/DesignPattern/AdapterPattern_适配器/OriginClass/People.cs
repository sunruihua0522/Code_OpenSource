using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterPattern_适配器.OriginClass
{
    public class People
    {
        public void Run()
        {
            Console.WriteLine("实现了行走的目标");
        }
    }
}
