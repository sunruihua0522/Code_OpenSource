using System;
using System.Collections.Generic;
using System.Linq;
using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.Alignment.ProfileND
{
    public class ProfileNDArgsProfile : AlignmentProfileBase
    {
        public string Instrument { get; set; }
        public string SelectedAligner { get; set; }
        public double Target { get; set; }
        public int MaxCycles { get; set; }
        public int MaxOrder { get; set; }
        public int[] ListScanOrder { get; set; }

        [HashIgnore]
        public Profile1DArgsProfile[] AxisGroup { get; set; }

       
        public override void FromArgsInstance(IAlignmentArgs Args)
        {
            var targ = Args as ProfileNDArgs;

            this.Instrument = targ.Instrument.HashString;
            this.SelectedAligner = targ.SelectedAligner.HashString;
            this.Target = targ.Target;
            this.MaxCycles = targ.MaxCycles;
            this.MaxOrder = targ.MaxOrder;
            this.ListScanOrder = targ.ListScanOrder.ToArray();

            List<Profile1DArgsProfile> subaxis = new List<Profile1DArgsProfile>();
            foreach(var item in targ.AxisParamCollection)
            {
                var profile = new Profile1DArgsProfile();
                profile.FromArgsInstance(item);
                subaxis.Add(profile);
            }
            this.AxisGroup = subaxis.ToArray();

            this.HashString = this.GetHashString();
        }

        public override void ToArgsInstance(SystemService Service, IAlignmentArgs Args)
        {
            var targ = Args as ProfileNDArgs;
            targ.Instrument = Service.FindInstrumentByHashString(this.Instrument);
            targ.SelectedAligner = Service.FindLogicalMotionComponentByHashString(this.SelectedAligner);
            targ.Target = this.Target;
            targ.MaxCycles = this.MaxCycles;

            foreach(var targ1d in targ.AxisParamCollection)
            {
                var axishash = targ1d.Axis.HashString;
                var profile = this.AxisGroup.Where(item => item.Axis == axishash).Select(item => { return item; }).First();
                profile.ToArgsInstance(null, targ1d);
            }
        }

        public override string GetHashString()
        {
            var str = base.GetHashString();
            
            // Note that hash string of each array element must be read manually and joined as a whole hash string
            str += string.Join(",", AxisGroup.Select(axis => 
            {
                return axis.HashString;

            }).ToArray());
            return HashGenerator.GetHashSHA256(str);
        }
   
    }
}
