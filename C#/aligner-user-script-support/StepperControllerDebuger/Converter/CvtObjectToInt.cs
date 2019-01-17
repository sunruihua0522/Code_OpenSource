using System;
using System.Globalization;
using System.Windows.Data;

namespace StepperControllerDebuger.Converter
{

    public class CvtObjectToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value.ToString(), out int ret))
                return ret;
            else
                return -1;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
