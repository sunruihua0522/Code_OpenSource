namespace Irixi_Aligner_Common.Classes.Base
{
    public class AxisCruiseArgs
    {
        public enum CruiseSpeed
        {
            FAST,
            MID,
            SLOW
        }

        public enum CruiseDirection
        {
            CW,
            CCW
        }

        public CruiseSpeed Speed { get; set; }

        public CruiseDirection Direction { get; set; }
    }
}
