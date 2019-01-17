using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.Alignment.FastRotatingScan
{
    public class FastRotatingScanArgsProfile : AlignmentProfileBase
    {
        public string AxisRotating { get; set; }
        public string AxisLinear { get; set; }
        public double LinearInterval { get; set; }
        public double LinearScanRange { get; set; }
        public double Pitch { get; set; }

        public int MoveSpeed { get; set; }

        public override void FromArgsInstance(IAlignmentArgs Args)
        {
            var targ = Args as FastRotatingScanArgs;

            this.AxisRotating = targ.AxisRotating.HashString;
            this.AxisLinear = targ.AxisLinear.HashString;
            this.LinearInterval = targ.LinearInterval;
            this.LinearScanRange = targ.LinearRange;
            this.Pitch = targ.Pitch;
            
            this.HashString = this.GetHashString();
        }

        public override void ToArgsInstance(SystemService Service, IAlignmentArgs Args)
        {
            var targ = Args as FastRotatingScanArgs;

            targ.AxisRotating = Service.FindLogicalAxisByHashString(this.AxisRotating);
            targ.AxisLinear = Service.FindLogicalAxisByHashString(this.AxisLinear);
            targ.LinearInterval = this.LinearInterval;
            targ.LinearRange = this.LinearScanRange;
            targ.Pitch = this.Pitch;
        }
    }
}
