using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class JogPositionModeToBoolean : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MoveMode mode = (MoveMode)value;
            if (mode == MoveMode.ABS)
                return true;
            else
                return false;
            
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            if (v)
                return MoveMode.ABS;
            else
                return MoveMode.REL;
        }
    }
}
