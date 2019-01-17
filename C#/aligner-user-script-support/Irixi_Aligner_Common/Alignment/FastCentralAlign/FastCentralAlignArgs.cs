using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Irixi_Aligner_Common.Alignment.FastCentralAlign
{
    public class FastCentralAlignArgs : AlignmentArgsBase
    {
        #region Variables
        const string PROP_GRP_H_AXIS = "Horizontal Settings";
        const string PROP_GRP_V_AXIS = "Vertical Settings";

        LogicalAxis axis, axis2;
        double hScanInterval = 1, vScanInterval = 1, axisRestriction = 100, axis2Restriction = 100;


        #endregion

        #region Constructors

        public FastCentralAlignArgs() : base()
        {
            ScanCurve = new ScanCurve();
            ScanCurve2 = new ScanCurve();
            ScanCurveGroup.Add(ScanCurve);
            ScanCurveGroup.Add(ScanCurve2);
            ScanCurveGroup.Add(ScanCurve.FittingCurve);
            ScanCurveGroup.Add(ScanCurve2.FittingCurve);
            ScanCurveGroup.Add(ScanCurve.MaxPowerConstantLine);
            ScanCurveGroup.Add(ScanCurve2.MaxPowerConstantLine);

            AxisXTitle = "Position";
            AxisYTitle = "Intensity";
            
            Properties.Add(new Property("MoveSpeed"));

            Properties.Add(new Property("Axis"));
            Properties.Add(new Property("HorizonalInterval"));
            Properties.Add(new Property("HorizonalRange"));
            Properties.Add(new Property("Axis2"));
            Properties.Add(new Property("VerticalInterval"));
            Properties.Add(new Property("VerticalRange"));
        }

        #endregion

        #region Properties

        public override string PresetProfileFolder
        {
            get
            {
                return "FastCentralAlign";
            }
        }

        [Browsable(false)]
        public ScanCurve ScanCurve
        {
            private set;
            get;
        }

        [Browsable(false)]
        public ScanCurve ScanCurve2
        {
            private set;
            get;
        }

        [Display(
            Name = "Axis",
            GroupName = PROP_GRP_H_AXIS,
            Description = "The axis of horizontal direction.")]
        public LogicalAxis Axis
        {
            get => axis;
            set
            {
                UpdateProperty(ref axis, value);
            }
        }

        [Display(
            Name = "Interval",
            GroupName = PROP_GRP_H_AXIS,
            Description = "The scan interval for horizontal direction.")]
        public double HorizonalInterval
        {
            get => hScanInterval;
            set
            {
                UpdateProperty(ref hScanInterval, value);
            }
        }

        [Display(
           Name = "Range",
           GroupName = PROP_GRP_H_AXIS,
           Description = "The scan range of the horizonal direction.")]
        public double HorizonalRange
        {
            get => axisRestriction;
            set
            {
                UpdateProperty(ref axisRestriction, value);
            }
        }

        [Display(
            Name = "Axis",
            GroupName = PROP_GRP_V_AXIS,
            Description = "The axis of vertical direction.")]
        public LogicalAxis Axis2
        {
            get => axis2;
            set
            {
                UpdateProperty(ref axis2, value);
            }
        }


        [Display(
            Name = "Interval",
            GroupName = PROP_GRP_V_AXIS,
            Description = "The scan interval of vertical direction.")]
        public double VerticalInterval
        {
            get => vScanInterval;
            set
            {
                UpdateProperty(ref vScanInterval, value);
            }
        }

        [Display(
            Name = "Range",
            GroupName = PROP_GRP_V_AXIS,
            Description = "The scan range of vertical direction.")]
        public double VerticalRange
        {
            get => axis2Restriction;
            set
            {
                UpdateProperty(ref axis2Restriction, value);
            }
        }

        #endregion

        #region Methods

        public override void Validate()
        {
            //base.Validate();

            if (MoveSpeed < 1 || MoveSpeed > 100)
                throw new ArgumentException("move speed must be between 1 ~ 100");
            
            if (Axis == null)
                throw new ArgumentException("the horizontal axis is empty.");

            if (Axis2 == null)
                throw new ArgumentException("the vertical axis is empty.");

            if (Axis == Axis2)
                throw new ArgumentException("the two axes are pointing to the same axis.");

            if (Axis.PhysicalAxisInst.UnitHelper.Unit != Axis2.PhysicalAxisInst.UnitHelper.Unit)
                throw new ArgumentException("the unit of two axes is different.");
        }

        #endregion
    }
}
