using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M12.Definitions;

namespace M12.Base
{
    public class DigitalInputStatus
    {
        public DigitalInputStatus(byte[] Data)
        {
            Integrated = new DigitalIOStatus[8];

            BitArray bits = new BitArray(Data);
            this.DIN1 = bits.Get(0) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            this.DIN2 = bits.Get(1) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            this.DIN3 = bits.Get(2) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            this.DIN4 = bits.Get(3) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            this.DIN5 = bits.Get(4) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            this.DIN6 = bits.Get(5) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            this.DIN7 = bits.Get(6) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            this.DIN8 = bits.Get(7) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;

            Integrated[0] = DIN1;
            Integrated[1] = DIN2;
            Integrated[2] = DIN3;
            Integrated[3] = DIN4;
            Integrated[4] = DIN5;
            Integrated[5] = DIN6;
            Integrated[6] = DIN7;
            Integrated[7] = DIN8;

        }

        public DigitalIOStatus DIN1 { get; set; }
        public DigitalIOStatus DIN2 { get; set; }
        public DigitalIOStatus DIN3 { get; set; }
        public DigitalIOStatus DIN4 { get; set; }
        public DigitalIOStatus DIN5 { get; set; }
        public DigitalIOStatus DIN6 { get; set; }
        public DigitalIOStatus DIN7 { get; set; }
        public DigitalIOStatus DIN8 { get; set; }

        public DigitalIOStatus[] Integrated { get; set; }


        public override string ToString()
        {
            List<string> tmp = new List<string>();
            for (int i = 1; i <= 8; i++)
            {
                tmp.Add($"DIN{i}: {Integrated[i - 1]}");
            }

            return string.Join(", ", tmp.ToArray());
        }
    }
}
