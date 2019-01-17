using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.Alignment.CentralAlign
{
    public class CentralAlignArgsProfile : AlignmentProfileBase
    {
        public string HorizontalAxis { get; set; }
        public double HorizontalInterval { get; set; }
        public double HorizontalRange { get; set; }

        public string VerticalAxis { get; set; }
        public double VerticalInterval { get; set; }
        public double VerticalRange { get; set; }

        public string Instrument { get; set; }
        public string Instrument2 { get; set; }

        public int MoveSpeed { get; set; }

        public override void FromArgsInstance(IAlignmentArgs Args)
        {
            var targ = Args as CentralAlignArgs;

            this.HorizontalAxis = targ.Axis.HashString;
            this.HorizontalInterval = targ.HorizonalRange;
            this.HorizontalRange = targ.HorizonalInterval;
            this.VerticalAxis = targ.Axis2.HashString;
            this.VerticalInterval = targ.VerticalRange;
            this.VerticalRange = targ.VerticalInterval;

            this.Instrument = targ.Instrument.HashString;
            this.Instrument2 = targ.Instrument2.HashString;

            this.MoveSpeed = targ.MoveSpeed;

            this.HashString = this.GetHashString();
        }

        public override void ToArgsInstance(SystemService Service, IAlignmentArgs Args)
        {
            var targ = Args as CentralAlignArgs;

            targ.Axis = Service.FindLogicalAxisByHashString(this.HorizontalAxis);
            targ.HorizonalRange = this.HorizontalRange;
            targ.HorizonalInterval = this.HorizontalInterval;

            targ.Axis2 = Service.FindLogicalAxisByHashString(this.VerticalAxis);
            targ.VerticalRange = this.VerticalRange;
            targ.VerticalInterval = this.VerticalInterval;

            targ.Instrument = Service.FindInstrumentByHashString(this.Instrument);
            targ.Instrument2 = Service.FindInstrumentByHashString(this.Instrument2);

            targ.MoveSpeed = this.MoveSpeed;
        }
    }
}
