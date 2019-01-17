using IrixiStepperControllerHelper;
using System;
using System.Globalization;
using System.Windows.Data;

namespace StepperControllerDebuger.Converter
{
    public class CvtErrorCodeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int err = (int)value;
            return DeviceStateReport.ErrorCodeToString(err);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
