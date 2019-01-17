using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Equipments.Base;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Base;

namespace Irixi_Aligner_Common.Alignment.RotatingProfile
{
    public class RotatingScanArgs : AlignmentArgsBase
    {
        #region Variables

        const string PROP_GRP_ROTATING = "Rotating Settings";
        const string PROP_GRP_LINEAR = "Linear Settings";


        LogicalAxis axisRotating = null, axisLinear = null;
        //double targetPositionDifferentialOfMaxPower = 5, targetPosDiffChangeRate = 10, gapRotating = 1;
        double _linearInterval = 1, _linearRange = 100, _pitch = 750 * 3;

        //2 channel should be detected at the same time, so we need 2 keithley2400s
        IInstrument instrument, instrument2;

        #endregion

        #region Constructors
        
        public RotatingScanArgs() :base()
        {
            ScanCurve = new ScanCurve();
            ScanCurve2 = new ScanCurve();

            // add the curves to the group
            ScanCurveGroup.Add(ScanCurve);
            ScanCurveGroup.Add(ScanCurve2);
            ScanCurveGroup.Add(ScanCurve.FittingCurve);
            ScanCurveGroup.Add(ScanCurve2.FittingCurve);
            ScanCurveGroup.Add(ScanCurve.MaxPowerConstantLine);
            ScanCurveGroup.Add(ScanCurve2.MaxPowerConstantLine);

            Properties.Add(new Property("AxisRotating"));
            Properties.Add(new Property("Instrument"));
            Properties.Add(new Property("AxisLinear"));
            Properties.Add(new Property("Instrument2"));
            Properties.Add(new Property("LinearInterval"));
            Properties.Add(new Property("LinearRestriction"));
            Properties.Add(new Property("LengthOfChannelStartToEnd"));
            Properties.Add(new Property("MoveSpeed"));

            AxisXTitle = "ΔPosition";
            AxisYTitle = "Intensity";
        }

        #endregion

        #region Properties

        public override string PresetProfileFolder
        {
            get
            {
                return "RotatingScan";
            }
        }

        public override IInstrument Instrument
        {
            get => instrument;
            set
            {
                UpdateProperty(ref instrument, value);

                ScanCurve.DisplayName = ((InstrumentBase)instrument).Config.Caption;
            }
        }

        /// <summary>
        /// The instrument to monitor the secondary feedback channel
        /// </summary>
        [Display(Name = "Instrument 2", 
            GroupName = PROP_GRP_COMMON, 
            Description = "The feedback instrument of the last channel")]
        public IInstrument Instrument2
        {
            get => instrument2;
            set
            {
                UpdateProperty(ref instrument2, value);

                ScanCurve2.DisplayName = ((InstrumentBase)instrument2).Config.Caption;
            }
        }

        [Display(
            Name = "Axis", 
            GroupName = PROP_GRP_ROTATING, 
            Description = "")]
        public LogicalAxis AxisRotating
        {
            get => axisRotating;
            set
            {
                UpdateProperty(ref axisRotating, value);
            }
        }

        [Display(Name = "Axis", GroupName = PROP_GRP_LINEAR, Description = "")]
        public LogicalAxis AxisLinear
        {
            get => axisLinear;
            set
            {
                UpdateProperty(ref axisLinear, value);
            }
        }

        [Display(Name = "Interval", GroupName = PROP_GRP_LINEAR, Description = "")]
        public double LinearInterval
        {
            get => _linearInterval;
            set
            {
                UpdateProperty(ref _linearInterval, value);
            }
        }


        [Display(Name = "Range", GroupName = PROP_GRP_LINEAR, Description = "")]
        public double LinearRestriction
        {
            get => _linearRange;
            set
            {
                UpdateProperty(ref _linearRange, value);
            }
        }

        [Display(Name = "Pitch", GroupName = PROP_GRP_COMMON, Description = "The pitch of the two channels used to scan.")]
        public double Pitch
        {
            get => _pitch;
            set
            {
                UpdateProperty(ref _pitch, value);
            }
        }

        /// <summary>
        /// Scan curve of instrument
        /// </summary>
        [Browsable(false)]
        public ScanCurve ScanCurve { private set; get; }
        
        /// <summary>
        /// Scan curve of instrument2
        /// </summary>
        [Browsable(false)]
        public ScanCurve ScanCurve2 { private set; get; }

        #endregion

        #region Methods
        public override void Validate()
        {
            if(AxisLinear == null)
                throw new ArgumentException("you must specify the linear axis.");

            if (AxisRotating == null)
                throw new ArgumentException("you must specify the rotating axis.");

            if (AxisLinear == AxisRotating)
                throw new ArgumentException("linear axis and rotating axis must be different.");

            if (MoveSpeed < 1 || MoveSpeed > 100)
                throw new ArgumentException("move speed must be between 1 ~ 100");

            if (Instrument == null)
                throw new ArgumentException(string.Format("you must specify the {0}",
                    ((DisplayAttribute)TypeDescriptor.GetProperties(this)["Instrument"].Attributes[typeof(DisplayAttribute)]).Name) ?? "instrument");

            if (Instrument2 == null)
                throw new ArgumentException(string.Format("you must specify the {0}",
                    ((DisplayAttribute)TypeDescriptor.GetProperties(this)["Instrument2"].Attributes[typeof(DisplayAttribute)]).Name) ?? "instrument2");

            if (Instrument == Instrument2)
                throw new ArgumentException("the two instruments must be different.");
        }

        #endregion
    }
}
