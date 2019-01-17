using Irixi_Aligner_Common.Alignment.FastND;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.ViewModel.Base;

namespace Irixi_Aligner_Common.ViewModel.Alignment
{
    public class ViewFastND : ViewBaseAlignment<FastNDArgsProfile>
    {
        public override IAlignmentHandler Handler => Service.FastNDHandler;

        public override IAlignmentArgs Arguments => Service.FastNDHandler.Args;
    }
}
