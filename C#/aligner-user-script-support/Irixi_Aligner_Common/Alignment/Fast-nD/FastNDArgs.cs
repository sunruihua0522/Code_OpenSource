using Irixi_Aligner_Common.Alignment.Base;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.MotionControllers.Base;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Irixi_Aligner_Common.Alignment.FastND
{
    public class FastNDArgs : AlignmentArgsBase
    {
        #region Variables
        LogicalMotionComponent _selectedAligner;
        double target;
        int maxCycles;
        int maxOrder;
        ObservableCollection<int> listScanOrder;

       ObservableCollection<Fast1DArgs> axisParamCollection;
       ReadOnlyObservableCollection<Fast1DArgs> readonlyAxisParamCollection;
        #endregion

        #region Constructors

        public FastNDArgs() : base()
        {
            this.target = 0;
            this.maxCycles = 1;

            // build valid scan order
            listScanOrder = new ObservableCollection<int>();

            // build list contains each single axis parameter object
            axisParamCollection = new ObservableCollection<Fast1DArgs>();
            readonlyAxisParamCollection = new ReadOnlyObservableCollection<Fast1DArgs>(axisParamCollection);

            Properties.Add(new Property("SelectedAligner"));
            Properties.Add(new Property("Target"));
            Properties.Add(new Property("MaxCycles"));
            Properties.Add(new Property() { CollectionName = "AxisParamCollection" });

            AxisXTitle = "Position";
            AxisYTitle = "Intensity";

        }

        #endregion

        #region Properties
        
        public override string PresetProfileFolder
        {
            get
            {
                return "FastND";
            }
        }

        [Display(
            Name = "Aligner",
            GroupName = PROP_GRP_COMMON,
            Description = "Select a aligner to specify the axis used to align.")]
        public override LogicalMotionComponent SelectedAligner
        {
            get => _selectedAligner;
            set
            {
                UpdateProperty(ref _selectedAligner, value);

                // add new editors
                axisParamCollection.Clear();
                ScanCurveGroup.Clear();

                foreach (var axis in _selectedAligner)
                {
                    var arg = new Fast1DArgs()
                    {
                        Axis = axis,
                        IsEnabled = false,
                        MoveSpeed = 100,
                        Interval = 10,
                        Range = 100,
                        Order = 1
                    };

                    axisParamCollection.Add(arg);
                    ScanCurveGroup.Add(arg.ScanCurve);
                }

                this.MaxOrder = value.Count;
            }
        }

        [Display(
            Name = "Target",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        public double Target
        {
            get => target;
            set
            {
                UpdateProperty(ref target, value);
            }
        }

        [Display(
            Name = "Max Cycles",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        public int MaxCycles
        {
            get => maxCycles;
            set
            {
                UpdateProperty(ref maxCycles, value);
            }
        }

        [Display(
            Name = "Axis Group",
            GroupName = PROP_GRP_COMMON,
            Description = "")]
        public ReadOnlyObservableCollection<Fast1DArgs> AxisParamCollection
        {
            get => readonlyAxisParamCollection;
        }

        [Browsable(false)]
        public int MaxOrder
        {
            set
            {
                maxOrder = value;

                listScanOrder.Clear();
                for (int i = 1; i <= maxOrder; i++)
                {
                    listScanOrder.Add(i);
                }
            }
            get => maxOrder;
        }

        [Browsable(false)]
        public ObservableCollection<int> ListScanOrder
        {
            get => listScanOrder;
        }

        #endregion

        #region Methods

        public override void Validate()
        {
            if (SelectedAligner == null)
                throw new ArgumentException("no motion component selected.");
            
            if(MaxCycles <= 0)
                throw new ArgumentException("the max cycles is not set.");

            if (AxisParamCollection == null || AxisParamCollection.Count < 1)
                throw new ArgumentNullException("not axis group can not be empty.");
            
            foreach(var arg in AxisParamCollection)
            {
                // check if the align order is unique
                if (arg.IsEnabled)
                {
                    if(arg.Range <= 0)
                        throw new ArgumentException(string.Format("the range is error."));

                    if(arg.Interval <= 0)
                        throw new ArgumentException(string.Format("the step is error."));

                    if(arg.MoveSpeed < 1 || arg.MoveSpeed > 100)
                        throw new ArgumentException(string.Format("the speed is out of range."));
                }
            }
        }

        public override void ClearScanCurve()
        {
            foreach (var arg in AxisParamCollection)
            {
                arg.ClearScanCurve();
            }
        }

        #endregion
    }
}
