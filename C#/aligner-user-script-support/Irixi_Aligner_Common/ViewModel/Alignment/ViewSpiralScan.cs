using Irixi_Aligner_Common.Alignment.SpiralScan;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.ViewModel.Base;

namespace Irixi_Aligner_Common.ViewModel.Alignment
{
    public class ViewSpiralScan : ViewBaseAlignment<SpiralScanArgsProfile>
    {
        public override IAlignmentHandler Handler => Service.SpiralScanHandler;

        public override IAlignmentArgs Arguments => Service.SpiralScanHandler.Args;
    }
}
