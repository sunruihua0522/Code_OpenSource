using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StepperControllerDebuger.Converter
{
    public class CvtBoolToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)value == true)
            {
                return new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
