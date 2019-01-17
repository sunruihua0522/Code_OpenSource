using Irixi_Aligner_Common.MotionControllers.Base;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class ConvertToLogicalAxisCollection : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogicalMotionComponent)
            {
                return ((LogicalMotionComponent)value);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
