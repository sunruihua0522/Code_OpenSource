using Irixi_Aligner_Common.Alignment.SnakeRouteScan;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.ViewModel.Base;

namespace Irixi_Aligner_Common.ViewModel.Alignment
{
    public class ViewSnakeRouteScan : ViewBaseAlignment<SnakeRouteScanArgsProfile>
    {
        public override IAlignmentHandler Handler => Service.SnakeRouteScanHandler;

        public override IAlignmentArgs Arguments => Service.SnakeRouteScanHandler.Args;
    }
}
