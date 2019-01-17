using Irixi_Aligner_Common.Configuration.Equipments;
using Irixi_Aligner_Common.Equipments.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using M12.Definitions;
using System;

namespace Irixi_Aligner_Common.Equipments.Equipments.Instruments
{
    public class InternalPowerMeter : InstrumentBase
    {
        #region Variables

        double _power = 0;

        #endregion

        #region Constructors

        public InternalPowerMeter(ConfigurationInternalPowerMeter Config, IrixiM12 ControllerAttached, PDFrontEnd FrontEnd) : base(Config)
        {
            this.Controller = ControllerAttached;
            this.FrontEnd = FrontEnd;
            this.BackgroundNoise = Config.BackgroundNoise;

            if(Enum.TryParse<ADCChannels>(Config.AttachedADCChannel, out ADCChannels ch))
            {
                this.AttachedADCChannel = ch;

                // once the analog input value updated in the background task, this event of M12 will be raised to 
                // ask the subscriber to udpate the corresponding properties.
                Controller.OnAnalogInputValueUpdated += (s, e) =>
                {
                    if(e.InputChannel == AttachedADCChannel)
                    {
                        // convert voltage to current.
                        double res = (int)FrontEnd.Range < 0 ? 1000000 : FrontEnd.Config.SamplingRes[(int)FrontEnd.Range - 1];
                        double voltage = e.AnalogValue - BackgroundNoise;
                        if (voltage < 0)
                            voltage = 0;
                        this.Power = Math.Round(voltage / res * 1000, 3);
                    }
                };
            }
            else
            {
                this.IsInitialized = false;
                throw new ArgumentException($"unable to parse the ADC Channel {Config.AttachedADCChannel}, please check the config file.");
            }
        }

        #endregion

        #region Properties

        private IrixiM12 Controller { get; }

        public ADCChannels AttachedADCChannel { get; }

        public double BackgroundNoise { get; }

        public PDFrontEnd FrontEnd { get; }

        public double Power
        {
            get
            {
                return _power;
            }
            set
            {
                UpdateProperty(ref _power, value);
            }
        }

        #endregion

        #region Methods

        public override bool Init()
        {
            if (this.IsEnabled)
            {
                if (this.Controller.IsInitialized)
                {
                    // initialize the PD front-end.
                    if (this.FrontEnd.Init())
                    {
                        this.IsInitialized = true;
                        return true;
                    }
                    else
                    {
                        this.LastError = FrontEnd.LastError;
                        return false;
                    }
                }
                else
                {
                    this.LastError = "the attached motion controller is not initialized.";
                    return false;
                }
            }
            else
            {
                this.LastError = "it's disabled.";
                return false;
            }
        }

        public override double Fetch()
        {
            return ReadVoltage(AttachedADCChannel);
        }

        /// <summary>
        /// Read the voltage of the force of the contact sensors.
        /// </summary>
        private double ReadVoltage(ADCChannels Ch)
        {
            try
            {
                var volts = Controller.ReadAN(Ch);
                return volts[0];
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        #endregion
    }
}
