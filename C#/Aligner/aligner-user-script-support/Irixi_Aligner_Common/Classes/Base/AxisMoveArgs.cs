using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Irixi_Aligner_Common.Classes.Base
{
    [Serializable]
    public class AxisMoveArgs : EventArgs, INotifyPropertyChanged, ICloneable, ISerializable
    {
        #region Variables
        
        string axisCaption = "null";
        int speed = 100;
        double distance = 0;
        MoveMode mode = MoveMode.REL;
        bool isMoveable = false;
        int moveOrder = 0;
        string unit = "um";

        #endregion

        #region Constructors

        public AxisMoveArgs()
        {

        }

        public AxisMoveArgs(MoveMode Mode, int Speed, double Distance, string Unit)
        {
            this.Mode = Mode;
            this.Speed = Speed;
            this.Distance = Distance;
            this.Unit = Unit;
        }

        public AxisMoveArgs(SerializationInfo info, StreamingContext context)
        {
            this.LogicalAxisHashString = (string)info.GetValue("LogicalAxisHashString", typeof(string));
            this.AxisCaption = (string)info.GetValue("AxisCaption", typeof(string));
            this.Speed = (int)info.GetValue("Speed", typeof(int));
            this.Distance = (double)info.GetValue("Distance", typeof(double));
            this.Mode = (MoveMode)info.GetValue("Mode", typeof(MoveMode));
            this.IsMoveable = (bool)info.GetValue("IsMoveable", typeof(bool));
            this.MoveOrder = (int)info.GetValue("MoveOrder", typeof(int));
            this.Unit = (string)info.GetValue("Unit", typeof(string));
        }

        #endregion

        #region Properties

        public MassMoveArgs Container { get; set; }

        public string LogicalAxisHashString { get; set; }
        
        /// <summary>
        /// Get or set the caption of axis which is defined in the config file
        /// <see cref="Configuration.MotionController.ConfigLogicalAxis.DisplayName"/>
        /// </summary>
        public string AxisCaption
        {
            get
            {
                return axisCaption;
            }
            set
            {
                UpdateProperty(ref axisCaption, value);
            }
        }

        /// <summary>
        /// The speed to move in %
        /// </summary>
        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                UpdateProperty(ref speed, value);
            }
        }

        /// <summary>
        /// The distance to move
        /// </summary>
        public double Distance
        {
            get
            {
                return distance;
            }
            set
            {
                UpdateProperty(ref distance, value);
            }
        }

        /// <summary>
        /// Abs/Rel
        /// </summary>
        public MoveMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                UpdateProperty(ref mode, value);
            }
        }

        /// <summary>
        /// Is it moveable? The property is specically used in Mass Move Functions
        /// </summary>
        public bool IsMoveable
        {
            get
            {
                return isMoveable;
            }
            set
            {
                UpdateProperty(ref isMoveable, value);
            }
        }

        /// <summary>
        /// The order in the queue. The property is specically used in Mass Move Functions
        /// </summary>
        public int MoveOrder
        {
            get
            {
                return moveOrder;
            }
            set
            {
                UpdateProperty(ref moveOrder, value);
            }
        }
        
        /// <summary>
        /// Get the unit of the distance to move
        /// </summary>
        public string Unit
        {
            get
            {
                return unit;
            }
            set
            {
                UpdateProperty(ref unit, value);
            }
        }

        public string HashString
        {
            get
            {
                var factor = String.Join(";", new object[]
                {
                LogicalAxisHashString,
                Speed,
                Distance,
                Mode,
                IsMoveable,
                MoveOrder,
                Unit
                });

                return HashGenerator.GetHashSHA256(factor);
            }
        }


        #endregion

        #region Methods
        
        public object Clone()
        {
            var obj = new AxisMoveArgs()
            {
                Container = this.Container,
                LogicalAxisHashString = this.LogicalAxisHashString,
                AxisCaption = this.AxisCaption,
                MoveOrder = this.MoveOrder,
                Speed = this.Speed,
                Distance = this.Distance,
                Mode = this.Mode,
                Unit = this.Unit
            };

            return obj;
        }

        public override string ToString()
        {
            return $"{Mode}/{Speed}/{Distance}{Unit}";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LogicalAxisHashString", LogicalAxisHashString, typeof(string));
            info.AddValue("AxisCaption", AxisCaption, typeof(string));
            info.AddValue("Speed", Speed, typeof(int));
            info.AddValue("Distance", Distance, typeof(double));
            info.AddValue("Mode", Mode, typeof(MoveMode));
            info.AddValue("IsMoveable", IsMoveable, typeof(bool));
            info.AddValue("MoveOrder", MoveOrder, typeof(int));
            info.AddValue("Unit", Unit, typeof(string));
        }

        #endregion

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
            OldValue = NewValue;                // Set the property value to the new value
            OnPropertyChanged(PropertyName);    // Raise the notify event
        }

        protected void OnPropertyChanged([CallerMemberName]string PropertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        }

        #endregion
    }
}
