using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.Alignment.SpiralScan
{
    public class SpiralScanArgsProfile : AlignmentProfileBase
    {
        public string Axis { get; set; }
        public string Axis2 { get; set; }
        public double Interval { get; set; }
        public double Range { get; set; }

        public override void FromArgsInstance(IAlignmentArgs Args)
        {
            var targ = Args as SpiralScanArgs;
            this.Axis = targ.Axis.HashString;
            this.Axis2 = targ.Axis2.HashString;
            this.Interval = targ.Interval;
            this.Range = targ.Range;
            this.Speed = targ.MoveSpeed;
            this.HashString = this.GetHashString();
        }


        public override void ToArgsInstance(SystemService Service, IAlignmentArgs Args)
        {
            var targ = Args as SpiralScanArgs;

            targ.Axis = Service.FindLogicalAxisByHashString(this.Axis);
            targ.Axis2 = Service.FindLogicalAxisByHashString(this.Axis2);

            targ.Interval = this.Interval;
            targ.Range = this.Range;
            targ.MoveSpeed = this.Speed;
        }
    }
}
