using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.MotionControllers.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Irixi_Aligner_Common.Alignment.FastND
{
    public class Fast1DArgs : AlignmentArgsBase
    {
        #region Variables

        LogicalAxis axis;
        bool isEnabled;
        
        #endregion

        public Fast1DArgs() : base()
        {
            ScanCurve = new ScanCurve();
        }
        
        [Display(
            Name = "Axis",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        
        public LogicalAxis Axis
        {
            get => axis;
            set
            {
                UpdateProperty(ref axis, value);

                this.ScanCurve.DisplayName = Axis.AxisName;
            }
        }

        [Display(
            Name = "Enabled",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                UpdateProperty(ref isEnabled, value);

                ScanCurve.Visible = isEnabled;
            }
        }

        [Display(
            Name = "Interval",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        public double Interval { set; get; }

        [Display(
            Name = "Range",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        public double Range { set; get; }

        [Display(
            Name = "Order",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        public int Order { set; get; }

        [Browsable(false)]
        public ScanCurve ScanCurve { set; get; }

        #region Methods

        public override void ClearScanCurve()
        {
            ScanCurve.Clear();
        }

        public override string ToString()
        {
            return string.Format("Axis {0}, {1}, {2}, {3}{4}/{5}{6}",
                new object[]
                {
                    Axis.AxisName,
                    IsEnabled ? "Enabled" : "Disabled",
                    Order,
                    Interval, Axis.PhysicalAxisInst.UnitHelper,
                    Range, Axis.PhysicalAxisInst.UnitHelper
                });
        }

        #endregion
    }
}
