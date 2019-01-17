using Irixi_Aligner_Common.Alignment.CentralAlign;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.ViewModel.Base;

namespace Irixi_Aligner_Common.ViewModel.Alignment
{
    public class ViewCentralAlign : ViewBaseAlignment<CentralAlignArgsProfile>
    {
        public override IAlignmentHandler Handler => Service.CentralAlignHandler;

        public override IAlignmentArgs Arguments => Service.CentralAlignHandler.Args;
    }
}
