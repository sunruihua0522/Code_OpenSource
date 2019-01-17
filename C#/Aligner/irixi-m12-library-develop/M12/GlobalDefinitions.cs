using System;

namespace M12.Definitions
{
    public enum UnitID
    { 
        ALL,
        U1,
        U2,
        U3,
        U4,
        U5,
        U6,
        U7,
        U8,
        U9,
        U10,
        U11,
        U12,
        INVALID = 0xFF
    }

    public enum CSSCH
    {
        CH1,
        CH2
    }

    public enum Errors
    {
        ERR_NONE,
        ERR_PARA,
        ERR_NOT_INIT,
        ERR_NOT_HOMED,
        ERR_BUSY,
        ERR_CWLS,
        ERR_CCWLS,
        ERR_EMERGENCY,
        ERR_USER_STOPPED,
        ERR_NO_STAGE_DETECTED,
        ERR_UNKNOWN,
        ERR_SYS_MCSU_ID = 0x80,
        ERR_SYS_CANBUS_RX,
        ERR_SYS_PARAM,
        ERR_SYS_CSS1_Triggered,
        ERR_SYS_CSS2_Triggered,
        ERR_SYS_BLINDSEARCH,

        // NOTE the following errors are only defined in PC side!
        ERR_TIMEOUT,
        ERR_OPERATION_CANCELLED,
    }

    public enum DigitalOutput
    {
        DOUT1 = 1,
        DOUT2,
        DOUT3,
        DOUT4,
        DOUT5,
        DOUT6,
        DOUT7,
        DOUT8,
    }

    public enum DigitalIOStatus
    {
        OFF,
        ON,
    }

    [Flags]
    public enum ADCChannels
    {
        CH1 = (1 << 0),
        CH2 = (1 << 1),
        CH3 = (1 << 2),
        CH4 = (1 << 3),
        CH5 = (1 << 4),
        CH6 = (1 << 5),
        CH7 = (1 << 6),
        CH8 = (1 << 7),
    }

    public class GlobalDefinition
    {
        /// <summary>
        /// Reference voltage of the ADC in mV.
        /// </summary>
        public const double ADCVref = 5000.0;

        public const ushort MAX_ACC_STEPS = 3000;

        public const int MAX_UNIT_SUPPORT = 12;


        /// <summary>
        /// Calculate how many "1"s in the specified number.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int NumberOfSetBits(int i)
        {
            // Java: use >>> instead of >>
            // C or C++: use uint32_t
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
    }
}
