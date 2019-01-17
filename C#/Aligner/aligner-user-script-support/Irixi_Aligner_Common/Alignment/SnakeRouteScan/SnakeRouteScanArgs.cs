using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.MotionControllers.Base;

namespace Irixi_Aligner_Common.Alignment.SnakeRouteScan
{
    public class SnakeRouteScanArgs: AlignmentArgsBase
    {
        #region Variables
        const string PROP_GRP_H = "Horizontal Settings";
        const string PROP_GRP_V = "Vertical Settings";

        LogicalAxis axis, axis2;
        double scanInterval = 1, axisRestriction = 100, axis2Restriction = 100;

        #endregion

        #region Constructors

        public SnakeRouteScanArgs() : base()
        {
            ScanCurve = new ScanCurve3D();
            ScanCurveGroup.Add(ScanCurve);

            AxisXTitle = "Horizontal";
            AxisYTitle = "Verical";
            AxisZTitle = "Intensity";

            Properties.Add(new Property("Instrument"));
            Properties.Add(new Property("Axis"));
            Properties.Add(new Property("AxisRestriction"));
            Properties.Add(new Property("Axis2"));
            Properties.Add(new Property("Axis2Restriction"));
            Properties.Add(new Property("ScanInterval"));
            Properties.Add(new Property("MoveSpeed"));
        }

        #endregion

        #region Properties
        
        public override string PresetProfileFolder
        {
            get
            {
                return "SnakeScan";
            }
        }

        [Browsable(false)]
        public ScanCurve3D ScanCurve
        {
            private set;
            get;
        }

        [Display(
            Name = "Axis",
            GroupName = PROP_GRP_H,
            Description = "The axis is the first one moved at the beginning of scan.")]
        public LogicalAxis Axis
        {
            get => axis;
            set
            {
                UpdateProperty(ref axis, value);
            }
        }

        [Display(
            Name = "Range",
            GroupName = PROP_GRP_H,
            Description = "The scan range of horizontal axis.")]
        public double AxisRestriction
        {
            get => axisRestriction;
            set
            {
                UpdateProperty(ref axisRestriction, value);
            }
        }

        [Display(
            Name = "Vertical",
            GroupName = PROP_GRP_V,
            Description = "The axis is the second one moved.")]
        public LogicalAxis Axis2
        {
            get => axis2;
            set
            {
                UpdateProperty(ref axis2, value);
            }
        }
        
        [Display(
            Name = "Range",
            GroupName = PROP_GRP_V,
            Description = "The scan range of vertical axis.")]
        public double Axis2Restriction
        {
            get => axis2Restriction;
            set
            {
                UpdateProperty(ref axis2Restriction, value);
            }
        }

        [Display(
            Name = "Interval",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        public double ScanInterval
        {
            get => scanInterval;
            set
            {
                UpdateProperty(ref scanInterval, value);
            }
        }

        #endregion

        #region Methods

        public override void Validate()
        {
            if(Axis == null)
                throw new ArgumentException("You must specify the horizontal axis.");

            if (Axis2 == null)
                throw new ArgumentException("You must specify the vertical axis.");

            if (Axis == Axis2)
                throw new ArgumentException("The horizontal axis and the vertical axis must be different.");

            if (Axis.PhysicalAxisInst.UnitHelper.Unit != Axis2.PhysicalAxisInst.UnitHelper.Unit)
                throw new ArgumentException("the two axes have different unit");
        }

        #endregion

    }
}
