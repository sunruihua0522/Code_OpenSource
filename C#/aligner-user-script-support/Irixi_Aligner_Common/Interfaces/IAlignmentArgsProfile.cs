using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;

namespace Irixi_Aligner_Common.Interfaces
{
    public interface IAlignmentArgsProfile
    {

        /// <summary>
        /// Set the hash string which had been saved in the profile.
        /// This is used to validate the profile after it is loaded from the saved file.
        /// </summary>
        string HashString { set; get; }

        #region Methods

        void FromArgsInstance(IAlignmentArgs Args);

        void ToArgsInstance(SystemService Service, IAlignmentArgs Args);

        string GetHashString();

        bool Validate();

        #endregion
    }
}
