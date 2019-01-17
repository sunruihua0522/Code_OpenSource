using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irixi_Aligner_Common.Equipments.Instruments
{
    sealed class Keithliey2400ProgressReportArgs : EventArgs
    {
        public Keithliey2400ProgressReportArgs()
        {
            IsMeasOverRange = false;
            IsInRangeCompliance = false;
            MeasurementValue = -999;
        }

        public bool IsMeasOverRange { get; set; }
        public bool IsInRangeCompliance { get; set; }
        public double MeasurementValue { get; set; }
    }
}
