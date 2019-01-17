using Irixi_Aligner_Common.Classes.Base;

namespace Irixi_Aligner_Common.Interfaces
{
    /// <summary>
    /// The IServiceSystem make it allow this object can be added to the busy component list in the system service class.
    /// </summary>
    public interface IAlignmentHandler : IStoppable
    {

        #region Properties
        
        IAlignmentArgs Args { get; }


        ObservableCollectionThreadSafe<string> Log { get; }

        #endregion

        #region Methods

        void Start();
        
        #endregion


    }
}
