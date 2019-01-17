using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Interfaces;
using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Irixi_Aligner_Common.Alignment.Base
{
    public class AlignmentArgsBase : IAlignmentArgs, INotifyPropertyChanged
    {
        #region Variables

        protected const string PROP_GRP_COMMON = "Common";
        protected const string PROP_GRP_TARGET = "Goal";
        private LogicalMotionComponent selectedAligner;
        private IInstrument instrument;
        private int moveSpeed = 100;
        private string axisXTitle = "", axisYTitle = "", axisY2Title = "", axisZTitle = "";

        #endregion Variables

        #region Constructors

        public AlignmentArgsBase()
        {
            this.ScanCurveGroup = new ScanCurveGroup();

            this.Properties = new ObservableCollection<Property>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the sub path of the preset profile where it saved
        /// </summary>
        [Browsable(false)]
        public virtual string PresetProfileFolder => throw new NotImplementedException();

        /// <summary>
        /// Get what properties are allowed to edit
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<Property> Properties { private set; get; }

        ///// <summary>
        ///// The instance of System Service class
        ///// This property is the binding source of the Logical Motion Components and instruments
        ///// </summary>
        //[Browsable(false)]
        //public SystemService Service { private set; get; }

        /// <summary>
        /// Get the title of the Axis X for both 2D&3D chart
        /// </summary>
        [Browsable(false)]
        public string AxisXTitle
        {
            set
            {
                UpdateProperty(ref axisXTitle, value);
            }
            get
            {
                return axisXTitle;
            }
        }

        /// <summary>
        /// Get the title of the Axis Y for both 2D&3D chart
        /// </summary>
        [Browsable(false)]
        public string AxisYTitle
        {
            set
            {
                UpdateProperty(ref axisYTitle, value);
            }
            get
            {
                return axisYTitle;
            }
        }

        /// <summary>
        /// Get the title of the Secondary Axis Y for both 2D&3D chart
        /// </summary>
        [Browsable(false)]
        public string AxisY2Title
        {
            set
            {
                UpdateProperty(ref axisY2Title, value);
            }
            get
            {
                return axisY2Title;
            }
        }

        /// <summary>
        /// Get the title of the Axis X for both 3D chart
        /// </summary>
        [Browsable(false)]
        public string AxisZTitle
        {
            set
            {
                UpdateProperty(ref axisZTitle, value);
            }
            get
            {
                return axisZTitle;
            }
        }

        /// <summary>
        /// Get the group of scanned curves, NOTE it's only used to contain the 2D scanned curves,
        /// for 3D curve, the ScanCurve is bound directly to UI because of issuses of Series3D binding.
        /// </summary>
        [Browsable(false)]
        public ScanCurveGroup ScanCurveGroup { private set; get; }
        
        [Display(
            Name = "Instrument",
            GroupName = PROP_GRP_COMMON,
            Description = "The instrument used to align.")]
        public virtual IInstrument Instrument
        {
            get => instrument;
            set
            {
                UpdateProperty(ref instrument, value);
            }
        }

        [Display(
            Name = "Speed(%)",
            GroupName = PROP_GRP_COMMON,
            Description = "The move speed in percent.")]
        public int MoveSpeed
        {
            get => moveSpeed;
            set
            {
                UpdateProperty(ref moveSpeed, value);
            }
        }

        [Display(
           Name = "Aligner",
           GroupName = PROP_GRP_COMMON,
           Description = "The aligner used to align.")]
        public virtual LogicalMotionComponent SelectedAligner
        {
            get
            {
                return selectedAligner;
            }
            set
            {
                UpdateProperty(ref selectedAligner, value);
            }
        }
        

        #endregion Properties

        #region Methods

        public virtual void Validate()
        {
        }

        /// <summary>
        /// Clear the previous points scan curve
        /// </summary>
        public virtual void ClearScanCurve()
        {
            this.ScanCurveGroup.ClearCurvesContent();
        }

        /// <summary>
        /// Export raw data to cvs file
        /// </summary>
        /// <returns></returns>
        public virtual string ExportScanCurveToCSV(string FileName)
        {
            throw new NotImplementedException();
        }

        #endregion Methods

        #region Raise Property Changed Event

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        /// <param name="PropertyName"></param>
        protected void UpdateProperty<T>(ref T OldValue, T NewValue, [CallerMemberName]string PropertyName = "")
        {
            //if (object.Equals(OldValue, NewValue))  // To save resource, if the value is not changed, do not raise the notify event
            //    return;

            OldValue = NewValue;                // Set the property value to the new value
            OnPropertyChanged(PropertyName);    // Raise the notify event
        }

        protected void OnPropertyChanged([CallerMemberName]string PropertyName = "")
        {
            //PropertyChangedEventHandler handler = PropertyChanged;
            //if (handler != null)
            //    handler(this, new PropertyChangedEventArgs(PropertyName));
            //RaisePropertyChanged(PropertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        }

        #endregion
    }
}