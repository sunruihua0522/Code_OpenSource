using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace IrixiStepperControllerHelper
{

    public class AxisStateReport : INotifyPropertyChanged, ICloneable
    {
        int _abs_position = 0;
        int _axis_index = 0;
        bool _is_homed = false, _is_busy = false;
        InputState _cwls = InputState.Untriggered, _ccwls = InputState.Untriggered, _org = InputState.Untriggered, _zero_out = InputState.Untriggered, _in_a = InputState.Untriggered, _in_b = InputState.Untriggered;
        OutputState _out_a = OutputState.Disabled, _out_b = OutputState.Disabled;
        int _error = 0;

        /// <summary>
        /// Get the absolut position
        /// </summary>
        public int AbsPosition
        {
            internal set
            {
                UpdateProperty<int>(ref _abs_position, value);
            }
            get
            {
                return _abs_position;
            }
        }

        /// <summary>
        /// Get the index of the current axis
        /// </summary>
        public int AxisIndex
        {
            internal set
            {
                UpdateProperty<int>(ref _axis_index, value);
            }
            get
            {
                return _axis_index;
            }

        }
        
        /// <summary>
        /// Get whether the axis has been home
        /// </summary>
        public bool IsHomed
        {
            internal set
            {
                UpdateProperty<bool>(ref _is_homed, value);
            }
            get
            {
                return _is_homed;
            }
        }

        /// <summary>
        /// Get whether the axis is busy
        /// </summary>
        public bool IsRunning
        {
            internal set
            {
                UpdateProperty<bool>(ref _is_busy, value);
            }

            get
            {
                return _is_busy;
            }
        }

        /// <summary>
        /// Get the error code that the controller returned
        /// </summary>
        public int Error
        {
            internal set
            {
                UpdateProperty<int>(ref _error, value);
            }
            get
            {
                return _error;
            }
        }

        public byte CommandOrder
        {
            set;
            get;
        }

        /// <summary>
        /// Get whether the CW limitation sensor has been touched
        /// </summary>
        public InputState CWLS
        {
            internal set
            {
                UpdateProperty<InputState>(ref _cwls, value);
            }
            get
            {
                return _cwls;
            }
        }

        /// <summary>
        /// Get whether the CCW limitation sensor has been touched
        /// </summary>
        public InputState CCWLS
        {
            internal set
            {
                UpdateProperty<InputState>(ref _ccwls, value);
            }
            get
            {
                return _ccwls;
            }
        }

        /// <summary>
        /// Get whether the Orginal Limitation Sensor has been touched
        /// </summary>
        public InputState ORG
        {
            internal set
            {
                UpdateProperty<InputState>(ref _org, value);
            }
            get
            {
                return _org;
            }
        }

        /// <summary>
        /// Get whether the ZeroOut Pusle (TIMING Signal) has been detected
        /// </summary>
        public InputState ZeroOut
        {
            internal set
            {
                UpdateProperty<InputState>(ref _zero_out, value);
            }
            get
            {
                return _zero_out;
            }
        }

        /// <summary>
        /// Get the status of the input A
        /// true: not triggered; false: triggered
        /// </summary>
        public InputState IN_A
        {
            internal set
            {
                UpdateProperty<InputState>(ref _in_a, value);
            }
            get
            {
                return _in_a;
            }
        }

        /// <summary>
        /// Get the status of the input A
        /// true: not triggered; false: triggered
        /// </summary>
        public InputState IN_B
        {
            internal set
            {
                UpdateProperty<InputState>(ref _in_b, value);
            }
            get
            {
                return _in_b;
            }
        }

        /// <summary>
        /// Get the status of the out port A
        /// </summary>
        public OutputState OUT_A
        {
            internal set
            {
                UpdateProperty<OutputState>(ref _out_a, value);
            }
            get
            {
                return _out_a;
            }
        }

        /// <summary>
        /// Get the status of the out port B
        /// </summary>
        public OutputState OUT_B
        {
            internal set
            {
                UpdateProperty<OutputState>(ref _out_b, value);
            }
            get
            {
                return _out_b;
            }
        }

        public bool Parse(byte[] Buffer)
        {
            byte temp = 0x0;

            using (MemoryStream stream = new MemoryStream(Buffer))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    this.AbsPosition = reader.ReadInt32();

                    // parse Usability
                    temp = reader.ReadByte();
                    this.IsHomed = ((temp >> 0) & 0x1) > 0 ? true : false;
                    this.IsRunning = ((temp >> 1) & 0x1) > 0 ? true : false;

                    // parse input signal
                    temp = reader.ReadByte();
                    this.CWLS = ((temp >> 0) & 0x1) > 0 ? InputState.Triggered : InputState.Untriggered;
                    this.CCWLS = ((temp >> 1) & 0x1) > 0 ? InputState.Triggered : InputState.Untriggered;
                    this.ORG = ((temp >> 2) & 0x1) > 0 ? InputState.Triggered : InputState.Untriggered;
                    this.ZeroOut = ((temp >> 3) & 0x1) > 0 ? InputState.Triggered : InputState.Untriggered;
                    this.IN_A = ((temp >> 4) & 0x1) > 0 ? InputState.Untriggered : InputState.Triggered;
                    this.IN_B = ((temp >> 5) & 0x1) > 0 ? InputState.Untriggered : InputState.Triggered;
                    this.OUT_A = ((temp >> 6) & 0x1) > 0 ? OutputState.Enabled : OutputState.Disabled;
                    this.OUT_B = ((temp >> 7) & 0x1) > 0 ? OutputState.Enabled : OutputState.Disabled;

                    this.Error = reader.ReadByte();
                    this.CommandOrder = reader.ReadByte();
                }
            }

            return true;
        }

        public object Clone()
        {
            AxisStateReport state = new AxisStateReport
            {
                AbsPosition = this.AbsPosition,
                AxisIndex = this.AxisIndex,
                IsHomed = this.IsHomed,
                IsRunning = this.IsRunning,
                Error = this.Error,
                CWLS = this.CWLS,
                CCWLS = this.CCWLS,
                ORG = this.ORG,
                ZeroOut = this.ZeroOut,
                IN_A = this.IN_A,
                IN_B = this.IN_B,
                OUT_A = this.OUT_A,
                OUT_B = this.OUT_B
            };

            return state;
        }
        
        #region RaisePropertyChangedEvent

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
            //RaisePropertyChanged(PropertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        }
        #endregion
    }

    public class DeviceStateReport : INotifyPropertyChanged, ICloneable
    {
        #region Constants
        const int ERR_NONE = 0;     
        const int ERR_AXISID = 1;   
        const int ERR_PARA = 2;     
        const int ERR_NOTHM = 5;    
        const int ERR_BUSY = 7;     
        const int ERR_CWLS = 10;    
        const int ERR_CCWLS = 20;   
        const int ERR_EMEGENCY = 30;
        const int ERR_USRSTOP = 31; 
        const int ERR_NOSTAGE = 253;
        const int ERR_IICBUS = 254; 
        const int ERR_FATAL = 255; 
        #endregion

        #region Variables
        static object _lock = new object();
        uint _counter;
        int _total_axes;
        int _is_busy;
        int _sys_error;
        InputState _triggerinput0 = InputState.Untriggered, _triggerinput1 = InputState.Untriggered;
        int _core_vref, _core_temp;
        #endregion

        #region Constructors

        public DeviceStateReport()
        {
            this.AxisStateCollection = new ObservableCollection<AxisStateReport>();
            BindingOperations.EnableCollectionSynchronization(this.AxisStateCollection, _lock);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Get the number of the counter of the report pack
        /// </summary>
        public uint Counter
        {
            internal set
            {
                UpdateProperty<uint>(ref _counter, value);
            }
            get
            {
                return _counter;
            }
        }

        /// <summary>
        /// Get the tatal axes the controller supports
        /// </summary>
        public int TotalAxes
        {
            internal set
            {
                UpdateProperty<int>(ref _total_axes, value);
            }
            get
            {
                return _total_axes;
            }
        }

        /// <summary>
        /// Get how many axes are moving
        /// </summary>
        public int IsBusy
        {
            internal set
            {
                UpdateProperty<int>(ref _is_busy, value);
            }
            get
            {
                return _is_busy;
            }
        }

        /// <summary>
        /// Indicates that the emergency button was pressed or the value of IsBusy was out of range
        /// 30: Emergency Button was pressed
        /// 255: IsBusy was out of range
        /// </summary>
        public int SystemError
        {
            internal set
            {
                UpdateProperty<int>(ref _sys_error, value);
            }
            get
            {
                return _sys_error;
            }
        }

        /// <summary>
        /// Get the state of the trigger input 0
        /// true: triggered
        /// false: not triggered
        /// </summary>
        public InputState TriggerInput0
        {
            internal set
            {
                UpdateProperty<InputState>(ref _triggerinput0, value);
            }
            get
            {
                return _triggerinput0;
            }
        }

        /// <summary>
        /// Get the state of the trigger input 1
        /// true: triggered
        /// false: not triggered
        /// </summary>
        public InputState TriggerInput1
        {
            internal set
            {
                UpdateProperty<InputState>(ref _triggerinput1, value);
            }
            get
            {
                return _triggerinput1;
            }
        }

        /// <summary>
        /// Get the value of the voltage reference regulator inside the core
        /// </summary>
        public int CoreVref
        {
            internal set
            {
                UpdateProperty<int>(ref _core_vref, value);
            }
            get
            {
                return _core_vref;
            }
        }

        /// <summary>
        /// Get the value of core temperature
        /// </summary>
        public int CoreTemp
        {
            internal set
            {
                UpdateProperty<int>(ref _core_temp, value);
            }
            get
            {
                return _core_temp;
            }
        }

        public ObservableCollection<AxisStateReport> AxisStateCollection { internal set; get; }
        #endregion

        #region Methods
        /// <summary>
        /// Parse the raw data of hid report
        /// </summary>
        /// <param name="Data"></param>
        public void Parse(byte[] Data)
        {
            byte temp = 0x0;

            //// check the lenght of the data arry
            //if (Data.Length < 64)
            //    return;

            using (MemoryStream stream = new MemoryStream(Data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.ReadByte(); // ignore report id

                    this.Counter = reader.ReadUInt32();
                    this.TotalAxes = reader.ReadByte();
                    this.IsBusy = reader.ReadByte();
                    this.SystemError = reader.ReadByte();

                    // Read the Trigger Input State
                    temp = reader.ReadByte();
                    this.TriggerInput0 = ((temp >> 0) & 0x1) > 0 ? InputState.Untriggered : InputState.Triggered;
                    this.TriggerInput1 = ((temp >> 1) & 0x1) > 0 ? InputState.Untriggered : InputState.Triggered;

                    // Read the parameters of the core
                    this.CoreVref = reader.ReadInt32();
                    this.CoreTemp = reader.ReadInt32();

                    if (this.AxisStateCollection == null || AxisStateCollection.Count == 0)
                        return;

                    var buffRemainCnt = (int)(stream.Length - stream.Position);
                    if (buffRemainCnt % 3 > 0)
                    {

                    }
                    else
                    {
                        var blockLen = buffRemainCnt / 3;
                        // flush the state of each axis
                        for (int i = 0; i < this.AxisStateCollection.Count; i++)
                        {
                            var buffAxisState = reader.ReadBytes(blockLen);
                            this.AxisStateCollection[i].Parse(buffAxisState);
                            /////
                            ///// The following parsing process are base on the type of AxisState_TypeDef which is defined in the controller firmware
                            /////

                            //this.AxisStateCollection[i].AbsPosition = reader.ReadInt32();

                            //// parse Usability
                            //temp = reader.ReadByte();
                            //this.AxisStateCollection[i].IsHomed = ((temp >> 0) & 0x1) > 0 ? true : false;
                            //this.AxisStateCollection[i].IsRunning = ((temp >> 1) & 0x1) > 0 ? true : false;

                            //// parse input signal
                            //temp = reader.ReadByte();
                            //this.AxisStateCollection[i].CWLS = ((temp >> 0) & 0x1) > 0 ? InputState.Triggered : InputState.Untriggered;
                            //this.AxisStateCollection[i].CCWLS = ((temp >> 1) & 0x1) > 0 ? InputState.Triggered : InputState.Untriggered;
                            //this.AxisStateCollection[i].ORG = ((temp >> 2) & 0x1) > 0 ? InputState.Triggered : InputState.Untriggered;
                            //this.AxisStateCollection[i].ZeroOut = ((temp >> 3) & 0x1) > 0 ? InputState.Triggered : InputState.Untriggered;
                            //this.AxisStateCollection[i].IN_A = ((temp >> 4) & 0x1) > 0 ? InputState.Untriggered : InputState.Triggered;
                            //this.AxisStateCollection[i].IN_B = ((temp >> 5) & 0x1) > 0 ? InputState.Untriggered : InputState.Triggered;
                            //this.AxisStateCollection[i].OUT_A = ((temp >> 6) & 0x1) > 0 ? OutputState.Enabled : OutputState.Disabled;
                            //this.AxisStateCollection[i].OUT_B = ((temp >> 7) & 0x1) > 0 ? OutputState.Enabled : OutputState.Disabled;

                            //this.AxisStateCollection[i].Error = reader.ReadByte();
                            //this.AxisStateCollection[i].CommandOrder = reader.ReadByte();
                        }
                    }

                    reader.Close();
                }

                stream.Close();
            }
        }

        /// <summary>
        /// Get the output state by channel
        /// </summary>
        /// <param name="Channel">for 3-Axis controller it should be 0 ~ 5</param>
        /// <returns></returns>
        public OutputState GetOutputState(int Channel)
        {
            OutputState st;
            int _axis = Channel / 2;
            int _port = Channel % 2;

            if (_port == 0)
                st = this.AxisStateCollection[_axis].OUT_A;
            else
                st = this.AxisStateCollection[_axis].OUT_B;

            return st;
        }

        public static string ErrorCodeToString(int error)
        {
            string _err_str = "";
            switch (error)
            {
                case ERR_NONE:
                    _err_str = "no error";
                    break;

                case ERR_AXISID:
                    _err_str = "unknown axis index";
                    break;

                case ERR_PARA:
                    _err_str = "parameters error";
                    break;

                case ERR_NOTHM:
                    _err_str = "axis not homed";
                    break;

                case ERR_BUSY:
                    _err_str = "busy";
                    break;

                case ERR_CWLS:
                    _err_str = "CWLS detected";
                    break;

                case ERR_CCWLS:
                    _err_str = "CCWLS detected";
                    break;

                case ERR_EMEGENCY:
                    _err_str = "emegency button pressed";
                    break;

                case ERR_USRSTOP:
                    _err_str = "stopped by user";
                    break;

                case ERR_NOSTAGE:
                    _err_str = "stage not exists";
                    break;

                case ERR_IICBUS:
                    _err_str = "IIC bus error";
                    break;

                case ERR_FATAL:
                    _err_str = "fatal error";
                    break;
            }

            return _err_str;
        }

        public object Clone()
        {
            DeviceStateReport state = new DeviceStateReport();
            state.Counter = this.Counter;
            state.TotalAxes = this.TotalAxes;
            state.IsBusy = this.IsBusy;
            state.SystemError = this.SystemError;
            state.TriggerInput0 = this.TriggerInput0;
            state.TriggerInput1 = this.TriggerInput1;
            state.CoreVref = this.CoreVref;
            state.CoreTemp = this.CoreTemp;

            state.AxisStateCollection = new ObservableCollection<AxisStateReport>();
            for (int i = 0; i < this.AxisStateCollection.Count; i++)
            {
                state.AxisStateCollection.Add(this.AxisStateCollection[i].Clone() as AxisStateReport);
            }

            return state;
        }

        #endregion

        #region RaisePropertyChangedEvent

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Update the specified property and fire the ProrpertyChanged event to update the UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        /// <param name="PropertyName"></param>
        protected void UpdateProperty<T>(ref T OldValue, T NewValue, [CallerMemberName]string PropertyName = "")
        {
            if (object.Equals(OldValue, NewValue))  // To save resource, if the value is not changed, do not raise the notify event
                return;

            OldValue = NewValue;                // Set the property value to the new value
            OnPropertyChanged(PropertyName);    // Raise the notify event
        }

        protected void OnPropertyChanged([CallerMemberName]string PropertyName = "")
        {

            // Implement MVVMLight framework
            // RaisePropertyChanged(PropertyName);

            // Implement INotifyPropertyChanged interface
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        }
        #endregion
    }
}
