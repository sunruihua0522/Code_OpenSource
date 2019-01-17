using Irixi_Aligner_Common.Alignment.RotatingProfile;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.ViewModel.Base;

namespace Irixi_Aligner_Common.ViewModel.Alignment
{
    public class ViewRotatingProfile : ViewBaseAlignment<RotatingScanArgsProfile>
    {
        public override IAlignmentHandler Handler => Service.RotatingScanHandler;

        public override IAlignmentArgs Arguments => Service.RotatingScanHandler.Args;
    }
}