using System;

namespace M12.Excpections
{
    /// <summary>
    /// <para>This exception is especially used in the fast alignement handler.</para>
    /// <para>It indicates that one or more ADC sampling points missed while scan since the trigger signal generates too fast.</para>
    /// <para>Try to reduce the scan speed or increase the value of sampling interval.</para>
    /// </summary>
    public class ADCSamplingPointMissException : Exception
    {
        public ADCSamplingPointMissException(int Desired, int Reality)
        {
            this.Desired = Desired;
            this.Reality = Reality;
        }
        
        /// <summary>
        /// How many sampling points desired.
        /// </summary>
        public int Desired { get; }

        /// <summary>
        /// How many sampling points actually get.
        /// </summary>
        public int Reality { get; }
    }
}
