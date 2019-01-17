using System;
using System.Collections.ObjectModel;
using System.Linq;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.Equipments.BaseClass
{
    public class EquipmentCollection<T>: ObservableCollection<T>
        where T: IHashable
    {
        public T FindItemByHashString(string hash)
        {
            T obj;
            var ret = this.Where(item => item.HashString == hash);

            if (ret.Any())
                obj = ret.First();
            else
            {
                throw new Exception($"unable to find the element of {hash}");
            }

            return obj;
        }
    }
}
