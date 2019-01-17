using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Irixi_Aligner_Common.View.UserControls
{
    /// <summary>
    /// Interaction logic for Axis4MassMove.xaml
    /// </summary>
    public partial class AxisForMassMove : UserControl
    {
        public AxisForMassMove()
        {
            InitializeComponent();
        }
    }

    public class IsMoveableToBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isMoveable = (bool)value;
            if (isMoveable)
            {
                //FFCDFFE6
                return new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xFF, 0xE6));
            }
            else
                return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MoveModeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = (MoveMode)value;
            if(mode == MoveMode.ABS)
            {
                return new SolidColorBrush(Colors.Coral);
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
