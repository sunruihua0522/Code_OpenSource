using Irixi_Aligner_Common.Alignment;
using Irixi_Aligner_Common.Alignment.SpiralScan;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Classes.CustomizedException;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Irixi_Aligner_Common.UserScript.Implementation
{
    [Serializable]
    public class UserScriptBlindSearch : UserScriptBase
    {
        #region Variables

        string _profile = "";

        #endregion

        #region Constructors

        public UserScriptBlindSearch() : base()
        {
            
        }

        public UserScriptBlindSearch(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.BlindSearchPresetProfile = (string)info.GetValue("PresetProfile", typeof(string));
        }

        #endregion

        #region Properties

        public override string Name => "Blind Search";

        public override string Usage => "Run the blind-search with the specified preset profile";

        [DisplayName("Preset Profile")]
        public string BlindSearchPresetProfile
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

        [Browsable(false)]
        public string[] ProfileList
        {
            get
            {
                return AlignmentProfileManager.LoadProfileList(Service.SpiralScanHandler.Args);
            }
        }

        #endregion

        #region Methods

        protected override string ChildCreateSummary()
        {
            if (IsError)
                return "";
            else
                return BlindSearchPresetProfile;
        }

        protected override void CreatePropertiesAllowTemplated()
        {
            this.PropertiesAllowTemplated.Add(new Property("BlindSearchPresetProfile"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PresetProfile", BlindSearchPresetProfile, typeof(string));
        }

        protected override void ChildPerform()
        {
            var profile = AlignmentProfileManager.Load<SpiralScanArgsProfile>(Service.SpiralScanHandler.Args, BlindSearchPresetProfile);
            profile.ToArgsInstance(Service, Service.SpiralScanHandler.Args);
            Service.SpiralScanHandler.Start();
        }

        public override void Validate()
        {
            if (BlindSearchPresetProfile == null || BlindSearchPresetProfile == "")
            {
                ErrorMessage = "No profile selected.";
            }
            else
            {
                ErrorMessage = MSG_PASSED;
            }

        }
        
        #endregion
    }
}
