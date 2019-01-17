using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StepperControllerDebuger.Converter
{
    public class CvtControllerErrorToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(int.TryParse(value.ToString(), out int err))
            {
                if (err > 0)
                    return new SolidColorBrush(Colors.Red);
                else
                    return new SolidColorBrush(Colors.Gray);
            }
            else
            {
                return new SolidColorBrush(Colors.Orange);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
