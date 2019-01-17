using IrixiStepperControllerHelper;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StepperControllerDebuger.Converter
{
    public class CvtInputStateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((InputState)value == InputState.Triggered) 
            {
                return new SolidColorBrush(Colors.Red);
            }
            else
                return new SolidColorBrush(Colors.LimeGreen);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
