using Irixi_Aligner_Common.Alignment;
using Irixi_Aligner_Common.Alignment.FastCentralAlign;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.ViewModel.Base;

namespace Irixi_Aligner_Common.ViewModel.Alignment
{
    public class ViewFastCentralAlign : ViewBaseAlignment<FastCentralAlignArgsProfile>
    {
        public override IAlignmentHandler Handler => Service.FastCentralAlignHandler;

        public override IAlignmentArgs Arguments => Service.FastCentralAlignHandler.Args;
    }
}
