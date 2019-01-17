using Irixi_Aligner_Common.Configuration.Equipments;
using Irixi_Aligner_Common.Equipments.Base;
using System;
using System.ComponentModel;
using System.IO.Ports;

namespace Irixi_Aligner_Common.Equipments.Equipments
{
    public enum PDFrontEndRange
    {
        [Description("Range 1")]
        RANGE1 = 1,
        [Description("Range 2")]
        RANGE2,
        [Description("Range 3")]
        RANGE3,
        [Description("Range 4")]
        RANGE4,
        [Description("Range 5")]
        RANGE5,
        [Description("Range 6")]
        RANGE6
    }

    public class PDFrontEnd : InstrumentBase
    {
        #region Variables

        SerialPort _port;
        PDFrontEndRange _range = PDFrontEndRange.RANGE1;

        #endregion

        #region Constructors

        public PDFrontEnd(ConfigurationPDFrontEnd Config) : base(Config)
        {
            this.Config = Config;
        }

        #endregion

        #region Properties

        public new ConfigurationPDFrontEnd Config { get; }

        public PDFrontEndRange Range
        {
            get
            {
                return _range;
            }
            set
            {
                try
                {
                    SwitchRange(value);
                    UpdateProperty(ref _range, value);
                }
                catch(Exception ex)
                {
                    // if it's failed to switch range, keep the selected item being the previous one in the combobox.

                    var oldRange = _range;
                    var tmpRange = _range > PDFrontEndRange.RANGE1 ? PDFrontEndRange.RANGE1 : PDFrontEndRange.RANGE2;
                    
                    UpdateProperty(ref _range, tmpRange);
                    UpdateProperty(ref _range, oldRange);
                }
               
            }
        }

        #endregion

        #region Methods
        
        public override bool Init()
        {
            _port = new SerialPort(Config.Port, Config.BaudRate, Parity.None, 8, StopBits.One);

            if (IsEnabled)
            {
                try
                {
                    _port.Open();
                    this.IsInitialized = true;
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = $"unable to open the serial port {Config.Port}, {ex.Message}";
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Switch range.
        /// </summary>
        /// <param name="Range">Must be 1 to 6</param>
        public void SwitchRange(PDFrontEndRange Range)
        {
            if (IsEnabled)
            {
                if (Range < PDFrontEndRange.RANGE1 || Range > PDFrontEndRange.RANGE6)
                    throw new ArgumentOutOfRangeException("The range must be 1 to 6.");

                _port.WriteLine($"RANGE {Range.ToString()}\r\n");
            }
            else
            {
                throw new Exception("the PD Front-End is disabled.");
            }
        }
        #endregion
    }
}
