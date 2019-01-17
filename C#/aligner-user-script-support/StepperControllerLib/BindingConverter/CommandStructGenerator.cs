using System;
using System.Globalization;
using System.Windows.Data;

namespace IrixiStepperControllerHelper.BindingConverter
{
    public class CommandStructGenerator : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {


                CommandStruct cmd = new CommandStruct()
                {
                    Command = (EnumCommand)(values[0]),
                    AxisIndex = System.Convert.ToInt32(values[1]),
                    DriveVelocity = System.Convert.ToInt32(values[2]),
                    TotalSteps = System.Convert.ToInt32(values[3]),
                    Mode = (bool)(values[4]) ? MoveMode.ABS : MoveMode.REL,
                };

                if (cmd.Mode == MoveMode.REL && values[5].ToString() == "-")
                    cmd.TotalSteps *= -1;

                return cmd;
            }
            catch
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
