using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdapterPattern_适配器.Adapter;
namespace AdapterPattern_适配器
{
    class Program
    {
        static void Main(string[] args)
        {
            //将原有的People改造为Animal
            AnimalAdapter animal = new AnimalAdapter();
            animal.Climb();

            Console.ReadKey();
        }
    }
}
