using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Equipments.Base;
using Irixi_Aligner_Common.Equipments.BaseClass;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Base;

namespace Irixi_Aligner_Common.Alignment.CentralAlign
{
    public class CentralAlignArgs : AlignmentArgsBase
    {
        #region Variables
        const string PROP_GRP_H_AXIS = "Horizontal Settings";
        const string PROP_GRP_V_AXIS = "Vertical Settings";

        IInstrument instrument, instrument2;

        LogicalAxis axis, axis2;
        double hScanInterval = 1, vScanInterval = 1, axisRestriction = 100, axis2Restriction = 100;


        #endregion

        #region Constructors

        public CentralAlignArgs() : base()
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

            Properties.Add(new Property("Instrument"));
            Properties.Add(new Property("Instrument2"));
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
                return "CentralAlign";
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
        
        public override IInstrument Instrument
        {
            get => instrument;
            set
            {
                UpdateProperty(ref instrument, value);

                ScanCurve.DisplayName = ((InstrumentBase)instrument).Config.Caption;
            }
        }

        [Display(
            Name = "Instrument 2",
            GroupName = PROP_GRP_COMMON,
            Description = "The instrument for feedback of the second channel")]
        public IInstrument Instrument2
        {
            get => instrument2;
            set
            {
                UpdateProperty(ref instrument2, value);

                ScanCurve2.DisplayName = ((InstrumentBase)instrument2).Config.Caption;
            }
        }

        #endregion

        #region Methods

        public override void Validate()
        {
            //base.Validate();

            if (MoveSpeed < 1 || MoveSpeed > 100)
                throw new ArgumentException("move speed must be between 1 ~ 100");

            if (Instrument == null)
                throw new ArgumentException(string.Format("the {0} is empty",
                    ((DisplayAttribute)TypeDescriptor.GetProperties(this)["Instrument"].Attributes[typeof(DisplayAttribute)]).Name) ?? "instrument");

            if (Instrument2 == null)
                throw new ArgumentException(string.Format("the {0} is empty.",
                    ((DisplayAttribute)TypeDescriptor.GetProperties(this)["Instrument2"].Attributes[typeof(DisplayAttribute)]).Name) ?? "instrument2");

            if (Instrument == Instrument2)
                throw new ArgumentException("the two instruments are pointing to the same instrument.");


            if (Axis == null)
                throw new ArgumentException("the horizontal axis is empty.");

            if (Axis2 == null)
                throw new ArgumentException("the vertical axis is empty.");

            if (Axis == Axis2)
                throw new ArgumentException("the two axes are pointing to the same axis.");

            if (Axis.PhysicalAxisInst.UnitHelper.Unit != Axis2.PhysicalAxisInst.UnitHelper.Unit)
                throw new ArgumentException("the unit of two axis is different.");
        }

        #endregion
    }
}
