using M12.Definitions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class OutputStateToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DigitalIOStatus v = (DigitalIOStatus)value;
            if (v == DigitalIOStatus.OFF)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
