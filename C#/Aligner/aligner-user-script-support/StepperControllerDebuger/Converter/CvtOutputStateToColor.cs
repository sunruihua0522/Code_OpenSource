using IrixiStepperControllerHelper;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StepperControllerDebuger.Converter
{
    public class CvtOutputStateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((OutputState)value == OutputState.Enabled)
            {
                return new SolidColorBrush(Colors.Lime);
            }
            else
                return new SolidColorBrush(Colors.DimGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
