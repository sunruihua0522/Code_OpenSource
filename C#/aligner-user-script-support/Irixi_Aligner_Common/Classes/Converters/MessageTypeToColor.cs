using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Irixi_Aligner_Common.Message;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class MessageTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = null;

            MessageType state = (MessageType)value;
            switch (state)
            {
                case MessageType.Normal:
                    brush = new SolidColorBrush(Colors.Black);
                    break;

                case MessageType.Good:
                    brush = new SolidColorBrush(Colors.DarkSeaGreen);
                    break;

                case MessageType.Warning:
                    brush = new SolidColorBrush(Colors.DarkOrange);
                    break;

                case MessageType.Error:
                    brush = new SolidColorBrush(Colors.Red);
                    break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
