using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class JogPositionModeToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MoveMode mode = (MoveMode)value;
            if (mode == MoveMode.ABS)
                return "ABS";
            else
                return "REL";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
