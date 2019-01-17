using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class BooleanToErrorColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isError = (bool)value;
            if (isError)
                return new SolidColorBrush(Colors.IndianRed);
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
