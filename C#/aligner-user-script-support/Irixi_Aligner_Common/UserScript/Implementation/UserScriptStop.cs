using System;
using System.Runtime.Serialization;

namespace Irixi_Aligner_Common.UserScript.Implementation
{
    [Serializable]
    public class UserScriptStop : UserScriptBase
    {
        #region Constructors

        public UserScriptStop() : base() { }

        public UserScriptStop(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region Properties

        public override string Name => "Sequence Stop";

        public override string Usage => "Stop the user script";

        #endregion

        #region Methods

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override void Validate()
        {
            ErrorMessage = MSG_PASSED;
        }

        protected override void ChildPerform()
        {
            
        }
        #endregion
    }
}
