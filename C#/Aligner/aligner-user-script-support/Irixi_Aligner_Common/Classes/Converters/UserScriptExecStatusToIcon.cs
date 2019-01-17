using Irixi_Aligner_Common.UserScript;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Irixi_Aligner_Common.Classes.Converters
{
    public class UserScriptExecStatusToIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            BitmapFrame image= null;
            UserScriptExecStatus status = (UserScriptExecStatus)value;

            switch (status)
            {
                case UserScriptExecStatus.NotExecuted:
                    image = null;
                    break;
                case UserScriptExecStatus.Executing:
                    image = (BitmapFrame)Application.Current.TryFindResource("IconArrowRight01");
                    break;

                case UserScriptExecStatus.Executed:
                    image = (BitmapFrame)Application.Current.TryFindResource("IconMessageConfirm");
                    break;

                case UserScriptExecStatus.Error:
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
