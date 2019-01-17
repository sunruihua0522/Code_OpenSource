using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IrixiStepperControllerHelper.BindingConverter
{
    public class CvtOutputStateToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OutputState st = (OutputState)value;
            if (st == OutputState.Enabled)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            if (val == true)
                return OutputState.Enabled;
            else
                return OutputState.Disabled;
        }
    }
}
