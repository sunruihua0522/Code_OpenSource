using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Equipments.Base;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Base;

namespace Irixi_Aligner_Common.Alignment.FastRotatingScan
{
    public class FastRotatingScanArgs : AlignmentArgsBase
    {
        #region Variables

        const string PROP_GRP_ROTATING = "Rotating Settings";
        const string PROP_GRP_LINEAR = "Linear Settings";


        LogicalAxis axisRotating = null, axisLinear = null;
        //double targetPositionDifferentialOfMaxPower = 5, targetPosDiffChangeRate = 10, gapRotating = 1;
        double _linearInterval = 1, _linearRange = 100, _pitch = 750 * 3;

        #endregion

        #region Constructors
        
        public FastRotatingScanArgs() :base()
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
            Properties.Add(new Property("AxisLinear"));
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
                return "FastRotatingScan";
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
        public double LinearRange
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
        }

        #endregion
    }
}
