using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Classes.CustomizedException;
using Irixi_Aligner_Common.Equipments.Equipments;
using Irixi_Aligner_Common.Equipments.Equipments.Instruments;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Irixi_Aligner_Common.UserScript.Implementation
{
    [Serializable]
    public class UserScriptSwitchIPMRange : UserScriptBase
    {
        #region Variables

        string _ipmHashString;
        InternalPowerMeter _ipm = null;
        PDFrontEndRange _range = PDFrontEndRange.RANGE1;

        #endregion

        #region Constructors

        protected override string ChildCreateSummary()
        {
            if (IsError)
                return "";
            else
                return $"{PowerMeter.Config.Caption}, {FrontEndRange}";
        }

        public UserScriptSwitchIPMRange()
        {
            CreatePropertiesAllowTemplated();
        }

        public UserScriptSwitchIPMRange(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _ipmHashString = (string)info.GetValue("PowerMeter", typeof(string));
            
            this.FrontEndRange = (PDFrontEndRange)info.GetValue("FrontEndRange", typeof(PDFrontEndRange));
        }

        #endregion  

        #region Properties

        public override string Name => "IPM Range";

        public override string Usage => "Switch the range of the internal powermeter";

        private string PowerMeterHashString { get; set; }

        [DisplayFormat(NullDisplayText = "Powermeter not set"), DisplayName("Power Meter")]
        public InternalPowerMeter PowerMeter
        {
            get
            {
                return _ipm;
            }
            set
            {
                UpdateProperty(ref _ipm, value);
            }
        }

        [DisplayName("Range")]
        public PDFrontEndRange FrontEndRange
        {
            get
            {
                return _range;
            }
            set
            {
                UpdateProperty(ref _range, value);
            }
        }

        #endregion

        #region Methods

        protected override void CreatePropertiesAllowTemplated()
        {
            PropertiesAllowTemplated.Add(new Property("PowerMeter"));
            PropertiesAllowTemplated.Add(new Property("PDFrontEndRange"));
        }

        public override void RecoverReferenceTypeProperties()
        {

            try
            {
                this.PowerMeter = Service.FindInstrumentByHashString(_ipmHashString) as InternalPowerMeter;
            }
            catch
            {
                this.PowerMeter = null;
                ErrorMessage = $"Unable to find the Internal Powermeter with hash string {_ipmHashString}.";
            }

            base.RecoverReferenceTypeProperties();
        }

        protected override void ChildPerform()
        {
            this.PowerMeter.FrontEnd.SwitchRange(this.FrontEndRange);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("PowerMeter", PowerMeter.HashString, typeof(string));
            info.AddValue("FrontEndRange", FrontEndRange, typeof(PDFrontEndRange));
        }
        
        public override void Validate()
        {
            if (this.PowerMeter == null)
                ErrorMessage = "No powermeter selected.";
            //else if (this.PowerMeter.IsInitialized == false)
            //    ErrorMessage = "The powermeter is not initialized.";
            else
                ErrorMessage = MSG_PASSED;
        }

        #endregion
    }
}
