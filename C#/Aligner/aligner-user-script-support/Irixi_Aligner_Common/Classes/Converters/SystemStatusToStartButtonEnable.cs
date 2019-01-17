using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class SystemStatusToStartButtonEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ret = false;
            SystemState state = (SystemState)value;
            switch (state)
            {
                case SystemState.IDLE:
                    ret = true;
                    break;

                default:
                    ret = false;
                    break;
            }

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
