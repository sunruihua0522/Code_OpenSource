using System;
using System.Threading.Tasks;

namespace Irixi_Aligner_Common.Interfaces
{
    public interface IInstrument : IEquipment
    {
        #region Properties

        bool IsMultiChannel { get; }

        int ActivedChannel { get; }

        #endregion

        #region Methods

        string GetDescription();

        void Reset();

        double Fetch();
        
        double Fetch(int Channel);

        double[] FetchAll();

        void StartAutoFetching();

        void StopAutoFetching();

        void PauseAutoFetching();

        void ResumeAutoFetching();

        #endregion
    }
}
