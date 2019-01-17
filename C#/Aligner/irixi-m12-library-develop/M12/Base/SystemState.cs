using System;
using System.Collections;
using System.IO;
using M12.Definitions;

namespace M12.Base
{
    public class SystemState
    {
        public SystemState(byte[] Data)
        {
            IsUnitBusy = new bool[GlobalDefinition.MAX_UNIT_SUPPORT];

            using (MemoryStream stream = new MemoryStream(Data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    OverallBusy = reader.ReadUInt16();

                    BitArray bits = new BitArray(new byte[] { Data[0], Data[1], Data[2] });

                    IsSystemBusy = bits.Get(0);
                    for (int i = 0; i < GlobalDefinition.MAX_UNIT_SUPPORT; i++)
                    {
                        IsUnitBusy[i] = bits.Get(i + 1);
                    }

                    IsEmergencyButtonPressed = bits.Get(16);
                    IsCSS1Detected = bits.Get(17);
                    IsCSS2Detected = bits.Get(18);
                   
                }
            }
        }

        public bool IsEmergencyButtonPressed { get; private set; }

        public UInt16 OverallBusy { get; private set; }

        public bool IsSystemBusy { get; private set; }

        public bool[] IsUnitBusy { get; private set; }

        public bool IsCSS1Detected { get; private set; }

        public bool IsCSS2Detected { get; private set; }

        public override string ToString()
        {
            return $"Overall Busy: 0x{OverallBusy.ToString("X")}, " +
                $"System Busy: {IsSystemBusy}, " +
                $"Emergency: {IsEmergencyButtonPressed}, " +
                $"CSS1: {IsCSS1Detected}, " +
                $"CSS2: {IsCSS2Detected}";
        }
    }
}
