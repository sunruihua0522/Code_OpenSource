using Irixi_Aligner_Common.UserScript;

namespace Irixi_Aligner_Common.Classes.CustomizedException
{
    public class UserScriptValidateException : System.Exception
    {
        public UserScriptValidateException(IUserScript ScriptItem, string Message) : base(Message)
        {
            this.ScriptItem = ScriptItem;
        }

        public IUserScript ScriptItem { get; }
    }
}
