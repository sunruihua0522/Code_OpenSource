using System;
using System.Collections.Generic;
using System.IO;
using M12.Definitions;

namespace M12.Base
{
    public class ADCValues
    {
        public ADCValues(ADCChannels EnabeldCH, byte[] Data)
        {
            using (MemoryStream stream = new MemoryStream(Data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    Values = new List<short>();
                    
                    foreach(var ch in Enum.GetValues(typeof(ADCChannels)))
                    {
                        if(EnabeldCH.HasFlag((ADCChannels)ch))
                        {
                            Values.Add(reader.ReadInt16());
                        }
                    }
                }
            }
        }

        public List<short> Values { get; set; }
    }
}
