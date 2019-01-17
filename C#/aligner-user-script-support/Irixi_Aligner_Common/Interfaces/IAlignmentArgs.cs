using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes.Base;

namespace Irixi_Aligner_Common.Interfaces
{
    public interface IAlignmentArgs
    {
        #region Properties

        string PresetProfileFolder { get; }
       
        #endregion

        #region Methods

        void Validate();

        void ClearScanCurve();

        string ExportScanCurveToCSV(string FileName);

        #endregion


    }
}
