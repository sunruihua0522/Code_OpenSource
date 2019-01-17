using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Irixi_Aligner_Common.Configuration.Layout
{
    public class Layout : INotifyPropertyChanged
    {
        public string PanelName
        {
            set;
            get;
        }

        Point _mdilocation = new Point(0, 0);
        public Point MDILocation
        {
            set
            {
                UpdateProperty<Point>(ref _mdilocation, value);
            }
            get
            {
                return _mdilocation;
            }

        }

        bool _isclosed = false;
        public bool IsClosed
        {
            set
            {
                UpdateProperty<bool>(ref _isclosed, value);
            }
            get
            {
                return _isclosed;
            }
        }

        public double Height { set; get; }

        public double Width { set; get; }

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


        }
        #endregion
    }
}