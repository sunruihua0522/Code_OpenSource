using Irixi_Aligner_Common.Configuration.Equipments;
using Irixi_Aligner_Common.Equipments.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using M12.Definitions;
using System;
using System.Threading.Tasks;

namespace Irixi_Aligner_Common.Equipments.Equipments.Instruments
{
    public class ContactSensor : InstrumentBase
    {
        #region Variables

        double _force = 0;

        #endregion

        #region Constructors

        public ContactSensor(ConfigurationContactSensor Config, IrixiM12 ControllerAttached):base(Config)
        {
            this.Controller = ControllerAttached;

            if (Enum.TryParse<ADCChannels>(Config.AttachedADCChannel, out ADCChannels ch))
            {
                AttachedADCChannel = ch;

                // once the analog input value updated in the background task, this event of M12 will be raised to 
                // ask the subscriber to udpate the corresponding properties.
                Controller.OnAnalogInputValueUpdated += (s, e) =>
                {
                    if (e.InputChannel == AttachedADCChannel)
                    {
                        this.Force = Math.Round(e.AnalogValue, 0);
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

        public IrixiM12 Controller { get; private set; }

        public ADCChannels AttachedADCChannel { get; }

        /// <summary>
        /// Get the force of the contact sensor connected to the AN1 of M12.
        /// </summary>
        public double Force
        {
            get
            {
                return _force;
            }
            set
            {
                UpdateProperty(ref _force, value);
            }
        }

        #endregion

        #region Public Methods

        public override bool Init()
        {
            if (this.IsEnabled)
            {
                if (this.Controller.IsInitialized)
                {
                    this.IsInitialized = true;
                    return true;
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
            return Controller.ReadAN(AttachedADCChannel)[0];
        }
        #endregion
    }
}
