using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Interfaces;
using M12.Base;

namespace Irixi_Aligner_Common.Alignment.Base
{
    public class ScanCurveBase<T>: ObservableCollectionThreadSafe<T>, IScanCurve //, INotifyPropertyChanged
    {
        string displayName = "-", prefix = "", suffix = "";
        bool isVisible = false;

        public ScanCurveBase()
        {
            Constructor();
        }

        public ScanCurveBase(string DisplayName)
        {
            Constructor();
            this.DisplayName = DisplayName;
        }

        private void Constructor()
        {
            // Set the default style of the 2D series
            LineStyle = new LineStyle(2);
            MarkerVisible = false;
            MarkerSize = 7;
            MarkerModel = new CircleMarker2DModel();
            Visible = true;
        }

        /// <summary>
        /// Get the name of the curve
        /// </summary>
        public string DisplayName
        {
            set
            {
                displayName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
                // UpdateProperty(ref displayName, value);
            }
            get
            {
                return string.Join(" ", new object[] { prefix, displayName, suffix });
            }
        }

        public string Prefix
        {
            set
            {
                // UpdateProperty(ref prefix, value);
                // OnPropertyChanged("DisplayName");
                prefix = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Prefix"));
                OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
            }
            get
            {
                return prefix;
            }
        }

        public string Suffix
        {
            set
            {
                //UpdateProperty(ref suffix, value);
                //OnPropertyChanged("DisplayName");

                suffix = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Suffix"));
                OnPropertyChanged(new PropertyChangedEventArgs("DisplayName"));
            }
            get
            {
                return suffix;
            }
        }

        #region NOTE the properties in block is used for the 2D series only

        public LineStyle LineStyle { set; get; }
        public Marker2DModel MarkerModel { get; set; }
        public int MarkerSize { get; set; }
        public bool MarkerVisible { get; set; }
        public SolidColorBrush Brush { set; get; }

        #endregion

        public bool Visible
        {
            get => isVisible;
            set
            {
                // UpdateProperty(ref isVisible, value);
                isVisible = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Visible"));
            }

        }

        #region Formulars to generate fake curve to debug
        protected Func<double, double, double, double> GaussianDistribution
        {
            get
            {
                /// You could draw the line on: 
                /// http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiIxLygxKnNxcnQoMipwaSkpKmV4cCgtMSooKHgtMyleMi8yKjFeMikpIiwiY29sb3IiOiIjMDAwMDAwIn0seyJ0eXBlIjoxMDAwfV0-
                /// Equation:
                /// 1/(1*sqrt(2*pi))*exp(-1*((x-3)^2/2*1^2))
                
                return new Func<double, double, double, double>((x, delta, u) =>
                {
                    return 1 / (delta * Math.Sqrt(2 * Math.PI)) * Math.Exp(-1 * Math.Pow((x - u), 2) / (2 * delta * delta));
                });
            }
        }

        #endregion


        #region Methods

        public virtual Point2D FindPositionWithMaxIntensity()
        {
            throw new NotImplementedException();
        }

        public virtual Point3D FindMaximalPosition3D()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
