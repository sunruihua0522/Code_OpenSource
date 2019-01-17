using M12.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M12.Commands.Alignment
{
    public class BlindSearchArgs
    {
        #region Constructors

        public BlindSearchArgs()
        {

        }

        /// <summary>
        /// Create the argumnts of the blind-search.
        /// </summary>
        /// <param name="Unit">active unit</param>
        /// <param name="Range">scan range</param>
        /// <param name="Gap">spiral curve gap</param>
        /// <param name="Interval">ADC sampling interval</param>
        /// <param name="speed">move speed</param>
        public BlindSearchArgs(UnitID Unit, uint Range, uint Gap, ushort Interval, byte Speed)
        {
            this.Unit = Unit;
            this.Range = Range;
            this.Gap = Gap;
            this.Interval = Interval;
            this.Speed = Speed;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the active unit.
        /// </summary>
        public UnitID Unit { get; set; }

        /// <summary>
        /// Get or set the scan range.
        /// </summary>
        public uint Range { get; set; }

        /// <summary>
        /// Get or set the gap of the spiral curve.
        /// </summary>
        public uint Gap { get; set; }

        /// <summary>
        /// Get or set the interval of the ADC sampling trigger signal.
        /// </summary>
        public ushort Interval { get; set; }

        /// <summary>
        /// Get or set the move speed.
        /// </summary>
        public byte Speed { get; set; }

        #endregion
    }
}
