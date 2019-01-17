using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IrixiStepperControllerHelper
{
    public class Axis : INotifyPropertyChanged
    {
        #region Variables

        readonly object lockAxis = new object();
        bool isBusy = false;
        private int _maxSteps = 10000;

        #endregion


        public bool IsBusy
        {
            get
            {
                lock (lockAxis)
                {
                    return isBusy;
                }
            }
            set
            {
                lock (lockAxis)
                {
                    isBusy = value;
                }
            }
        }

        /// <summary>
        /// Get the max distance(steps) the axis supports
        /// Typically, this value is set in the application's config files.
        /// </summary>
        public int MaxSteps
        {
            get => _maxSteps;
            set => _maxSteps = value;
        }

        /// <summary>
        /// Get the CCW (home point) direction limitation
        /// </summary>
        public int SoftCCWLS { get; set; }

        /// <summary>
        /// Get the CW direction limitation
        /// </summary>
        public int SoftCWLS { get; set; }

        /// <summary>
        /// Get the position after the home process
        /// Typically, this value is set in the application's config files.
        /// </summary>
        public int PosAfterHome
        {
            set;
            get;
        }

        /// <summary>
        /// Get the max drive speed
        /// </summary>
        public int MaxSpeed { get; set; }

        /// <summary>
        /// Get how many steps are used to accelerate to the max speed
        /// </summary>
        public int AccelerationSteps { set; get; }

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            ////RaisePropertyChanged(PropertyName);

        }
        #endregion
    }
}
