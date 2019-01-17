using Irixi_Aligner_Common.Alignment.FastRotatingScan;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.ViewModel.Base;

namespace Irixi_Aligner_Common.ViewModel.Alignment
{
    public class ViewFastRotatingScan : ViewBaseAlignment<FastRotatingScanArgsProfile>
    {
        public override IAlignmentHandler Handler => Service.FastRotatingScanHandler;

        public override IAlignmentArgs Arguments => Service.FastRotatingScanHandler.Args;
    }
}