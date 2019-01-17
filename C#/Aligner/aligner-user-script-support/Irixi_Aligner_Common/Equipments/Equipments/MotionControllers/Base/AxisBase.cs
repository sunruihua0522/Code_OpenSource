using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Configuration.MotionController;
using Irixi_Aligner_Common.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Irixi_Aligner_Common.MotionControllers.Base
{
    /*
     * NOTE: 
     * All parameters in this class that related position are in 'steps' except UnitHelper
     */
    public class AxisBase : IAxis, INotifyPropertyChanged
    {
        #region Variables
        int absPosition = 0, relPosition = 0;
        int scwl = 0, sccwl = 0;
        bool
            isEnabled = false,
            isAligner = true,
            isHomed = false,
            isManualEnabled = false,
            isAbsMode = false,
            isBusy = false;
        
        ManualResetEvent _axis_lock;

        #endregion

        #region Constructors

        private void GenericConstructor()
        {
            _axis_lock = new ManualResetEvent(true);

            this.IsEnabled = false;
        }

        ///// <summary>
        ///// If you create this object without any parameters, the SetParameters() function MUST BE implemented
        ///// </summary>
        //public AxisBase()
        //{
        //    GenericConstructor();
        //}

        public AxisBase(ConfigPhysicalAxis Config, IMotionController Controller)
        {
            GenericConstructor();
            Setup(Config, Controller);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get what the motion controller belongs to.
        /// </summary>
        public IMotionController Parent { get; private set; }

        /// <summary>
        /// Get the name of the axis defined in the config file.
        /// The property is used to recognize the each independent axis of the motion controller, 
        /// For the Luminos it should be  X, Y, Z, ROLL, etc.
        /// For the IrixiEE0017 it should be the number 1, 2, 3 since there are only three channels available for the board
        /// For the IrixiM12 it should be Unit1 - Unit12
        /// </summary>
        public virtual string AxisName { set; get; }

        /// <summary>
        /// Is the axis moving or not.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                UpdateProperty<bool>(ref isBusy, value);
            }
        }


        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                UpdateProperty<bool>(ref isEnabled, value);
            }
        }

        public bool IsAligner
        {
            get
            {
                return isAligner;
            }
            set
            {
                UpdateProperty<bool>(ref isAligner, value);
            }
        }

        public bool IsManualEnabled
        {
            set
            {
                UpdateProperty<bool>(ref isManualEnabled, value);
            }
            get
            {
                return isManualEnabled;
            }
        }

        public bool IsAbsMode
        {
            set
            {
                UpdateProperty<bool>(ref isAbsMode, value);
            }
            get
            {
                return isAbsMode;
            }
        }

        public bool IsHomed
        {
            internal set
            {
                UpdateProperty<bool>(ref isHomed, value);
            }
            get
            {
                return isHomed;
            }
        }

        public int InitPosition { get; private set; }

        public int AbsPosition
        {
            get
            {
                return absPosition;
            }
            set
            {
                // calculate relative postion once the absolute position was changed
                this.RelPosition += (value - absPosition);

                UpdateProperty<int>(ref absPosition, value);

                // convert steps to real-world distance
                this.UnitHelper.AbsPosition = this.UnitHelper.ConvertStepsToDistance(absPosition);
            }
        }

        public int RelPosition
        {
            get
            {
                return relPosition;
            }
            private set
            {
                UpdateProperty<int>(ref relPosition, value);

                // convert steps to real-world distance
                this.UnitHelper.RelPosition = this.UnitHelper.ConvertStepsToDistance(relPosition);
            }
        }

        public object Tag { get; set; }

        /// <summary>
        /// Get the maximum speed
        /// </summary>
        public virtual int MaxSpeed { get; protected set; }

        /// <summary>
        /// Get how many steps used to accelerate
        /// </summary>
        public int AccelerationSteps { private set; get; }
        
        /// <summary>
        /// Get the soft CCW limit in config file (normal zero point)
        /// </summary>
        public int SCCWL
        {
            get
            {
                return sccwl;
            }
            protected set
            {
                UpdateProperty(ref sccwl, value);
            }
        }

        /// <summary>
        /// Get the soft CW limit in config file
        /// </summary>
        public int SCWL
        {
            get
            {
                return scwl;
            }
            protected set
            {
                UpdateProperty(ref scwl, value);
            }
        }

        public RealworldUnitManager UnitHelper { protected set; get; }
        
        public string LastError { set; get; }

        public string HashString
        {
            get
            {
                var factor = string.Join("", new object[]
                {
                AxisName,
                UnitHelper.HashString,
                Parent.DeviceClass
                });

                return HashGenerator.GetHashSHA256(factor);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Read the current status of the controller.
        /// Why we need this function? If the software is restarted but the controller is not restarted, the axes are
        /// </summary>
        /// <param name="AbsPosition"></param>
        /// <param name="IsHomed"></param>
        public virtual void SetInitStatus(int AbsPosition, bool IsHomed)
        {
            this.AbsPosition = AbsPosition;
            this.IsHomed = IsHomed;
        }

        public bool Lock()
        {
            return _axis_lock.WaitOne(500);
        }
        
        public void Unlock()
        {
            _axis_lock.Set();
        }
        
        public bool CheckSoftLimitation(int TargetPosition)
        {
            if (TargetPosition < this.SCCWL || TargetPosition > this.SCWL)
                return false;
            else
                return true;
        }

        public virtual bool Home()
        {
            return this.Parent.Home(this);
        }
        
        public virtual bool Move(MoveMode Mode, int Speed, double Distance)
        {
            int steps = 0;

            if (Mode == MoveMode.REL && Distance == 0)
                return true;
            
            try
            {
                steps = this.UnitHelper.ConvertDistanceToSteps(Distance);
                return Parent.Move(this, Mode, Speed, steps);
            }
            catch(Exception ex)
            {
                this.LastError = ex.Message;
                return false;
            }
            
        }

        public virtual bool MoveWithTrigger(MoveMode Mode, int Speed, double Distance, double Interval, int Channel)
        {
            return Parent.MoveWithTrigger(
                this, 
                Mode, 
                Speed, 
                this.UnitHelper.ConvertDistanceToSteps(Distance),
                this.UnitHelper.ConvertDistanceToSteps(Interval), 
                Channel);
        }

        public virtual bool MoveWithInnerADC(MoveMode Mode, int Speed, double Distance, double Interval, int Channel)
        {
            return Parent.MoveWithInnerADC(this,
                Mode,
                Speed,
                this.UnitHelper.ConvertDistanceToSteps(Distance),
                this.UnitHelper.ConvertDistanceToSteps(Interval),
                Channel);
        }

        public virtual void Stop()
        {
            this.Parent.Stop();
        }

        public void ToggleMoveMode()
        {
            if(this.IsAbsMode)  // changed move mode from ABS to REL
            {
                ClearRelPosition();
                this.IsAbsMode = false;
            }
            else  // change move mode from REL to ABS
            {
                this.UnitHelper.RelPosition = this.UnitHelper.AbsPosition;
                this.RelPosition = this.AbsPosition;
                this.IsAbsMode = true;
            }
        }

        public void ClearRelPosition()
        {
            this.RelPosition = 0;
            this.UnitHelper.RelPosition = 0;
        }

        public override string ToString()
        {
            return string.Format("{0}@{1}", this.AxisName, this.Parent.DeviceClass);
        }

        #endregion

        #region Private Methods

        internal virtual void Setup(ConfigPhysicalAxis Config, IMotionController Controller)
        {
            if (Config == null)
            {
                this.IsEnabled = false;
                this.AxisName = "NA";
                this.UnitHelper = new RealworldUnitManager(0, 0, RealworldUnitManager.UnitType.mm);
            }
            else
            {
                this.AxisName = Config.Name;
                this.IsEnabled = Config.Enabled;
                this.InitPosition = Config.OffsetAfterHome;
                this.MaxSpeed = Config.MaxSpeed;
                this.AccelerationSteps = Config.Acceleration;
                this.Parent = Controller;

                this.UnitHelper = new RealworldUnitManager(
                    Config.MotorizedStageProfile.TravelDistance,
                    Config.MotorizedStageProfile.Resolution,
                    Config.MotorizedStageProfile.Unit,
                    Config.DecimalPlacesDisplayed);

                this.SCCWL = 0;
                this.SCWL = this.UnitHelper.MaxSteps;
            }
        }

        #endregion

        #region RaisePropertyChangedEvent
        public event PropertyChangedEventHandler PropertyChanged;

        protected void UpdateProperty<T>(ref T OldValue, T NewValue, [CallerMemberName]string PropertyName = "")
        {
            if (object.Equals(OldValue, NewValue))  // To save resource, if the value is not changed, do not raise the notify event
                return;

            OldValue = NewValue;                // Set the property value to the new value
            OnPropertyChanged(PropertyName);    // Raise the notify event
        }

        protected void OnPropertyChanged([CallerMemberName]string PropertyName = "")
        {
            //PropertyChangedEventHandler handler = PropertyChanged;
            //if (handler != null)
            //    handler(this, new PropertyChangedEventArgs(PropertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
                
        }

        #endregion
    }
}
