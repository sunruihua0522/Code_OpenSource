using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrixiStepperControllerHelper
{
    public class PCA9534Info
    {
        public PCA9534Info()
        {
            I2C_Result = new bool[3];
            Value = new UInt32[3];
        }

        public bool[] I2C_Result { get; private set; }
        public UInt32[] Value { get; private set; }

        public bool Parse(byte[] data)
        {
            this.I2C_Result[0] = BitConverter.ToUInt32(data, 1) > 0;
            this.Value[0] = BitConverter.ToUInt32(data, 5);
            this.I2C_Result[1] = BitConverter.ToUInt32(data, 9) > 0;
            this.Value[1] = BitConverter.ToUInt32(data, 13);
            this.I2C_Result[2] = BitConverter.ToUInt32(data, 17) > 0;
            this.Value[2] = BitConverter.ToUInt32(data, 21);
            return true;
        }

        public override string ToString()
        {
            return string.Format("{0}/0x{1:X}, {2}/0x{3:X}, {4}/0x{5:X}", new object[] 
            {
                I2C_Result[0],
                Value[0],
                I2C_Result[1],
                Value[1],
                I2C_Result[2],
                Value[2],
            });
        }
    }
}
