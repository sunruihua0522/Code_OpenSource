using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.Alignment.SnakeRouteScan
{
    public class SnakeRouteScanArgsProfile : AlignmentProfileBase
    {
        public string Axis { get; set; }
        public string Axis2 { get; set; }
        public double ScanRange { get; set; }
        public double ScanRange2 { get; set; }
        public double Interval { get; set; }
        public int MoveSpeed { get; set; }


        public override void FromArgsInstance(IAlignmentArgs Args)
        {
            var targ = Args as SnakeRouteScanArgs;

            this.Axis = targ.Axis.HashString;
            this.Axis2 = targ.Axis2.HashString;
            this.ScanRange = targ.AxisRestriction;
            this.ScanRange2 = targ.Axis2Restriction;
            this.Interval = targ.ScanInterval;
            this.MoveSpeed = targ.MoveSpeed;

            this.HashString = this.GetHashString();
        }

        public override void ToArgsInstance(SystemService Service, IAlignmentArgs Args)
        {
            var targ = Args as SnakeRouteScanArgs;

            targ.Axis = Service.FindLogicalAxisByHashString(this.Axis);
            targ.Axis2 = Service.FindLogicalAxisByHashString(this.Axis2);
            targ.AxisRestriction = this.ScanRange;
            targ.Axis2Restriction = this.ScanRange2;
            targ.ScanInterval = this.Interval;
            targ.MoveSpeed = this.MoveSpeed;
        }
    }
}
