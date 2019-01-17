using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Irixi_Aligner_Common.UserScript.Implementation
{
    [Serializable]
    public class UserScriptMoveToPresetPosition : UserScriptBase
    {
        #region Variables

        LogicalMotionComponent _aligner;
        string _profile = "";
        string _alignerHashString;

        #endregion

        #region Constructors

        public UserScriptMoveToPresetPosition()
        {
            this.ProfileManager = new PositionPresetManager();
        }

        public UserScriptMoveToPresetPosition(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _alignerHashString = (string)info.GetValue("Aligner", typeof(string));
            PositionPresetProfile = (string)info.GetValue("PresetProfile", typeof(string));
        }

        #endregion  

        #region Properties

        [Browsable(false), ReadOnly(true)]
        public PositionPresetManager ProfileManager { get; private set; }

        public override string Name => "Preset Position";

        public override string Usage => "Move the specified component to the preset position with the specified profile";

        public LogicalMotionComponent Aligner
        {
            get
            {
                return _aligner;
            }
            set
            {
                if (_aligner != value)
                    this.PositionPresetProfile = "";

                ProfileManager.SelectedMotionComponent = value;

                UpdateProperty(ref _aligner, value);
                
            }
        }

        [DisplayName("Preset Profile")]
        public string PositionPresetProfile
        {
            get
            {
                return _profile;
            }
            set
            {
                UpdateProperty(ref _profile, value);
            }
        }

        #endregion

        #region Methods

        public override void RecoverReferenceTypeProperties()
        {
            this.ProfileManager = new PositionPresetManager();

            // protect profile since it will be cleaned once the aligner set.
            var profile = this.PositionPresetProfile;

            try
            {
                Aligner = Service.FindLogicalMotionComponentByHashString(_alignerHashString);
            }
            catch
            {
                IsError = true;
                ErrorMessage = $"Unable to find the aligner with hash string {_alignerHashString}.";
            }

            // recover the profile
            this.PositionPresetProfile = profile;
            

            base.RecoverReferenceTypeProperties();
        }

        protected override string ChildCreateSummary()
        {
            if (IsError)
                return "";
            else
                return $"{Aligner}, {PositionPresetProfile}";
        }

        protected override void CreatePropertiesAllowTemplated()
        {
            this.PropertiesAllowTemplated.Add(new Classes.Base.Property("Aligner"));
            this.PropertiesAllowTemplated.Add(new Classes.Base.Property("PositionPresetProfile"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Aligner", Aligner.HashString, typeof(string));
            info.AddValue("PresetProfile", PositionPresetProfile, typeof(string));
        }

        protected override void ChildPerform()
        {
            
        }

        public override void Validate()
        {
            if (Aligner == null)
                ErrorMessage = "No aligner selected.";
            else if (PositionPresetProfile == null || PositionPresetProfile == "")
                ErrorMessage = "No profile selected.";
            else
                ErrorMessage = MSG_PASSED;
        }

        #endregion
    }
}
