using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Equipments.Equipments.MotionControllers.M12;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Irixi_Aligner_Common.Alignment.SpiralScan
{
    public class SpiralScanArgs : AlignmentArgsBase
    {
        #region Variables
        const string PROP_GRP_AXIS = "Axis Settings";

        private LogicalAxis axis, axis2;
        private double interval, range;
        #endregion

        #region Constructors

        public SpiralScanArgs() : base()
        {
            ScanCurve = new ScanCurve3D();
            this.ScanCurveGroup.Add(ScanCurve);

            AxisXTitle = "Horizontal";
            AxisYTitle = "Verical";
            AxisZTitle = "Intensity";

            Properties.Add(new Property("Instrument"));
            Properties.Add(new Property("Axis"));
            Properties.Add(new Property("Axis2"));
            Properties.Add(new Property("Interval"));
            Properties.Add(new Property("Range"));
            Properties.Add(new Property("MoveSpeed"));
        }

        #endregion

        #region Properties

        [Browsable(false)]
        public override string PresetProfileFolder
        {
            get
            {
                return "SpiralScan";
            }
        }

        [Display(
            Name = "H Axis",
            GroupName = PROP_GRP_AXIS,
            Description = "The scan process starts along the horizontal axis first.")]
        public LogicalAxis Axis
        {
            get => axis;
            set
            {
                UpdateProperty(ref axis, value);
            }
        }

        //[Browsable(false)]
        //public string AxisHashString
        //{
        //    private get; set;
        //}

        [Display(
            Name = "V Axis",
            GroupName = PROP_GRP_AXIS,
            Description = "The scan process starts along the horizontal axis first.")]
        public LogicalAxis Axis2
        {
            get => axis2;
            set
            {
                UpdateProperty(ref axis2, value);
            }
        }

        //[Browsable(false)]
        //public string Axis2HashString
        //{
        //    private get; set;
        //}

        [Display(
            Name = "Interval",
            GroupName = PROP_GRP_AXIS,
            Description = "The scan interval for both H-Axis and V-Axis.")]
        public double Interval
        {
            get => interval;
            set
            {
                UpdateProperty(ref interval, value);
            }
        }

        [Display(
            Name = "Range",
            GroupName = PROP_GRP_AXIS,
            Description = "The scan scope restriction.")]
        public double Range
        {
            get => range;
            set
            {
                UpdateProperty(ref range, value);
            }
        }

        [Browsable(false)]
        public override IInstrument Instrument { get => base.Instrument; set => base.Instrument = value; }

        [Browsable(false)]
        public ScanCurve3D ScanCurve
        {
            get;
        }

        #endregion

        #region Methods

        public override void Validate()
        {
            base.Validate();

            if (Axis == null)
                throw new ArgumentException("You must specify the horizontal axis.");

            if (Axis2 == null)
                throw new ArgumentException("You must specify the vertical axis.");

            if (Axis.PhysicalAxisInst.Parent != Axis2.PhysicalAxisInst.Parent)
                throw new ArgumentException("The two axes must come from the same motion controller.");

            if (Axis.Equals(Axis2))
                throw new ArgumentException("the two axes must be different");

            if (Axis.PhysicalAxisInst.UnitHelper.Unit != Axis2.PhysicalAxisInst.UnitHelper.Unit)
                throw new ArgumentException("the two axes have different unit");

            if (Axis.PhysicalAxisInst.GetType() != typeof(IrixiM12Axis))
                throw new ArgumentException("The function only available on the M12 controller.");
        }

        public override void ClearScanCurve()
        {
            ScanCurve.Clear();
        }

        #endregion
    }
}
