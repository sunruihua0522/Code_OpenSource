using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Interfaces;

namespace Irixi_Aligner_Common.MotionControllers.Base
{
    public class RealworldUnitManager : INotifyPropertyChanged
    {
        public enum UnitType
        {
            mm,
            um,
            nm,
            deg,
            min,
            sec
        }

        #region Variables
        double _abs_pos = 0;
        double _rel_pos = 0;
        #endregion

        /// <summary>
        /// This class is used to manage the unit system, it allows user to change unit and conveter distance to move to steps, vice verse
        /// </summary>
        /// <param name="TravelDistance">The max distance of the positioner in real world unit</param>
        /// <param name="Resolution">How long it moves in real world unit per step</param>
        /// <param name="Unit">The real world unit it works on</param>
        /// <param name="DecimalPlacesDisplayed">How many decimal places will be displayed on the window</param>
        public RealworldUnitManager(double TravelDistance, double Resolution, UnitType Unit = UnitType.mm, int DecimalPlacesDisplayed = 2)
        {
            this.DecimalPlacesDisplayed = DecimalPlacesDisplayed;
            this.Unit = Unit;
            this.TravelDistance = TravelDistance;
            this.Resolution = Resolution;
            this.MaxSteps = (int)(this.TravelDistance / this.Resolution);
        }

        #region Properties 

        /// <summary>
        /// Get the maximum travel distance of the stage
        /// </summary>
        public double TravelDistance
        {
            private set;
            get;
        }

        /// <summary>
        /// Get the distance of the stage travels each step
        /// </summary>
        public double Resolution
        {
            private set;
            get;
        }

        /// <summary>
        /// Get the total steps of the stage traveling from one limit position to the other
        /// </summary>
        public int MaxSteps
        {
            private set;
            get;
        }

        /// <summary>
        /// Get the real-world unit used to measure the position in system
        /// </summary>
        public UnitType Unit
        {
            internal set;
            get;
        }

        
        /// <summary>
        /// Get the absolut distance in real world unit
        /// </summary>
        public double AbsPosition
        {
            set
            {
                UpdateProperty<double>(ref _abs_pos, value);
            }
            get
            {
                return Math.Round(_abs_pos, this.DecimalPlacesDisplayed);
            }
        }

        /// <summary>
        /// Get the relative distance in real world unit
        /// </summary>
        public double RelPosition
        {
            set
            {
                UpdateProperty<double>(ref _rel_pos, value);
            }

            get
            {
                return Math.Round(_rel_pos, this.DecimalPlacesDisplayed);
            }
        }

        /// <summary>
        /// Get how many decimal places will be displayed on the screen
        /// </summary>
        public int DecimalPlacesDisplayed
        {
            internal set;
            get;
        }

        public string HashString
        {
            get
            {
                var factor = String.Join("", new object[]
                {
                Resolution,
                MaxSteps,
                Unit
                });

                return HashGenerator.GetHashSHA256(factor);
            }
        }

        #endregion

        #region Methods

        public int ConvertDistanceToSteps(double RealworldDistance)
        {
            //if (Math.Abs(RealworldDistance) < Math.Abs(Resolution))
            //    throw new Exception($"the minimum distance supported is {Resolution}{Unit}.");

            return (int)(RealworldDistance / this.Resolution);
        }

        /// <summary>
        /// set the abs position in steps to convert to real world distance
        /// </summary>
        /// <param name="steps"></param>
        public double ConvertStepsToDistance(int Steps)
        {
            return Steps * this.Resolution;
        }

        /// <summary>
        /// This method exists specially for the Luminos P6A.
        /// For the P6A, though the travel distance is defined in the profile, 
        /// it might be different from the value stored in the P6A's flash which has the higher 
        /// priority, so the MaxSteps should be changed to the value in flash.
        /// </summary>
        /// <param name="MaxSteps"></param>
        public void ChangeMaxSteps(int MaxSteps)
        {
            this.MaxSteps = MaxSteps;

            // if the property was set, the related properties must be recalculated
            this.Resolution = this.TravelDistance / this.MaxSteps;
        }
        
        public override string ToString()
        {
            return this.Unit.ToString();
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
