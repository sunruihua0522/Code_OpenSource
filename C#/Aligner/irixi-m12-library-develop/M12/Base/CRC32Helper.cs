using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace STM32F407
{
    public class CRC32
    {
        const UInt32 INIT_VAL = 0xFFFFFFFF;
        const UInt32 POLY = 0x4C11DB7;
        UInt32 _dr = INIT_VAL;

        /// <summary>
        /// Get the 32-bit CRC value.
        /// </summary>
        public UInt32 CRC
        {
            get
            {
                return _dr;
            }
            private set
            {
                _dr = value;
            }
        }

        /// <summary>
        /// Reset the data register of the CRC
        /// </summary>
        public void Reset()
        {
            this.CRC = INIT_VAL;
        }

        /// <summary>
        /// Computes the 32-bit CRC of 32-bit data buffer independently of the previous CRC value.
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public UInt32 Calculate(UInt32[] Buffer, int Length)
        {
            Reset();
            return Accumulate(Buffer, Length);
        }

        public UInt32 Calculate(byte[] Data, int Length)
        {
            int len = Length / 4;
            int remain = Length % 4;

            byte[] tmp = null;
            UInt32[] crcRaw = null;

            if(remain > 0)
                tmp = new byte[len * 4 + 4];
            else
                tmp = new byte[Length];

            crcRaw = new UInt32[tmp.Length / 4];

            Buffer.BlockCopy(Data, 0, tmp, 0, Length);
            Buffer.BlockCopy(tmp, 0, crcRaw, 0, tmp.Length);

            return Calculate(crcRaw, crcRaw.Length);
        }

        /// <summary>
        /// Computes the 32-bit CRC of 32-bit data buffer using combination of the previous CRC value and the new one.
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public UInt32 Accumulate(UInt32[] Buffer, int Length)
        {
            for (int i = 0; i < Length; i++)
            {
                calculate(Buffer[i]);
            }

            return this.CRC;
        }

        /// <summary>
        /// Computs the 32-bit CRC using the preivous CRC value and the new value.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private UInt32 calculate(UInt32 data)
        {
            int bindex = 0;
            UInt32 crc = data ^ this.CRC;

            while (bindex < 32)
            {
                if ((crc & 0x80000000) > 0)
                {
                    crc = (crc << 1) ^ POLY;
                }
                else
                {
                    crc <<= 1;
                }

                bindex++;
            }

            this.CRC = crc;

            return crc;
        }
    }
}
