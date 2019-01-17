using M12.Definitions;
using System.Collections;
using System.Collections.Generic;

namespace M12.Base
{
    public class DigitalOutputStatus
    {
        public DigitalOutputStatus(byte[] Data)
        {
            Integrated = new DigitalIOStatus[8];

            BitArray bits = new BitArray(Data);

            DOUT1 = bits.Get(0) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            DOUT2 = bits.Get(1) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            DOUT3 = bits.Get(2) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            DOUT4 = bits.Get(3) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            DOUT5 = bits.Get(4) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            DOUT6 = bits.Get(5) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            DOUT7 = bits.Get(6) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;
            DOUT8 = bits.Get(7) == false ? DigitalIOStatus.OFF : DigitalIOStatus.ON;

            Integrated[0] = DOUT1;
            Integrated[1] = DOUT2;
            Integrated[2] = DOUT3;
            Integrated[3] = DOUT4;
            Integrated[4] = DOUT5;
            Integrated[5] = DOUT6;
            Integrated[6] = DOUT7;
            Integrated[7] = DOUT8;
        }

        public DigitalIOStatus DOUT1 { get; set; }
        public DigitalIOStatus DOUT2 { get; set; }
        public DigitalIOStatus DOUT3 { get; set; }
        public DigitalIOStatus DOUT4 { get; set; }
        public DigitalIOStatus DOUT5 { get; set; }
        public DigitalIOStatus DOUT6 { get; set; }
        public DigitalIOStatus DOUT7 { get; set; }
        public DigitalIOStatus DOUT8 { get; set; }

        public DigitalIOStatus[] Integrated { get; set; }

        public override string ToString()
        {
            List<string> tmp = new List<string>();
            for(int i = 1; i <= 8; i++)
            {
                tmp.Add($"DOUT{i}: {Integrated[i - 1]}");
            }

            return string.Join(", ", tmp.ToArray());
        }
    }
}
