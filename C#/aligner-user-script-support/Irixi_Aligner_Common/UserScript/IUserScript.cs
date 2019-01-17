using Irixi_Aligner_Common.Classes;
using System.Runtime.Serialization;

namespace Irixi_Aligner_Common.UserScript
{
    public interface IUserScript : ISerializable
    {
        string Name { get; }

        string Usage { get; }

        int Order { get; set; }

        SystemService Service { get; set; }

        string Summary { get; }

        UserScriptExecStatus ExecStatus { get; }

        bool IsError { get; }

        string ErrorMessage { get; }
        
        void RecoverReferenceTypeProperties();

        void Perform();

        void Validate();

        void Reset();
    }
}
