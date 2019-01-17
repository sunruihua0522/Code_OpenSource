using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterPattern_适配器.Adapter
{
    public class AnimalAdapter
    {
        private AdapterPattern_适配器.OriginClass.People p =new AdapterPattern_适配器.OriginClass.People();
        public void Climb()
        {
            p.Run();
        }
    }
}
