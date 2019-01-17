using Irixi_Aligner_Common.Alignment.ProfileND;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.ViewModel.Base;

namespace Irixi_Aligner_Common.ViewModel.Alignment
{
    public class ViewProfileND : ViewBaseAlignment<ProfileNDArgsProfile>
    {
        public override IAlignmentHandler Handler => Service.ProfileNDHandler;

        public override IAlignmentArgs Arguments => Service.ProfileNDHandler.Args;
    }
}
