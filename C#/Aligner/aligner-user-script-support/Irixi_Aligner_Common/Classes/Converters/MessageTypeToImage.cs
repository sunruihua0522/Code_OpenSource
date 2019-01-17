using Irixi_Aligner_Common.Message;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class MessageTypeToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapFrame image = null;

            MessageType state = (MessageType)value;
            switch (state)
            {
                case MessageType.Normal:
                    image = null;
                    break;

                case MessageType.Good:
                    image = (BitmapFrame)Application.Current.TryFindResource("IconMessageConfirm");
                    break;

                case MessageType.Warning:
                    image = (BitmapFrame)Application.Current.TryFindResource("IconMessageWarning");
                    break;

                case MessageType.Error:
                    image = (BitmapFrame)Application.Current.TryFindResource("IconMessageError");
                    break;
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
