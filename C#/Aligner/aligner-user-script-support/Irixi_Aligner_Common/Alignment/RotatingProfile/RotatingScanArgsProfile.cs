using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.Alignment.RotatingProfile
{
    public class RotatingScanArgsProfile : AlignmentProfileBase
    {
        public string Instrument { get; set; }
        public string Instrument2 { get; set; }
        public string AxisRotating { get; set; }
        public string AxisLinear { get; set; }
        public double LinearInterval { get; set; }
        public double LinearScanRange { get; set; }
        public double Pitch { get; set; }

        public int MoveSpeed { get; set; }

        public override void FromArgsInstance(IAlignmentArgs Args)
        {
            var targ = Args as RotatingScanArgs;

            this.AxisRotating = targ.AxisRotating.HashString;
            this.AxisLinear = targ.AxisLinear.HashString;
            this.LinearInterval = targ.LinearInterval;
            this.LinearScanRange = targ.LinearRestriction;
            this.Pitch = targ.Pitch;

            this.Instrument = targ.Instrument.HashString;
            this.Instrument2 = targ.Instrument2.HashString;

            this.HashString = this.GetHashString();
        }

        public override void ToArgsInstance(SystemService Service, IAlignmentArgs Args)
        {
            var targ = Args as RotatingScanArgs;

            targ.AxisRotating = Service.FindLogicalAxisByHashString(this.AxisRotating);
            targ.AxisLinear = Service.FindLogicalAxisByHashString(this.AxisLinear);
            targ.LinearInterval = this.LinearInterval;
            targ.LinearRestriction = this.LinearScanRange;
            targ.Pitch = this.Pitch;
           

            targ.Instrument = Service.FindInstrumentByHashString(this.Instrument);
            targ.Instrument2 = Service.FindInstrumentByHashString(this.Instrument2);
        }
    }
}
